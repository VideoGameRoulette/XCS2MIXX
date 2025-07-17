namespace XCS2MIXX
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnRun = new System.Windows.Forms.Button();
            txtLog = new System.Windows.Forms.TextBox();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            groupBox1 = new System.Windows.Forms.GroupBox();
            tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            cmbInputExt = new System.Windows.Forms.ComboBox();
            cmbOutputExt = new System.Windows.Forms.ComboBox();
            cmbMode = new System.Windows.Forms.ComboBox();
            cmbTemplates = new System.Windows.Forms.ComboBox();
            genMixx = new System.Windows.Forms.CheckBox();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            setXConv = new System.Windows.Forms.ToolStripMenuItem();
            setPrograms = new System.Windows.Forms.ToolStripMenuItem();
            addTemplate = new System.Windows.Forms.ToolStripMenuItem();
            addInputExt = new System.Windows.Forms.ToolStripMenuItem();
            addOutputExt = new System.Windows.Forms.ToolStripMenuItem();
            showConversionSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tableLayoutPanel2.SuspendLayout();
            groupBox1.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // btnRun
            // 
            tableLayoutPanel2.SetColumnSpan(btnRun, 3);
            btnRun.Dock = System.Windows.Forms.DockStyle.Fill;
            btnRun.Location = new System.Drawing.Point(3, 109);
            btnRun.Name = "btnRun";
            btnRun.Size = new System.Drawing.Size(478, 24);
            btnRun.TabIndex = 17;
            btnRun.Text = "Run";
            btnRun.Click += btnRun_Click;
            // 
            // txtLog
            // 
            tableLayoutPanel2.SetColumnSpan(txtLog, 3);
            txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            txtLog.Location = new System.Drawing.Point(4, 139);
            txtLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtLog.Size = new System.Drawing.Size(476, 395);
            txtLog.TabIndex = 15;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            tableLayoutPanel2.Controls.Add(txtLog, 0, 6);
            tableLayoutPanel2.Controls.Add(btnRun, 0, 5);
            tableLayoutPanel2.Controls.Add(groupBox1, 0, 4);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(0, 24);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 7;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel2.Size = new System.Drawing.Size(484, 537);
            tableLayoutPanel2.TabIndex = 4;
            // 
            // groupBox1
            // 
            tableLayoutPanel2.SetColumnSpan(groupBox1, 3);
            groupBox1.Controls.Add(tableLayoutPanel3);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox1.Location = new System.Drawing.Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(478, 100);
            groupBox1.TabIndex = 13;
            groupBox1.TabStop = false;
            groupBox1.Text = "Options";
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 4;
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel3.Controls.Add(label1, 0, 0);
            tableLayoutPanel3.Controls.Add(label2, 1, 0);
            tableLayoutPanel3.Controls.Add(label3, 2, 0);
            tableLayoutPanel3.Controls.Add(label4, 3, 0);
            tableLayoutPanel3.Controls.Add(cmbInputExt, 0, 1);
            tableLayoutPanel3.Controls.Add(cmbOutputExt, 1, 1);
            tableLayoutPanel3.Controls.Add(cmbMode, 2, 1);
            tableLayoutPanel3.Controls.Add(cmbTemplates, 3, 1);
            tableLayoutPanel3.Controls.Add(genMixx, 0, 2);
            tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel3.Location = new System.Drawing.Point(3, 19);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 3;
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel3.Size = new System.Drawing.Size(472, 78);
            tableLayoutPanel3.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(3, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(88, 15);
            label1.TabIndex = 0;
            label1.Text = "Input Ext. (.xcs)";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(121, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(112, 15);
            label2.TabIndex = 1;
            label2.Text = "Output Ext. (.pgmx)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(239, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(68, 15);
            label3.TabIndex = 2;
            label3.Text = "Mode Level";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(357, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(46, 15);
            label4.TabIndex = 3;
            label4.Text = "Tooling";
            // 
            // cmbInputExt
            // 
            cmbInputExt.Dock = System.Windows.Forms.DockStyle.Fill;
            cmbInputExt.FormattingEnabled = true;
            cmbInputExt.Location = new System.Drawing.Point(3, 23);
            cmbInputExt.Name = "cmbInputExt";
            cmbInputExt.Size = new System.Drawing.Size(112, 23);
            cmbInputExt.TabIndex = 4;
            // 
            // cmbOutputExt
            // 
            cmbOutputExt.Dock = System.Windows.Forms.DockStyle.Fill;
            cmbOutputExt.FormattingEnabled = true;
            cmbOutputExt.Location = new System.Drawing.Point(121, 23);
            cmbOutputExt.Name = "cmbOutputExt";
            cmbOutputExt.Size = new System.Drawing.Size(112, 23);
            cmbOutputExt.TabIndex = 5;
            // 
            // cmbMode
            // 
            cmbMode.Dock = System.Windows.Forms.DockStyle.Fill;
            cmbMode.FormattingEnabled = true;
            cmbMode.Location = new System.Drawing.Point(239, 23);
            cmbMode.Name = "cmbMode";
            cmbMode.Size = new System.Drawing.Size(112, 23);
            cmbMode.TabIndex = 6;
            // 
            // cmbTemplates
            // 
            cmbTemplates.Dock = System.Windows.Forms.DockStyle.Fill;
            cmbTemplates.FormattingEnabled = true;
            cmbTemplates.Location = new System.Drawing.Point(357, 23);
            cmbTemplates.Name = "cmbTemplates";
            cmbTemplates.Size = new System.Drawing.Size(112, 23);
            cmbTemplates.TabIndex = 7;
            // 
            // genMixx
            // 
            genMixx.AutoSize = true;
            genMixx.Checked = true;
            genMixx.CheckState = System.Windows.Forms.CheckState.Checked;
            genMixx.Location = new System.Drawing.Point(3, 53);
            genMixx.Name = "genMixx";
            genMixx.Size = new System.Drawing.Size(102, 19);
            genMixx.TabIndex = 8;
            genMixx.Text = "Generate Mixx";
            genMixx.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { settingsToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new System.Drawing.Size(484, 24);
            menuStrip1.TabIndex = 5;
            menuStrip1.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { setXConv, setPrograms, addTemplate, addInputExt, addOutputExt, showConversionSettingsToolStripMenuItem });
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            settingsToolStripMenuItem.Text = "Settings";
            // 
            // setXConv
            // 
            setXConv.Name = "setXConv";
            setXConv.Size = new System.Drawing.Size(224, 22);
            setXConv.Text = "Set XConverter.exe Directory";
            setXConv.Click += setXConv_Click;
            // 
            // setPrograms
            // 
            setPrograms.Name = "setPrograms";
            setPrograms.Size = new System.Drawing.Size(224, 22);
            setPrograms.Text = "Set Programs Folder";
            setPrograms.Click += setPrograms_Click;
            // 
            // addTemplate
            // 
            addTemplate.Name = "addTemplate";
            addTemplate.Size = new System.Drawing.Size(224, 22);
            addTemplate.Text = "Add Tooling Template";
            addTemplate.Click += addTemplate_Click;
            // 
            // addInputExt
            // 
            addInputExt.Name = "addInputExt";
            addInputExt.Size = new System.Drawing.Size(224, 22);
            addInputExt.Text = "Add Input Extension";
            addInputExt.Click += addInputExt_Click;
            // 
            // addOutputExt
            // 
            addOutputExt.Name = "addOutputExt";
            addOutputExt.Size = new System.Drawing.Size(224, 22);
            addOutputExt.Text = "Add Output Extension";
            addOutputExt.Click += addOutputExt_Click;
            // 
            // showConversionSettingsToolStripMenuItem
            // 
            showConversionSettingsToolStripMenuItem.Name = "showConversionSettingsToolStripMenuItem";
            showConversionSettingsToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            showConversionSettingsToolStripMenuItem.Text = "Show Conversion Settings";
            showConversionSettingsToolStripMenuItem.Click += showConversionSettingsToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(484, 561);
            Controls.Add(tableLayoutPanel2);
            Controls.Add(menuStrip1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimumSize = new System.Drawing.Size(500, 400);
            Name = "Form1";
            Text = "XConvert Toolkit";
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            groupBox1.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbInputExt;
        private System.Windows.Forms.ComboBox cmbOutputExt;
        private System.Windows.Forms.ComboBox cmbMode;
        private System.Windows.Forms.ComboBox cmbTemplates;
        private System.Windows.Forms.CheckBox genMixx;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setXConv;
        private System.Windows.Forms.ToolStripMenuItem setPrograms;
        private System.Windows.Forms.ToolStripMenuItem addTemplate;
        private System.Windows.Forms.ToolStripMenuItem addInputExt;
        private System.Windows.Forms.ToolStripMenuItem addOutputExt;
        private System.Windows.Forms.ToolStripMenuItem showConversionSettingsToolStripMenuItem;
    }
}

