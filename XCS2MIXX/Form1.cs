using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace XCS2MIXX
{
    public partial class Form1 : Form
    {
        private class Settings
        {
            public string ConverterPath { get; set; } = "";
            public string RootDir { get; set; } = "";
            public Dictionary<string, string> ToolTemplates { get; set; }
                = new Dictionary<string, string>();
        }

        private Settings _settings = new();
        private static readonly string SettingsFilePath =
            Path.Combine(Application.StartupPath, "settings.json");

        public Form1()
        {
            InitializeComponent();
            LoadSettings();
            txtConverterPath.Text = _settings.ConverterPath;
            txtRootDir.Text = _settings.RootDir;
            foreach (var kv in _settings.ToolTemplates)
                _toolTemplates[kv.Key] = kv.Value;
            RefreshTemplateList();
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
        }

        private void SaveSettings()
        {
            try
            {
                // sync UI back into settings
                _settings.ConverterPath = txtConverterPath.Text.Trim();
                _settings.RootDir = txtRootDir.Text.Trim();
                _settings.ToolTemplates = new Dictionary<string, string>(_toolTemplates);

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

        private readonly Dictionary<string, string> _toolTemplates = new();
        private void RefreshTemplateList()
        {
            lstTemplates.Items.Clear();
            foreach (var label in _toolTemplates.Keys)
                lstTemplates.Items.Add(label);
            if (lstTemplates.Items.Count > 0)
                lstTemplates.SelectedIndex = 0;
        }

        private void btnBrowseConverter_Click(object sender, EventArgs e)
        {
            using var fd = new OpenFileDialog { Filter = "XConverter.exe|XConverter.exe" };
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtConverterPath.Text = fd.FileName;
                SaveSettings();
            }
        }

        private void btnBrowseRoot_Click(object sender, EventArgs e)
        {
            using var bd = new FolderBrowserDialog();
            if (bd.ShowDialog() == DialogResult.OK)
            {
                txtRootDir.Text = bd.SelectedPath;
                SaveSettings();
            }
        }

        private void btnBrowseTpl1_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Tool template (*.tlgx)|*.tlgx|All files|*.*"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
                txtTemplatePath.Text = ofd.FileName;
        }

        private void btnBrowseTpl2_Click(object sender, EventArgs e)
        {
            var label = txtTemplateLabel.Text.Trim();
            var path = txtTemplatePath.Text.Trim();
            if (string.IsNullOrEmpty(label) || !File.Exists(path))
            {
                MessageBox.Show("Enter a name and pick a valid .tlgx file.",
                                "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _toolTemplates[label] = path;
            RefreshTemplateList();
            SaveSettings();
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            // 1) Clear previous log
            txtLog.Clear();

            // 2) Gather inputs
            string converter = txtConverterPath.Text.Trim();
            string root = txtRootDir.Text.Trim();
            string label = lstTemplates.SelectedItem?.ToString() ?? "";

            // 3) Resolve selected template
            if (!_toolTemplates.TryGetValue(label, out string tpl))
            {
                MessageBox.Show("Select a valid template.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4) Debug print
            txtLog.AppendText($"[DEBUG] Converter: {converter}\r\n");
            txtLog.AppendText($"[DEBUG] Root Dir:  {root}\r\n");
            txtLog.AppendText($"[DEBUG] Template: {label} ➔ {tpl}\r\n\r\n");

            // 5) Validate paths
            if (!File.Exists(converter))
            {
                MessageBox.Show($"XConverter.exe not found at:\r\n{converter}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Directory.Exists(root))
            {
                MessageBox.Show($"Root folder not found:\r\n{root}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!File.Exists(tpl))
            {
                MessageBox.Show($"Template file not found:\r\n{tpl}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 6) Group all .xcs files by their containing directory
            var dirGroups = Directory
                .GetFiles(root, "*.xcs", SearchOption.AllDirectories)
                .GroupBy(Path.GetDirectoryName)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .ToList();

            if (dirGroups.Count == 0)
            {
                txtLog.AppendText("No .xcs files found under the root folder.\r\n");
                return;
            }

            // 7) Disable Run button while working
            btnRun.Enabled = false;
            int totalFiles = dirGroups.Sum(g => g.Count());
            txtLog.AppendText($"Found {totalFiles} .xcs files in {dirGroups.Count} folders.\r\n");

            // 8) Process each folder
            foreach (var group in dirGroups)
            {
                string dir = group.Key!;
                // Sort by filename (r59f0001, r59f0002, etc.)
                var xcsList = group
                    .OrderBy(path => Path.GetFileNameWithoutExtension(path))
                    .ToList();

                txtLog.AppendText($"\r\n--- Processing folder: {dir} ({xcsList.Count} files) ---\r\n");

                // 8a) Convert each .xcs to .pgmx
                foreach (var xcs in xcsList)
                {
                    string baseN = Path.GetFileNameWithoutExtension(xcs);
                    string pgmx = Path.Combine(dir, baseN + ".pgmx");

                    var psi = new ProcessStartInfo
                    {
                        FileName = converter,
                        Arguments = $"-s -ow -i \"{xcs}\" -o \"{pgmx}\" -t \"{tpl}\" -m 0",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    txtLog.AppendText($" • Converting {baseN}.xcs → {baseN}.pgmx … ");
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

                // 8b) Write sorted CSV (overwrite if exists)
                string csvPath = Path.Combine(dir, "Mixx_XCS-PGMX-MIXX.csv");
                try
                {
                    if (File.Exists(csvPath))
                        File.Delete(csvPath);

                    using var writer = new StreamWriter(csvPath, false);
                    foreach (var xcs in xcsList)
                    {
                        string pgmxName = Path.GetFileNameWithoutExtension(xcs) + ".pgmx";
                        writer.WriteLine($"[PRG]={pgmxName};");
                    }
                    txtLog.AppendText($"[DEBUG] Wrote CSV: {csvPath}\r\n");
                }
                catch (Exception ex)
                {
                    txtLog.AppendText($"CSV WRITE ERROR: {ex.Message}\r\n");
                    // skip mixx step for this folder
                    continue;
                }

                // 8c) Generate .mixx from the CSV (overwrite if exists)
                string mixxPath = Path.Combine(dir, "Mixx_XCS-PGMX-MIXX.mixx");
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

            // 9) All done!
            txtLog.AppendText("\r\nAll done!\r\n");
            btnRun.Enabled = true;
        }

    }
}
