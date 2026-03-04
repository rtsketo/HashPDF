using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HashPDF.WinForms
{
    partial class MainForm
    {
        private IContainer components;
        private Panel headerPanel;
        private Panel languagePanel;
        private TableLayoutPanel bodyLayout;
        private Panel leftColumn;
        private Panel rightColumn;
        private Label titleLabel;
        private Label subtitleLabel;
        private Label themeLabel;
        private ComboBox themeComboBox;
        private Label languageLabel;
        private ComboBox languageComboBox;
        private Label inputTitleLabel;
        private Label inputSubtitleLabel;
        private Panel dropSurfaceHostPanel;
        private Panel resultPanel;
        private Label resultTitleLabel;
        private Label hashCaptionLabel;
        private TextBox hashTextBox;
        private Label fileCaptionLabel;
        private Label fileValueLabel;
        private Label outputCaptionLabel;
        private Label outputValueLabel;
        private Button openFolderButton;
        private Button openPdfButton;
        private Label statusLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.languagePanel = new System.Windows.Forms.Panel();
            this.themeLabel = new System.Windows.Forms.Label();
            this.themeComboBox = new System.Windows.Forms.ComboBox();
            this.languageLabel = new System.Windows.Forms.Label();
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.titleLabel = new System.Windows.Forms.Label();
            this.subtitleLabel = new System.Windows.Forms.Label();
            this.bodyLayout = new System.Windows.Forms.TableLayoutPanel();
            this.leftColumn = new System.Windows.Forms.Panel();
            this.inputTitleLabel = new System.Windows.Forms.Label();
            this.inputSubtitleLabel = new System.Windows.Forms.Label();
            this.dropSurfaceHostPanel = new System.Windows.Forms.Panel();
            this.rightColumn = new System.Windows.Forms.Panel();
            this.resultPanel = new System.Windows.Forms.Panel();
            this.resultTitleLabel = new System.Windows.Forms.Label();
            this.hashCaptionLabel = new System.Windows.Forms.Label();
            this.hashTextBox = new System.Windows.Forms.TextBox();
            this.fileCaptionLabel = new System.Windows.Forms.Label();
            this.fileValueLabel = new System.Windows.Forms.Label();
            this.outputCaptionLabel = new System.Windows.Forms.Label();
            this.outputValueLabel = new System.Windows.Forms.Label();
            this.openFolderButton = new System.Windows.Forms.Button();
            this.openPdfButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.headerPanel.SuspendLayout();
            this.languagePanel.SuspendLayout();
            this.bodyLayout.SuspendLayout();
            this.leftColumn.SuspendLayout();
            this.rightColumn.SuspendLayout();
            this.resultPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.Controls.Add(this.languagePanel);
            this.headerPanel.Controls.Add(this.titleLabel);
            this.headerPanel.Controls.Add(this.subtitleLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(32, 28, 32, 12);
            this.headerPanel.Size = new System.Drawing.Size(1180, 128);
            this.headerPanel.TabIndex = 2;
            // 
            // languagePanel
            // 
            this.languagePanel.BackColor = System.Drawing.Color.White;
            this.languagePanel.Controls.Add(this.themeLabel);
            this.languagePanel.Controls.Add(this.themeComboBox);
            this.languagePanel.Controls.Add(this.languageLabel);
            this.languagePanel.Controls.Add(this.languageComboBox);
            this.languagePanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.languagePanel.Location = new System.Drawing.Point(729, 28);
            this.languagePanel.Name = "languagePanel";
            this.languagePanel.Size = new System.Drawing.Size(419, 88);
            this.languagePanel.TabIndex = 0;
            this.languagePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.ResultPanelPaint);
            // 
            // themeLabel
            // 
            this.themeLabel.AutoSize = true;
            this.themeLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.themeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(108)))), ((int)(((byte)(104)))));
            this.themeLabel.Location = new System.Drawing.Point(18, 16);
            this.themeLabel.Name = "themeLabel";
            this.themeLabel.Size = new System.Drawing.Size(0, 15);
            this.themeLabel.TabIndex = 0;
            this.themeLabel.Text = "Theme";
            // 
            // themeComboBox
            // 
            this.themeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.themeComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.themeComboBox.FormattingEnabled = true;
            this.themeComboBox.Items.AddRange(new object[] {
            "Dark",
            "Light"});
            this.themeComboBox.Location = new System.Drawing.Point(70, 42);
            this.themeComboBox.Name = "themeComboBox";
            this.themeComboBox.Size = new System.Drawing.Size(124, 25);
            this.themeComboBox.TabIndex = 1;
            this.themeComboBox.SelectedIndexChanged += new System.EventHandler(this.ThemeComboBoxSelectedIndexChanged);
            // 
            // languageLabel
            // 
            this.languageLabel.AutoSize = true;
            this.languageLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.languageLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(108)))), ((int)(((byte)(104)))));
            this.languageLabel.Location = new System.Drawing.Point(232, 16);
            this.languageLabel.Name = "languageLabel";
            this.languageLabel.Size = new System.Drawing.Size(0, 15);
            this.languageLabel.TabIndex = 2;
            this.languageLabel.Text = "Language";
            // 
            // languageComboBox
            // 
            this.languageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.languageComboBox.FormattingEnabled = true;
            this.languageComboBox.Items.AddRange(new object[] {
            "Ελληνικά",
            "English"});
            this.languageComboBox.Location = new System.Drawing.Point(232, 42);
            this.languageComboBox.Name = "languageComboBox";
            this.languageComboBox.Size = new System.Drawing.Size(176, 25);
            this.languageComboBox.TabIndex = 3;
            this.languageComboBox.SelectedIndexChanged += new System.EventHandler(this.LanguageComboBoxSelectedIndexChanged);
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(34)))), ((int)(((byte)(32)))));
            this.titleLabel.Location = new System.Drawing.Point(29, 16);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(0, 51);
            this.titleLabel.TabIndex = 1;
            this.titleLabel.Text = "HashPDF";
            // 
            // subtitleLabel
            // 
            this.subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(108)))), ((int)(((byte)(104)))));
            this.subtitleLabel.Location = new System.Drawing.Point(34, 70);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Size = new System.Drawing.Size(660, 36);
            this.subtitleLabel.TabIndex = 2;
            this.subtitleLabel.Text = "Create a SHA-512 hash and a PDF proof in the same folder.";
            // 
            // bodyLayout
            // 
            this.bodyLayout.ColumnCount = 2;
            this.bodyLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52F));
            this.bodyLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48F));
            this.bodyLayout.Controls.Add(this.leftColumn, 0, 0);
            this.bodyLayout.Controls.Add(this.rightColumn, 1, 0);
            this.bodyLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bodyLayout.Location = new System.Drawing.Point(0, 128);
            this.bodyLayout.Name = "bodyLayout";
            this.bodyLayout.Padding = new System.Windows.Forms.Padding(32, 0, 32, 20);
            this.bodyLayout.RowCount = 1;
            this.bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bodyLayout.Size = new System.Drawing.Size(1180, 588);
            this.bodyLayout.TabIndex = 0;
            // 
            // leftColumn
            // 
            this.leftColumn.BackColor = System.Drawing.Color.White;
            this.leftColumn.Controls.Add(this.inputTitleLabel);
            this.leftColumn.Controls.Add(this.inputSubtitleLabel);
            this.leftColumn.Controls.Add(this.dropSurfaceHostPanel);
            this.leftColumn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftColumn.Location = new System.Drawing.Point(32, 0);
            this.leftColumn.Margin = new System.Windows.Forms.Padding(0, 0, 14, 0);
            this.leftColumn.Name = "leftColumn";
            this.leftColumn.Padding = new System.Windows.Forms.Padding(24);
            this.leftColumn.Size = new System.Drawing.Size(566, 568);
            this.leftColumn.TabIndex = 0;
            this.leftColumn.Paint += new System.Windows.Forms.PaintEventHandler(this.ResultPanelPaint);
            // 
            // inputTitleLabel
            // 
            this.inputTitleLabel.AutoSize = true;
            this.inputTitleLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.inputTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(34)))), ((int)(((byte)(32)))));
            this.inputTitleLabel.Location = new System.Drawing.Point(24, 24);
            this.inputTitleLabel.Name = "inputTitleLabel";
            this.inputTitleLabel.Size = new System.Drawing.Size(0, 28);
            this.inputTitleLabel.TabIndex = 0;
            this.inputTitleLabel.Text = "Select File";
            // 
            // inputSubtitleLabel
            // 
            this.inputSubtitleLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.inputSubtitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(108)))), ((int)(((byte)(104)))));
            this.inputSubtitleLabel.Location = new System.Drawing.Point(24, 58);
            this.inputSubtitleLabel.Name = "inputSubtitleLabel";
            this.inputSubtitleLabel.Size = new System.Drawing.Size(470, 42);
            this.inputSubtitleLabel.TabIndex = 1;
            this.inputSubtitleLabel.Text = "Drop a file into the panel or click the drop area to choose one.";
            // 
            // dropSurfaceHostPanel
            // 
            this.dropSurfaceHostPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dropSurfaceHostPanel.BackColor = System.Drawing.Color.Transparent;
            this.dropSurfaceHostPanel.Location = new System.Drawing.Point(27, 133);
            this.dropSurfaceHostPanel.Name = "dropSurfaceHostPanel";
            this.dropSurfaceHostPanel.Size = new System.Drawing.Size(512, 408);
            this.dropSurfaceHostPanel.TabIndex = 2;
            // 
            // rightColumn
            // 
            this.rightColumn.Controls.Add(this.resultPanel);
            this.rightColumn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightColumn.Location = new System.Drawing.Point(626, 0);
            this.rightColumn.Margin = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.rightColumn.Name = "rightColumn";
            this.rightColumn.Size = new System.Drawing.Size(522, 568);
            this.rightColumn.TabIndex = 1;
            // 
            // resultPanel
            // 
            this.resultPanel.BackColor = System.Drawing.Color.White;
            this.resultPanel.Controls.Add(this.resultTitleLabel);
            this.resultPanel.Controls.Add(this.hashCaptionLabel);
            this.resultPanel.Controls.Add(this.hashTextBox);
            this.resultPanel.Controls.Add(this.fileCaptionLabel);
            this.resultPanel.Controls.Add(this.fileValueLabel);
            this.resultPanel.Controls.Add(this.outputCaptionLabel);
            this.resultPanel.Controls.Add(this.outputValueLabel);
            this.resultPanel.Controls.Add(this.openFolderButton);
            this.resultPanel.Controls.Add(this.openPdfButton);
            this.resultPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultPanel.Location = new System.Drawing.Point(0, 0);
            this.resultPanel.Name = "resultPanel";
            this.resultPanel.Padding = new System.Windows.Forms.Padding(24);
            this.resultPanel.Size = new System.Drawing.Size(522, 568);
            this.resultPanel.TabIndex = 0;
            this.resultPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.ResultPanelPaint);
            // 
            // resultTitleLabel
            // 
            this.resultTitleLabel.AutoSize = true;
            this.resultTitleLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.resultTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(34)))), ((int)(((byte)(32)))));
            this.resultTitleLabel.Location = new System.Drawing.Point(24, 24);
            this.resultTitleLabel.Name = "resultTitleLabel";
            this.resultTitleLabel.Size = new System.Drawing.Size(0, 28);
            this.resultTitleLabel.TabIndex = 0;
            this.resultTitleLabel.Text = "Hash Result";
            // 
            // hashCaptionLabel
            // 
            this.hashCaptionLabel.AutoSize = true;
            this.hashCaptionLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.hashCaptionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(108)))), ((int)(((byte)(104)))));
            this.hashCaptionLabel.Location = new System.Drawing.Point(24, 62);
            this.hashCaptionLabel.Name = "hashCaptionLabel";
            this.hashCaptionLabel.Size = new System.Drawing.Size(0, 15);
            this.hashCaptionLabel.TabIndex = 1;
            this.hashCaptionLabel.Text = "SHA-512 Hash";
            // 
            // hashTextBox
            // 
            this.hashTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hashTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(249)))), ((int)(((byte)(247)))));
            this.hashTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hashTextBox.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.hashTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(50)))), ((int)(((byte)(65)))));
            this.hashTextBox.Location = new System.Drawing.Point(24, 86);
            this.hashTextBox.Multiline = true;
            this.hashTextBox.Name = "hashTextBox";
            this.hashTextBox.ReadOnly = true;
            this.hashTextBox.Size = new System.Drawing.Size(471, 116);
            this.hashTextBox.TabIndex = 2;
            this.hashTextBox.Text = "3A5902E0D4E5A9F9C5B8B08D6D6E65A5F632D34816F0C21213E88A53FD7A17764CC99E2AA2CC9334A95" +
    "7F0E91E95EAC34F6079F57390E2D1F2F7B669FC0A4B71";
            // 
            // fileCaptionLabel
            // 
            this.fileCaptionLabel.AutoSize = true;
            this.fileCaptionLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.fileCaptionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(108)))), ((int)(((byte)(104)))));
            this.fileCaptionLabel.Location = new System.Drawing.Point(24, 224);
            this.fileCaptionLabel.Name = "fileCaptionLabel";
            this.fileCaptionLabel.Size = new System.Drawing.Size(0, 15);
            this.fileCaptionLabel.TabIndex = 3;
            this.fileCaptionLabel.Text = "Source File";
            // 
            // fileValueLabel
            // 
            this.fileValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileValueLabel.AutoEllipsis = true;
            this.fileValueLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(249)))), ((int)(((byte)(247)))));
            this.fileValueLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fileValueLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.fileValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(50)))), ((int)(((byte)(65)))));
            this.fileValueLabel.Location = new System.Drawing.Point(24, 246);
            this.fileValueLabel.Name = "fileValueLabel";
            this.fileValueLabel.Padding = new System.Windows.Forms.Padding(10);
            this.fileValueLabel.Size = new System.Drawing.Size(471, 58);
            this.fileValueLabel.TabIndex = 4;
            this.fileValueLabel.Text = "C:\\Samples\\topographic_plan.dxf";
            // 
            // outputCaptionLabel
            // 
            this.outputCaptionLabel.AutoSize = true;
            this.outputCaptionLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.outputCaptionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(108)))), ((int)(((byte)(104)))));
            this.outputCaptionLabel.Location = new System.Drawing.Point(24, 330);
            this.outputCaptionLabel.Name = "outputCaptionLabel";
            this.outputCaptionLabel.Size = new System.Drawing.Size(0, 15);
            this.outputCaptionLabel.TabIndex = 5;
            this.outputCaptionLabel.Text = "Generated PDF";
            // 
            // outputValueLabel
            // 
            this.outputValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputValueLabel.AutoEllipsis = true;
            this.outputValueLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(249)))), ((int)(((byte)(247)))));
            this.outputValueLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.outputValueLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.outputValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(50)))), ((int)(((byte)(65)))));
            this.outputValueLabel.Location = new System.Drawing.Point(24, 352);
            this.outputValueLabel.Name = "outputValueLabel";
            this.outputValueLabel.Padding = new System.Windows.Forms.Padding(10);
            this.outputValueLabel.Size = new System.Drawing.Size(471, 58);
            this.outputValueLabel.TabIndex = 6;
            this.outputValueLabel.Text = "C:\\Samples\\topographic_plan.hash.pdf";
            // 
            // openFolderButton
            // 
            this.openFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.openFolderButton.BackColor = System.Drawing.Color.White;
            this.openFolderButton.Enabled = false;
            this.openFolderButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(226)))), ((int)(((byte)(222)))));
            this.openFolderButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.openFolderButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.openFolderButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(50)))), ((int)(((byte)(65)))));
            this.openFolderButton.Location = new System.Drawing.Point(24, 500);
            this.openFolderButton.Name = "openFolderButton";
            this.openFolderButton.Size = new System.Drawing.Size(168, 44);
            this.openFolderButton.TabIndex = 7;
            this.openFolderButton.Text = "Open Folder";
            this.openFolderButton.UseVisualStyleBackColor = false;
            this.openFolderButton.Click += new System.EventHandler(this.OpenFolderButtonClick);
            // 
            // openPdfButton
            // 
            this.openPdfButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.openPdfButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(115)))), ((int)(((byte)(90)))));
            this.openPdfButton.Enabled = false;
            this.openPdfButton.FlatAppearance.BorderSize = 0;
            this.openPdfButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.openPdfButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.openPdfButton.ForeColor = System.Drawing.Color.White;
            this.openPdfButton.Location = new System.Drawing.Point(208, 500);
            this.openPdfButton.Name = "openPdfButton";
            this.openPdfButton.Size = new System.Drawing.Size(168, 44);
            this.openPdfButton.TabIndex = 8;
            this.openPdfButton.Text = "Open PDF";
            this.openPdfButton.UseVisualStyleBackColor = false;
            this.openPdfButton.Click += new System.EventHandler(this.OpenPdfButtonClick);
            // 
            // statusLabel
            // 
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.statusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(108)))), ((int)(((byte)(104)))));
            this.statusLabel.Location = new System.Drawing.Point(0, 716);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Padding = new System.Windows.Forms.Padding(32, 0, 32, 12);
            this.statusLabel.Size = new System.Drawing.Size(1180, 44);
            this.statusLabel.TabIndex = 1;
            this.statusLabel.Text = "Ready for a new file.";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(245)))), ((int)(((byte)(241)))));
            this.ClientSize = new System.Drawing.Size(1180, 760);
            this.Controls.Add(this.bodyLayout);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.headerPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.MinimumSize = new System.Drawing.Size(1040, 680);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HashPDF";
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.languagePanel.ResumeLayout(false);
            this.languagePanel.PerformLayout();
            this.bodyLayout.ResumeLayout(false);
            this.leftColumn.ResumeLayout(false);
            this.leftColumn.PerformLayout();
            this.rightColumn.ResumeLayout(false);
            this.resultPanel.ResumeLayout(false);
            this.resultPanel.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
