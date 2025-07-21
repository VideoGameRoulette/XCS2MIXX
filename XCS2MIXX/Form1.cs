using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XCS2MIXX
{
    public partial class Form1 : Form
    {
        private class Settings
        {
            public string ConverterPath { get; set; } = string.Empty;
            public string RootDir { get; set; } = string.Empty;
            public Dictionary<string, string> ToolTemplates { get; set; } = new();
            public List<string> InputExtensions { get; set; } = new() { ".xcs", ".pgmx", ".pgm", ".xxl", ".csv", ".dxf" };
            public List<string> OutputExtensions { get; set; } = new() { ".pgmx", ".mixx", ".xxl", ".csv" };
            public List<int> Modes { get; set; } = new() { 0, 1, 2, 3, 4, 5, 10, 11, 12, 14, 16, 18 };
            public bool GenerateMixx { get; set; } = true;
        }

        private Settings _settings = new();
        private static readonly string SettingsFilePath = Path.Combine(Application.StartupPath, "settings.json");
        private readonly Dictionary<string, string> _toolTemplates = new();
        private string _converterPath = string.Empty;
        private string _rootDir = string.Empty;

        public Form1()
        {
            InitializeComponent();
            LoadSettings();
            InitializeUIFromSettings();
            // Optional: initial validation
            ValidateRequiredSettings();
        }

        private void LoadSettings()
        {
            if (!File.Exists(SettingsFilePath)) return;
            try
            {
                var json = File.ReadAllText(SettingsFilePath);
                var s = JsonSerializer.Deserialize<Settings>(json);
                if (s != null)
                {
                    _settings = s;
                    // restore templates

                    _toolTemplates.Clear();
                    foreach (var kv in _settings.ToolTemplates)
                        _toolTemplates[kv.Key] = kv.Value;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed loading settings:\r\n{ex.Message}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            _converterPath = _settings.ConverterPath;
            _rootDir = _settings.RootDir;
            
        }

        private void SaveSettings()
        {
            _settings.ConverterPath = _converterPath;
            _settings.RootDir = _rootDir;
            // persist templates
            _settings.ToolTemplates = new Dictionary<string, string>(_toolTemplates);

            try
            {
                var opts = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_settings, opts);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed saving settings:\r\n{ex.Message}", "Save Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InitializeUIFromSettings()
        {
            genMixx.Checked = _settings.GenerateMixx;
            programFolderTB.Text = _settings.RootDir;

            // Templates
            cmbTemplates.Items.Clear();
            foreach (var kv in _toolTemplates)
                cmbTemplates.Items.Add(kv.Key);
            if (cmbTemplates.Items.Count > 0)
                cmbTemplates.SelectedIndex = 0;

            // Modes
            cmbMode.Items.Clear();
            foreach (var mode in _settings.Modes)
                cmbMode.Items.Add(mode.ToString());
            cmbMode.SelectedIndex = 0;
            cmbMode.SelectedIndexChanged += cmbMode_SelectedIndexChanged;

            // Input/Output extensions (display only)
            cmbInputExt.Items.Clear();
            foreach (var ext in _settings.InputExtensions)
                cmbInputExt.Items.Add(ext);
            cmbOutputExt.Items.Clear();
            foreach (var ext in _settings.OutputExtensions)
                cmbOutputExt.Items.Add(ext);

            // Set initial in/out based on mode
            UpdateExtFromMode();
            cmbInputExt.Enabled = false;
            cmbOutputExt.Enabled = false;
        }

        // Event: mode changed
        private void cmbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateExtFromMode();
        }

        private void UpdateExtFromMode()
        {
            if (int.TryParse(cmbMode.SelectedItem?.ToString(), out var mode))
            {
                var (inExt, outExt) = GetExtForMode(mode);
                if (cmbInputExt.Items.Contains(inExt)) cmbInputExt.SelectedItem = inExt;
                if (cmbOutputExt.Items.Contains(outExt)) cmbOutputExt.SelectedItem = outExt;
                // Update helper text label for mode description
                helperText.Text = GetModeDescription(mode);
            }
        }

        // Path setters
        private void setXConv_Click(object sender, EventArgs e)
            => SetPath(ref _converterPath, "XConverter.exe", "[DEBUG] Converter: ");

        private void setPrograms_Click(object sender, EventArgs e)
        {
            SetPath(ref _rootDir, string.Empty, "[DEBUG] Root Dir: ", folder: true);
            programFolderTB.Text = _rootDir;
        }

        private void SetPath(ref string target, string filter, string logPrefix, bool folder = false)
        {
            if (folder)
            {
                using var dlg = new FolderBrowserDialog();
                if (dlg.ShowDialog() != DialogResult.OK) return;
                target = dlg.SelectedPath;
            }
            else
            {
                using var dlg = new OpenFileDialog { Filter = filter + "|" + filter };
                if (dlg.ShowDialog() != DialogResult.OK) return;
                target = dlg.FileName;
            }
            SaveSettings();
            txtLog.Clear();
            txtLog.AppendText($"{logPrefix}{target}\r\n");
        }

        // Template management
        private void addTemplate_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Tool template (*.tlgx)|*.tlgx|All files|*.*",
                Title = "Select Tool Template"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            var path = ofd.FileName;
            var label = Interaction.InputBox("Enter label:", "Template Label",
                                            Path.GetFileNameWithoutExtension(path)).Trim();
            if (string.IsNullOrEmpty(label) || !File.Exists(path))
            {
                MessageBox.Show("Invalid label or file.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _toolTemplates[label] = path;
            InitializeUIFromSettings();
            SaveSettings();
            txtLog.Clear();
            txtLog.AppendText($"[DEBUG] Added template: {label} -> {path}\r\n");
        }

        private bool ValidateRequiredSettings()
        {
            bool ok = true;
            if (string.IsNullOrWhiteSpace(_converterPath) || !File.Exists(_converterPath))
            {
                MessageBox.Show("Please set XConverter path.", "Missing Converter",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                setXConv_Click(this, EventArgs.Empty);
                ok = false;
            }
            if (string.IsNullOrWhiteSpace(_rootDir) || !Directory.Exists(_rootDir))
            {
                MessageBox.Show("Please set root folder.", "Missing Root",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                setPrograms_Click(this, EventArgs.Empty);
                ok = false;
            }
            if (!_toolTemplates.Any())
            {
                MessageBox.Show("Please add a .tlgx template.", "Missing Template",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                addTemplate_Click(this, EventArgs.Empty);
                ok = false;
            }
            return ok;
        }

        private (string inExt, string outExt) GetExtForMode(int mode)
            => mode switch
            {
                0 => (".xcs", ".pgmx"),
                1 => (".pgmx", ".pgmx"),
                2 => (".pgmx", ".pgmx"),
                3 => (".xcs", ".pgmx"),
                4 => (".xxl", ".pgmx"),
                5 => (".pgm", ".pgmx"),
                10 => (".pgmx", ".xxl"),
                11 => (".csv", ".mixx"),
                12 => (".xcs", ".pgmx"),
                14 => (".pgm", ".pgmx"),
                16 => (".mixx", ".csv"),
                18 => (".dxf", ".pgmx"),
                _ => (".xcs", ".pgmx"),
            };

        private async void btnRun_Click(object sender, EventArgs e)
        {
            if (!ValidateRequiredSettings()) return;

            if (cmbTemplates.SelectedItem is not string label || !_toolTemplates.TryGetValue(label, out var tplPath))
            {
                MessageBox.Show("Select a valid template.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int mode = int.TryParse(cmbMode.SelectedItem?.ToString(), out var m) ? m : 0;
            var (inExt, outExt) = GetExtForMode(mode);

            txtLog.Clear();
            txtLog.AppendText($"[DEBUG] Converter: {_converterPath}\r\n");
            txtLog.AppendText($"[DEBUG] Root Dir:  {_rootDir}\r\n");
            txtLog.AppendText($"[DEBUG] Template:  {label} -> {tplPath}\r\n");
            txtLog.AppendText($"[DEBUG] Mode:       {mode}\r\n");
            txtLog.AppendText($"[DEBUG] Exts:       {inExt} -> {outExt}\r\n");
            txtLog.AppendText($"[DEBUG] Mixx:       {genMixx.Checked}\r\n\r\n");

            var groups = Directory.GetFiles(_rootDir, $"*{inExt}", SearchOption.AllDirectories)
                                  .GroupBy(Path.GetDirectoryName)
                                  .Where(g => !string.IsNullOrEmpty(g.Key));

            btnRun.Enabled = false;
            foreach (var grp in groups)
                await ProcessGroupAsync(grp.ToList(), tplPath, mode, inExt, outExt);
            txtLog.AppendText("\r\nAll done!\r\n");
            btnRun.Enabled = true;
        }

        private async Task ProcessGroupAsync(List<string> files, string tplPath,
                                             int mode, string inExt, string outExt)
        {
            string dir = Path.GetDirectoryName(files.First())!;
            txtLog.AppendText($"\r\n--- Processing: {dir} ({files.Count}) ---\r\n");
            await ConvertFilesAsync(files, tplPath, mode, inExt, outExt);
            if (genMixx.Checked && outExt == ".mixx")
            {
                string csv = WriteCsv(dir, files, inExt, outExt);
                await GenerateMixxAsync(csv, tplPath);
            }
        }

        private async Task ConvertFilesAsync(List<string> files, string tplPath,
                                             int mode, string inExt, string outExt)
        {
            foreach (var file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                string outFile = Path.Combine(Path.GetDirectoryName(file)!, name + outExt);
                txtLog.AppendText($" • {name}{inExt} -> {name}{outExt} ... ");
                try
                {
                    var psi = new ProcessStartInfo(_converterPath,
                        $"-s -ow -i \"{file}\" -o \"{outFile}\" -t \"{tplPath}\" -m {mode}")
                    { UseShellExecute = false, CreateNoWindow = true, RedirectStandardError = true };
                    using var proc = Process.Start(psi);
                    if (proc == null)
                    {
                        txtLog.AppendText("START FAILED\r\n");
                        continue;
                    }
                    string err = await proc.StandardError.ReadToEndAsync();
                    await proc.WaitForExitAsync();
                    txtLog.AppendText(proc.ExitCode == 0 ? "OK\r\n" : $"ERROR({proc.ExitCode})\r\n{err}\r\n");
                }
                catch (Exception ex)
                {
                    txtLog.AppendText($"EXCEPTION: {ex.Message}\r\n");
                }
            }
        }

        private string WriteCsv(string dir, List<string> files, string inExt, string outExt)
        {
            string csvName = $"MIXX_{inExt.Trim('.')}_TO_{outExt.Trim('.')}.csv";
            string path = Path.Combine(dir, csvName);
            try
            {
                if (File.Exists(path)) File.Delete(path);
                using var writer = new StreamWriter(path);
                foreach (var f in files)
                {
                    string bn = Path.GetFileNameWithoutExtension(f);
                    writer.WriteLine($"[PRG]={bn}{outExt};");
                }
                txtLog.AppendText($"[DEBUG] CSV: {path}\r\n");
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"CSV ERROR: {ex.Message}\r\n");
            }
            return path;
        }

        private async Task GenerateMixxAsync(string csvPath, string tplPath)
        {
            string mixx = Path.ChangeExtension(csvPath, ".mixx");
            txtLog.AppendText($" • MIXX -> {Path.GetFileName(mixx)} ... ");
            try
            {
                if (File.Exists(mixx)) File.Delete(mixx);
                var psi = new ProcessStartInfo(_converterPath,
                    $"-s -m 11 -i \"{csvPath}\" -t \"{tplPath}\" -o \"{mixx}\"")
                { UseShellExecute = false, CreateNoWindow = true, RedirectStandardError = true };
                using var proc = Process.Start(psi);
                if (proc == null)
                {
                    txtLog.AppendText("START FAILED\r\n");
                    return;
                }
                string err = await proc.StandardError.ReadToEndAsync();
                await proc.WaitForExitAsync();
                txtLog.AppendText(proc.ExitCode == 0 ? "OK\r\n" : $"ERROR({proc.ExitCode})\r\n{err}\r\n");
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"MIXX EXCEPTION: {ex.Message}\r\n");
            }
        }
        // Mode descriptions helper
        private string GetModeDescription(int mode)
        {
            return mode switch
            {
                0 => "Mode 0: .xcs → .pgmx (basic conversion)",
                1 => "Mode 1: .pgmx → .pgmx (merge)",
                2 => "Mode 2: .pgmx → .pgmx (optimized)",
                3 => "Mode 3: .xcs → .pgmx (optimized)",
                4 => "Mode 4: .xxl → .pgmx",
                5 => "Mode 5: .pgm → .pgmx",
                10 => "Mode 10: .pgmx → .xxl",
                11 => "Mode 11: .csv → .mixx (mixx project)",
                12 => "Mode 12: .xcs → .pgmx (complete project)",
                14 => "Mode 14: .pgm → .pgmx (suction setup)",
                16 => "Mode 16: .mixx → .csv (time estimation)",
                18 => "Mode 18: .dxf → .pgmx",
                _ => "Unknown mode"
            };
        }

        // Handler for the "Show Conversion Settings" menu item
        private void showConversionSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
            if (cmbTemplates.SelectedItem is string templ && _toolTemplates.TryGetValue(templ, out var path))
            {
                int mode = int.TryParse(cmbMode.SelectedItem?.ToString(), out var m) ? m : 0;
                var (inExt, outExt) = GetExtForMode(mode);
                txtLog.AppendText($"[DEBUG] Converter: {_converterPath}\r\n");
                txtLog.AppendText($"[DEBUG] Root Dir: {_rootDir}\r\n");
                txtLog.AppendText($"[DEBUG] Template: {templ} -> {path}\r\n");
                txtLog.AppendText($"[DEBUG] Mode: {mode}\r\n");
                txtLog.AppendText($"[DEBUG] Exts: {inExt} -> {outExt}\r\n");
                txtLog.AppendText($"[DEBUG] Mixx: {genMixx.Checked}\r\n\r\n");
            }
        }

        private void setProgramFolderBtn_Click(object sender, EventArgs e)
        {
            SetPath(ref _rootDir, string.Empty, "[DEBUG] Root Dir: ", folder: true);
            programFolderTB.Text = _rootDir;
        }
    }
}
