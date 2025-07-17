using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
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
        private static readonly string SettingsFilePath =
            Path.Combine(Application.StartupPath, "settings.json");

        private readonly Dictionary<string, string> _toolTemplates = new();
        private string _converterPath = "";
        private string _rootDir = "";

        public Form1()
        {
            InitializeComponent();
            LoadSettings();
            InitializeUIFromSettings();
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

        private void setXConv_Click(object sender, EventArgs e)
        {
            using var fd = new OpenFileDialog { Filter = "XConverter.exe|XConverter.exe" };
            if (fd.ShowDialog() == DialogResult.OK)
            {
                _converterPath = fd.FileName;
                SaveSettings();
                MessageBox.Show($"XConverter path set:\n{_converterPath}", "Set Path", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void setPrograms_Click(object sender, EventArgs e)
        {
            using var bd = new FolderBrowserDialog();
            if (bd.ShowDialog() == DialogResult.OK)
            {
                _rootDir = bd.SelectedPath;
                SaveSettings();
                MessageBox.Show($"Root folder set:\n{_rootDir}", "Set Root Folder", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void addTemplate_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Tool template (*.tlgx)|*.tlgx|All files|*.*",
                Title = "Select Tool Template"
            };

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            string selectedPath = ofd.FileName;

            string label = Interaction.InputBox(
                "Enter a label name for this tooling template:",
                "Template Label",
                Path.GetFileNameWithoutExtension(selectedPath)
            ).Trim();

            if (string.IsNullOrEmpty(label) || !File.Exists(selectedPath))
            {
                MessageBox.Show("Enter a name and pick a valid .tlgx file.",
                                "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _toolTemplates[label] = selectedPath;
            RefreshTemplateList();
            SaveSettings();
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            txtLog.Clear();

            string converter = _converterPath;
            string root = _rootDir;
            string label = cmbTemplates.SelectedItem?.ToString() ?? "";
            string inputExt = cmbInputExt.SelectedItem?.ToString() ?? ".xcs";
            string outputExt = cmbOutputExt.SelectedItem?.ToString() ?? ".pgmx";
            int selectedMode = int.TryParse(cmbMode.SelectedItem?.ToString(), out int val) ? val : 0;

            if (!_toolTemplates.TryGetValue(label, out string tpl))
            {
                MessageBox.Show("Select a valid template.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            txtLog.AppendText($"[DEBUG] Converter: {converter}\r\n");
            txtLog.AppendText($"[DEBUG] Root Dir:  {root}\r\n");
            txtLog.AppendText($"[DEBUG] Template:  {label} ➔ {tpl}\r\n");
            txtLog.AppendText($"[DEBUG] Extension: {inputExt} ➔ {outputExt}, Mode: {selectedMode}\r\n");
            txtLog.AppendText($"[DEBUG] Generate .mixx: {genMixx.Checked}\r\n\r\n");

            if (!File.Exists(converter) || !Directory.Exists(root) || !File.Exists(tpl))
            {
                MessageBox.Show("Invalid converter, root directory, or template file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var dirGroups = Directory
                .GetFiles(root, $"*{inputExt}", SearchOption.AllDirectories)
                .GroupBy(Path.GetDirectoryName)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .ToList();

            if (dirGroups.Count == 0)
            {
                txtLog.AppendText($"No {inputExt} files found under the root folder.\r\n");
                return;
            }

            btnRun.Enabled = false;
            int totalFiles = dirGroups.Sum(g => g.Count());
            txtLog.AppendText($"Found {totalFiles} {inputExt} files in {dirGroups.Count} folders.\r\n");

            foreach (var group in dirGroups)
            {
                string dir = group.Key!;
                var fileList = group.OrderBy(path => Path.GetFileNameWithoutExtension(path)).ToList();

                txtLog.AppendText($"\r\n--- Processing folder: {dir} ({fileList.Count} files) ---\r\n");

                foreach (var inputFile in fileList)
                {
                    string baseName = Path.GetFileNameWithoutExtension(inputFile);
                    string outputFile = Path.Combine(dir, baseName + outputExt);

                    var psi = new ProcessStartInfo
                    {
                        FileName = converter,
                        Arguments = $"-s -ow -i \"{inputFile}\" -o \"{outputFile}\" -t \"{tpl}\" -m {selectedMode}",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    txtLog.AppendText($" • Converting {baseName}{inputExt} → {baseName}{outputExt} … ");
                    try
                    {
                        using var proc = Process.Start(psi);
                        if (proc == null)
                        {
                            txtLog.AppendText("START FAILED\r\n");
                            continue;
                        }
                        string stderr = await proc.StandardError.ReadToEndAsync();
                        await proc.WaitForExitAsync();

                        txtLog.AppendText(proc.ExitCode == 0
                            ? "OK\r\n"
                            : $"ERROR({proc.ExitCode})\r\n{stderr}\r\n");
                    }
                    catch (Exception ex)
                    {
                        txtLog.AppendText($"EXCEPTION: {ex.Message}\r\n");
                    }
                }

                if (!genMixx.Checked) continue;

                string csvPath = Path.Combine(dir, $"Mixx_{inputExt.Trim('.')}-{outputExt.Trim('.')}-MIXX.csv");
                try
                {
                    if (File.Exists(csvPath))
                        File.Delete(csvPath);

                    using var writer = new StreamWriter(csvPath, false);
                    foreach (var f in fileList)
                    {
                        string pgmxName = Path.GetFileNameWithoutExtension(f) + outputExt;
                        writer.WriteLine($"[PRG]={pgmxName};");
                    }
                    txtLog.AppendText($"[DEBUG] Wrote CSV: {csvPath}\r\n");
                }
                catch (Exception ex)
                {
                    txtLog.AppendText($"CSV WRITE ERROR: {ex.Message}\r\n");
                    continue;
                }

                string mixxPath = Path.Combine(dir, Path.GetFileNameWithoutExtension(csvPath) + ".mixx");
                if (File.Exists(mixxPath))
                    File.Delete(mixxPath);

                var psi2 = new ProcessStartInfo
                {
                    FileName = converter,
                    Arguments = $"-s -m 11 -i \"{csvPath}\" -t \"{tpl}\" -o \"{mixxPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                txtLog.AppendText($" • Generating mixx → {Path.GetFileName(mixxPath)} … ");
                try
                {
                    using var proc2 = Process.Start(psi2);
                    if (proc2 == null)
                    {
                        txtLog.AppendText("START FAILED\r\n");
                    }
                    else
                    {
                        string stderr2 = await proc2.StandardError.ReadToEndAsync();
                        await proc2.WaitForExitAsync();

                        txtLog.AppendText(proc2.ExitCode == 0
                            ? "OK\r\n"
                            : $"ERROR({proc2.ExitCode})\r\n{stderr2}\r\n");
                    }
                }
                catch (Exception ex)
                {
                    txtLog.AppendText($"EXCEPTION: {ex.Message}\r\n");
                }
            }

            txtLog.AppendText("\r\nAll done!\r\n");
            btnRun.Enabled = true;
        }

    }
}
