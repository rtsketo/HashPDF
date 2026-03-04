using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using HashPDF.WinForms.Controls;

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
        private Label languageLabel;
        private ComboBox languageComboBox;
        private Label inputTitleLabel;
        private Label inputSubtitleLabel;
        private DropSurfacePanel dropSurfacePanel;
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
            this.components = new Container();
            this.headerPanel = new Panel();
            this.languagePanel = new Panel();
            this.languageLabel = new Label();
            this.languageComboBox = new ComboBox();
            this.titleLabel = new Label();
            this.subtitleLabel = new Label();
            this.bodyLayout = new TableLayoutPanel();
            this.leftColumn = new Panel();
            this.inputTitleLabel = new Label();
            this.inputSubtitleLabel = new Label();
            this.dropSurfacePanel = new DropSurfacePanel();
            this.rightColumn = new Panel();
            this.resultPanel = new Panel();
            this.resultTitleLabel = new Label();
            this.hashCaptionLabel = new Label();
            this.hashTextBox = new TextBox();
            this.fileCaptionLabel = CreateSectionLabel(224);
            this.fileValueLabel = CreateMetaLabel(246);
            this.outputCaptionLabel = CreateSectionLabel(330);
            this.outputValueLabel = CreateMetaLabel(352);
            this.openFolderButton = CreateSecondaryButton();
            this.openPdfButton = CreatePrimaryButton();
            this.statusLabel = new Label();
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
            this.headerPanel.Dock = DockStyle.Top;
            this.headerPanel.Height = 128;
            this.headerPanel.Padding = new Padding(32, 28, 32, 12);
            this.headerPanel.Name = "headerPanel";
            //
            // languagePanel
            //
            this.languagePanel.BackColor = Color.White;
            this.languagePanel.Controls.Add(this.languageLabel);
            this.languagePanel.Controls.Add(this.languageComboBox);
            this.languagePanel.Dock = DockStyle.Right;
            this.languagePanel.Name = "languagePanel";
            this.languagePanel.Paint += new PaintEventHandler(this.ResultPanelPaint);
            this.languagePanel.Size = new Size(214, 88);
            //
            // languageLabel
            //
            this.languageLabel.AutoSize = true;
            this.languageLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 161);
            this.languageLabel.ForeColor = Color.FromArgb(97, 108, 104);
            this.languageLabel.Location = new Point(18, 16);
            this.languageLabel.Name = "languageLabel";
            //
            // languageComboBox
            //
            this.languageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.languageComboBox.FlatStyle = FlatStyle.Flat;
            this.languageComboBox.FormattingEnabled = true;
            this.languageComboBox.Location = new Point(18, 42);
            this.languageComboBox.Name = "languageComboBox";
            this.languageComboBox.Size = new Size(176, 25);
            this.languageComboBox.SelectedIndexChanged += new System.EventHandler(this.LanguageComboBoxSelectedIndexChanged);
            //
            // titleLabel
            //
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new Font("Segoe UI Semibold", 28F, FontStyle.Bold, GraphicsUnit.Point, 161);
            this.titleLabel.ForeColor = Color.FromArgb(26, 34, 32);
            this.titleLabel.Location = new Point(32, 22);
            this.titleLabel.Name = "titleLabel";
            //
            // subtitleLabel
            //
            this.subtitleLabel.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 161);
            this.subtitleLabel.ForeColor = Color.FromArgb(97, 108, 104);
            this.subtitleLabel.Location = new Point(34, 60);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Size = new Size(660, 46);
            //
            // bodyLayout
            //
            this.bodyLayout.ColumnCount = 2;
            this.bodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
            this.bodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
            this.bodyLayout.Controls.Add(this.leftColumn, 0, 0);
            this.bodyLayout.Controls.Add(this.rightColumn, 1, 0);
            this.bodyLayout.Dock = DockStyle.Fill;
            this.bodyLayout.Name = "bodyLayout";
            this.bodyLayout.Padding = new Padding(32, 0, 32, 20);
            this.bodyLayout.RowCount = 1;
            this.bodyLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            //
            // leftColumn
            //
            this.leftColumn.BackColor = Color.White;
            this.leftColumn.Controls.Add(this.inputTitleLabel);
            this.leftColumn.Controls.Add(this.inputSubtitleLabel);
            this.leftColumn.Controls.Add(this.dropSurfacePanel);
            this.leftColumn.Dock = DockStyle.Fill;
            this.leftColumn.Margin = new Padding(0, 0, 14, 0);
            this.leftColumn.Name = "leftColumn";
            this.leftColumn.Padding = new Padding(24);
            this.leftColumn.Paint += new PaintEventHandler(this.ResultPanelPaint);
            //
            // inputTitleLabel
            //
            this.inputTitleLabel.AutoSize = true;
            this.inputTitleLabel.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold, GraphicsUnit.Point, 161);
            this.inputTitleLabel.ForeColor = Color.FromArgb(26, 34, 32);
            this.inputTitleLabel.Location = new Point(24, 24);
            this.inputTitleLabel.Name = "inputTitleLabel";
            //
            // inputSubtitleLabel
            //
            this.inputSubtitleLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 161);
            this.inputSubtitleLabel.ForeColor = Color.FromArgb(97, 108, 104);
            this.inputSubtitleLabel.Location = new Point(24, 58);
            this.inputSubtitleLabel.Name = "inputSubtitleLabel";
            this.inputSubtitleLabel.Size = new Size(470, 42);
            //
            // dropSurfacePanel
            //
            this.dropSurfacePanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dropSurfacePanel.Location = new Point(24, 118);
            this.dropSurfacePanel.Name = "dropSurfacePanel";
            this.dropSurfacePanel.Size = new Size(470, 420);
            this.dropSurfacePanel.FileDropped += new System.EventHandler<FileDroppedEventArgs>(this.DropSurfacePanelFileDropped);
            this.dropSurfacePanel.Click += new System.EventHandler(this.DropSurfacePanelClick);
            //
            // rightColumn
            //
            this.rightColumn.Controls.Add(this.resultPanel);
            this.rightColumn.Dock = DockStyle.Fill;
            this.rightColumn.Margin = new Padding(14, 0, 0, 0);
            this.rightColumn.Name = "rightColumn";
            //
            // resultPanel
            //
            this.resultPanel.BackColor = Color.White;
            this.resultPanel.Controls.Add(this.resultTitleLabel);
            this.resultPanel.Controls.Add(this.hashCaptionLabel);
            this.resultPanel.Controls.Add(this.hashTextBox);
            this.resultPanel.Controls.Add(this.fileCaptionLabel);
            this.resultPanel.Controls.Add(this.fileValueLabel);
            this.resultPanel.Controls.Add(this.outputCaptionLabel);
            this.resultPanel.Controls.Add(this.outputValueLabel);
            this.resultPanel.Controls.Add(this.openFolderButton);
            this.resultPanel.Controls.Add(this.openPdfButton);
            this.resultPanel.Dock = DockStyle.Fill;
            this.resultPanel.Name = "resultPanel";
            this.resultPanel.Padding = new Padding(24);
            this.resultPanel.Paint += new PaintEventHandler(this.ResultPanelPaint);
            //
            // resultTitleLabel
            //
            this.resultTitleLabel.AutoSize = true;
            this.resultTitleLabel.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold, GraphicsUnit.Point, 161);
            this.resultTitleLabel.ForeColor = Color.FromArgb(26, 34, 32);
            this.resultTitleLabel.Location = new Point(24, 24);
            this.resultTitleLabel.Name = "resultTitleLabel";
            //
            // hashCaptionLabel
            //
            this.hashCaptionLabel.AutoSize = true;
            this.hashCaptionLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 161);
            this.hashCaptionLabel.ForeColor = Color.FromArgb(97, 108, 104);
            this.hashCaptionLabel.Location = new Point(24, 62);
            this.hashCaptionLabel.Name = "hashCaptionLabel";
            //
            // hashTextBox
            //
            this.hashTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.hashTextBox.BackColor = Color.FromArgb(247, 249, 247);
            this.hashTextBox.BorderStyle = BorderStyle.FixedSingle;
            this.hashTextBox.Font = new Font("Consolas", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 161);
            this.hashTextBox.ForeColor = Color.FromArgb(37, 50, 65);
            this.hashTextBox.Location = new Point(24, 86);
            this.hashTextBox.Multiline = true;
            this.hashTextBox.Name = "hashTextBox";
            this.hashTextBox.ReadOnly = true;
            this.hashTextBox.ScrollBars = ScrollBars.Horizontal;
            this.hashTextBox.Size = new Size(416, 116);
            this.hashTextBox.WordWrap = false;
            //
            // fileCaptionLabel
            //
            this.fileCaptionLabel.Left = 24;
            //
            // fileValueLabel
            //
            this.fileValueLabel.Left = 24;
            //
            // outputCaptionLabel
            //
            this.outputCaptionLabel.Left = 24;
            //
            // outputValueLabel
            //
            this.outputValueLabel.Left = 24;
            //
            // openFolderButton
            //
            this.openFolderButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.openFolderButton.Enabled = false;
            this.openFolderButton.Location = new Point(24, 426);
            this.openFolderButton.Name = "openFolderButton";
            this.openFolderButton.Size = new Size(168, 44);
            this.openFolderButton.UseVisualStyleBackColor = false;
            this.openFolderButton.Click += new System.EventHandler(this.OpenFolderButtonClick);
            //
            // openPdfButton
            //
            this.openPdfButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.openPdfButton.Enabled = false;
            this.openPdfButton.Location = new Point(208, 426);
            this.openPdfButton.Name = "openPdfButton";
            this.openPdfButton.Size = new Size(168, 44);
            this.openPdfButton.UseVisualStyleBackColor = false;
            this.openPdfButton.Click += new System.EventHandler(this.OpenPdfButtonClick);
            //
            // statusLabel
            //
            this.statusLabel.Dock = DockStyle.Bottom;
            this.statusLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 161);
            this.statusLabel.ForeColor = Color.FromArgb(97, 108, 104);
            this.statusLabel.Height = 44;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Padding = new Padding(32, 0, 32, 12);
            //
            // MainForm
            //
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(243, 245, 241);
            this.ClientSize = new Size(1180, 760);
            this.Controls.Add(this.bodyLayout);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.headerPanel);
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 161);
            this.MinimumSize = new Size(1040, 680);
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
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
