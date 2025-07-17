using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace XCS2MIXX
{
    public partial class Form1 : Form
    {
        private class Settings
        {
            public string ConverterPath { get; set; } = "";
            public string RootDir { get; set; } = "";
            public Dictionary<string, string> ToolTemplates { get; set; } = new();
            public List<string> InputExtensions { get; set; } = new() { ".xcs" };
            public List<string> OutputExtensions { get; set; } = new() { ".pgmx" };
            public List<int> Modes { get; set; } = new() { 0, 11 };
            public bool GenerateMixx { get; set; } = true;
        }

        private Settings _settings = new();
        private static readonly string SettingsFilePath = Path.Combine(Application.StartupPath, "settings.json");
        private readonly Dictionary<string, string> _toolTemplates = new();
        private string _converterPath = "";
        private string _rootDir = "";

        public Form1()
        {
            InitializeComponent();
            LoadSettings();
            InitializeUIFromSettings();
            ValidateRequiredSettings();
        }

        private void LoadSettings()
        {
            if (!File.Exists(SettingsFilePath)) return;
            try
            {
                var json = File.ReadAllText(SettingsFilePath);
                var s = JsonSerializer.Deserialize<Settings>(json);
                if (s != null) _settings = s;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed loading settings:\r\n{ex.Message}",
                                "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            _converterPath = _settings.ConverterPath;
            _rootDir = _settings.RootDir;
        }

        private void SaveSettings()
        {
            try
            {
                _settings.ConverterPath = _converterPath;
                _settings.RootDir = _rootDir;
                _settings.ToolTemplates = new Dictionary<string, string>(_toolTemplates);
                _settings.GenerateMixx = genMixx.Checked;

                var opts = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_settings, opts);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed saving settings:\r\n{ex.Message}",
                                "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InitializeUIFromSettings()
        {
            genMixx.Checked = _settings.GenerateMixx;
            _toolTemplates.Clear();
            foreach (var kv in _settings.ToolTemplates)
                _toolTemplates[kv.Key] = kv.Value;

            RefreshTemplateList();

            cmbInputExt.Items.Clear();
            cmbOutputExt.Items.Clear();
            cmbMode.Items.Clear();

            foreach (var ext in _settings.InputExtensions)
                cmbInputExt.Items.Add(ext);
            foreach (var ext in _settings.OutputExtensions)
                cmbOutputExt.Items.Add(ext);
            foreach (var mode in _settings.Modes)
                cmbMode.Items.Add(mode.ToString());

            if (cmbInputExt.Items.Count > 0) cmbInputExt.SelectedIndex = 0;
            if (cmbOutputExt.Items.Count > 0) cmbOutputExt.SelectedIndex = 0;
            if (cmbMode.Items.Count > 0) cmbMode.SelectedIndex = 0;
        }

        private void RefreshTemplateList()
        {
            cmbTemplates.Items.Clear();
            foreach (var label in _toolTemplates.Keys)
                cmbTemplates.Items.Add(label);
            if (cmbTemplates.Items.Count > 0)
                cmbTemplates.SelectedIndex = 0;
        }

        private void setXConverterPath() =>
            SetPath(ref _converterPath, "XConverter.exe", "[DEBUG] Converter: ");

        private void setXConv_Click(object sender, EventArgs e) => setXConverterPath();

        private void setProgramPath() =>
            SetPath(ref _rootDir, string.Empty, "[DEBUG] Programs Folder: ", folder: true);

        private void setPrograms_Click(object sender, EventArgs e) => setProgramPath();

        private void SetPath(ref string target, string filter, string logPrefix, bool folder = false)
        {
            if (folder)
            {
                using var bd = new FolderBrowserDialog();
                if (bd.ShowDialog() != DialogResult.OK) return;
                target = bd.SelectedPath;
            }
            else
            {
                using var fd = new OpenFileDialog { Filter = filter + "|" + filter };
                if (fd.ShowDialog() != DialogResult.OK) return;
                target = fd.FileName;
            }
            SaveSettings();
            txtLog.Clear();
            txtLog.AppendText($"{logPrefix}{target}\r\n");
        }

        private void addTemplate_Click(object sender, EventArgs e) // renamed to match event handler

        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Tool template (*.tlgx)|*.tlgx|All files|*.*",
                Title = "Select Tool Template"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            var selected = ofd.FileName;
            var label = Interaction.InputBox("Enter a label:", "Template Label",
                                            Path.GetFileNameWithoutExtension(selected)).Trim();
            if (string.IsNullOrWhiteSpace(label) || !File.Exists(selected))
            {
                MessageBox.Show("Invalid label or file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _toolTemplates[label] = selected;
            RefreshTemplateList();
            SaveSettings();
            txtLog.Clear();
            txtLog.AppendText($"[DEBUG] Template: {label} => {selected}\r\n");
        }

        private void addInputExt_Click(object sender, EventArgs e)
        {
            var ext = Interaction.InputBox("New input extension (e.g. .xcs):", "Add Input Ext", ".xcs").Trim();
            if (string.IsNullOrEmpty(ext)) return;
            if (!ext.StartsWith(".")) ext = "." + ext;
            if (!_settings.InputExtensions.Contains(ext))
            {
                _settings.InputExtensions.Add(ext);
                cmbInputExt.Items.Add(ext);
                cmbInputExt.SelectedItem = ext;
                SaveSettings();
            }
            else MessageBox.Show("Already exists.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void addOutputExt_Click(object sender, EventArgs e)
        {
            var ext = Interaction.InputBox("New output extension (e.g. .pgmx):", "Add Output Ext", ".pgmx").Trim();
            if (string.IsNullOrEmpty(ext)) return;
            if (!ext.StartsWith(".")) ext = "." + ext;
            if (!_settings.OutputExtensions.Contains(ext))
            {
                _settings.OutputExtensions.Add(ext);
                cmbOutputExt.Items.Add(ext);
                cmbOutputExt.SelectedItem = ext;
                SaveSettings();
            }
            else MessageBox.Show("Already exists.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ValidateRequiredSettings()
        {
            var ok = true;
            if (string.IsNullOrWhiteSpace(_converterPath) || !File.Exists(_converterPath))
            {
                MessageBox.Show("Set path to XConverter.exe", "Missing Converter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                setXConverterPath(); ok = false;
            }
            if (string.IsNullOrWhiteSpace(_rootDir) || !Directory.Exists(_rootDir))
            {
                MessageBox.Show("Set root folder for .xcs files", "Missing Root", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                setProgramPath(); ok = false;
            }
            if (!_toolTemplates.Any())
            {
                MessageBox.Show("Add at least one .tlgx template", "Missing Template", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                addTemplate_Click(null, null); ok = false;
            }
            RefreshTemplateList();
            return ok;
        }

        private string GetSelectedTemplateLabel() => cmbTemplates.SelectedItem?.ToString() ?? string.Empty;
        private string GetSelectedTemplatePath()
            => _toolTemplates.TryGetValue(GetSelectedTemplateLabel(), out var p) ? p : string.Empty;

        private string GetSelectedInputExt() => cmbInputExt.SelectedItem?.ToString() ?? ".xcs";
        private string GetSelectedOutputExt() => cmbOutputExt.SelectedItem?.ToString() ?? ".pgmx";
        private int GetSelectedMode() => int.TryParse(cmbMode.SelectedItem?.ToString(), out var m) ? m : 0;

        private async void btnRun_Click(object sender, EventArgs e)
        {
            if (!ValidateRequiredSettings()) return;

            var tplPath = GetSelectedTemplatePath();
            if (string.IsNullOrEmpty(tplPath) || !File.Exists(tplPath))
            {
                MessageBox.Show("Select a valid template.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LogRunSettings(_converterPath, _rootDir, tplPath,
                           GetSelectedInputExt(), GetSelectedOutputExt(),
                           GetSelectedMode(), genMixx.Checked);

            var groups = Directory.GetFiles(_rootDir, $"*{GetSelectedInputExt()}", SearchOption.AllDirectories)
                                  .GroupBy(Path.GetDirectoryName)
                                  .Where(g => !string.IsNullOrEmpty(g.Key));

            btnRun.Enabled = false;
            foreach (var grp in groups)
                await ProcessGroupAsync(grp.ToList(), grp.Key!, tplPath);

            txtLog.AppendText("\r\nAll done!\r\n");
            btnRun.Enabled = true;
        }

        private void LogRunSettings(string converter, string root, string tplPath,
                                    string inputExt, string outputExt,
                                    int mode, bool mixx)
        {
            txtLog.Clear();
            txtLog.AppendText($"[DEBUG] Converter: {converter}\r\n");
            txtLog.AppendText($"[DEBUG] Root Dir:  {root}\r\n");
            txtLog.AppendText($"[DEBUG] Template: {tplPath}\r\n");
            txtLog.AppendText($"[DEBUG] Extension: {inputExt} ➔ {outputExt}, Mode: {mode}\r\n");
            txtLog.AppendText($"[DEBUG] Generate .mixx: {mixx}\r\n\r\n");
        }

        private async Task ProcessGroupAsync(List<string> files, string dir, string tplPath)
        {
            txtLog.AppendText($"\r\n--- Processing {dir} ({files.Count} files) ---\r\n");
            await ConvertFilesAsync(files, tplPath);
            if (genMixx.Checked)
            {
                var csv = WriteCsv(dir, files);
                await GenerateMixxAsync(csv, tplPath);
            }
        }

        private async Task ConvertFilesAsync(List<string> inputs, string tpl)
        {
            var outExt = GetSelectedOutputExt();
            var mode = GetSelectedMode();
            foreach (var inp in inputs)
            {
                var name = Path.GetFileNameWithoutExtension(inp);
                var outFile = Path.Combine(Path.GetDirectoryName(inp)!, name + outExt);
                txtLog.AppendText($" • Converting {name}{GetSelectedInputExt()} → {name}{outExt} … ");
                try
                {
                    var psi = new ProcessStartInfo(_converterPath,
                        $"-s -ow -i \"{inp}\" -o \"{outFile}\" -t \"{tpl}\" -m {mode}")
                    { UseShellExecute = false, CreateNoWindow = true, RedirectStandardError = true };

                    using var p = Process.Start(psi);
                    if (p == null) { txtLog.AppendText("START FAILED\r\n"); continue; }
                    var err = await p.StandardError.ReadToEndAsync();
                    await p.WaitForExitAsync();
                    txtLog.AppendText(p.ExitCode == 0 ? "OK\r\n" : $"ERROR({p.ExitCode})\r\n{err}\r\n");
                }
                catch (Exception ex)
                {
                    txtLog.AppendText($"EXCEPTION: {ex.Message}\r\n");
                }
            }
        }

        private string WriteCsv(string dir, List<string> files)
        {
            var csvName = $"Mixx_{GetSelectedInputExt().Trim('.')}–{GetSelectedOutputExt().Trim('.')}–MIXX.csv";
            var path = Path.Combine(dir, csvName);
            try
            {
                if (File.Exists(path)) File.Delete(path);
                using var w = new StreamWriter(path);
                foreach (var f in files)
                    w.WriteLine($"[PRG]={Path.GetFileNameWithoutExtension(f)}{GetSelectedOutputExt()};");
                txtLog.AppendText($"[DEBUG] CSV: {path}\r\n");
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"CSV ERROR: {ex.Message}\r\n");
            }
            return path;
        }

        private async Task GenerateMixxAsync(string csvPath, string tpl)
        {
            var mixx = Path.ChangeExtension(csvPath, ".mixx");
            txtLog.AppendText($" • Generating mixx → {Path.GetFileName(mixx)} … ");
            try
            {
                if (File.Exists(mixx)) File.Delete(mixx);
                var psi = new ProcessStartInfo(_converterPath,
                    $"-s -m 11 -i \"{csvPath}\" -t \"{tpl}\" -o \"{mixx}\"")
                { UseShellExecute = false, CreateNoWindow = true, RedirectStandardError = true };

                using var p = Process.Start(psi);
                if (p == null) { txtLog.AppendText("START FAILED\r\n"); return; }
                var err = await p.StandardError.ReadToEndAsync();
                await p.WaitForExitAsync();
                txtLog.AppendText(p.ExitCode == 0 ? "OK\r\n" : $"ERROR({p.ExitCode})\r\n{err}\r\n");
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"EXCEPTION: {ex.Message}\r\n");
            }
        }

        private void showConversionSettingsToolStripMenuItem_Click(object sender, EventArgs e)
            => LogRunSettings(_converterPath, _rootDir, GetSelectedTemplatePath(),
                             GetSelectedInputExt(), GetSelectedOutputExt(),
                             GetSelectedMode(), genMixx.Checked);
    }
}
