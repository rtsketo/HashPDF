using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using HashPDF.WinForms.Controls;
using HashPDF.WinForms.Exceptions;
using HashPDF.WinForms.Localization;
using HashPDF.WinForms.Models;
using HashPDF.WinForms.Services;

namespace HashPDF.WinForms
{
    public partial class MainForm : Form
    {
        private readonly BackgroundWorker worker;
        private DropSurfacePanel dropSurfacePanel;
        private AppLanguage currentLanguage;
        private AppTheme currentTheme;
        private string selectedFilePath;
        private HashPdfResult lastResult;
        private bool suppressOptionEvents;
        private Color panelBorderColor;
        private Color panelTopAccentColor;

        public MainForm()
        {
            InitializeComponent();
            InitializeDropSurfacePanel();

            worker = new BackgroundWorker();
            worker.DoWork += WorkerDoWork;
            worker.RunWorkerCompleted += WorkerRunWorkerCompleted;

            currentLanguage = AppLanguage.Greek;
            currentTheme = AppTheme.Light;
            ApplyLanguage();
            ApplyTheme();
        }

        private void InitializeDropSurfacePanel()
        {
            dropSurfacePanel = new DropSurfacePanel();
            dropSurfacePanel.Dock = DockStyle.Fill;
            dropSurfacePanel.Margin = new Padding(0);
            dropSurfacePanel.FileDropped += DropSurfacePanelFileDropped;
            dropSurfacePanel.Click += DropSurfacePanelClick;
            dropSurfaceHostPanel.Controls.Clear();
            dropSurfaceHostPanel.Controls.Add(dropSurfacePanel);
        }

        private static Button CreatePrimaryButton()
        {
            Button button = new Button();
            button.Height = 44;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.FromArgb(24, 115, 90);
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 161);
            button.Cursor = Cursors.Hand;
            return button;
        }

        private static Button CreateSecondaryButton()
        {
            Button button = new Button();
            button.Height = 44;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Color.FromArgb(220, 226, 222);
            button.BackColor = Color.White;
            button.ForeColor = Color.FromArgb(37, 50, 65);
            button.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 161);
            button.Cursor = Cursors.Hand;
            return button;
        }

        private static Label CreateSectionLabel(int top)
        {
            Label label = new Label();
            label.AutoSize = true;
            label.Left = 0;
            label.Top = top;
            label.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 161);
            label.ForeColor = Color.FromArgb(97, 108, 104);
            return label;
        }

        private static Label CreateMetaLabel(int top)
        {
            Label label = new Label();
            label.AutoEllipsis = true;
            label.Left = 0;
            label.Top = top;
            label.Width = 416;
            label.Height = 58;
            label.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 161);
            label.ForeColor = Color.FromArgb(37, 50, 65);
            label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label.BackColor = Color.FromArgb(247, 249, 247);
            label.BorderStyle = BorderStyle.FixedSingle;
            label.Padding = new Padding(10, 10, 10, 10);
            return label;
        }

        private void RefreshLanguageOptions()
        {
            languageComboBox.Items.Clear();
            languageComboBox.Items.Add(new LanguageItem(AppLanguage.Greek, TextCatalog.Get(currentLanguage, "LanguageGreek")));
            languageComboBox.Items.Add(new LanguageItem(AppLanguage.English, TextCatalog.Get(currentLanguage, "LanguageEnglish")));
            languageComboBox.SelectedIndex = currentLanguage == AppLanguage.Greek ? 0 : 1;
        }

        private void ApplyLanguage()
        {
            titleLabel.Text = TextCatalog.Get(currentLanguage, "HeaderTitle");
            subtitleLabel.Text = TextCatalog.Get(currentLanguage, "HeaderSubtitle");
            darkModeCheckBox.Text = TextCatalog.Get(currentLanguage, "DarkModeToggle");
            languageLabel.Text = TextCatalog.Get(currentLanguage, "LanguageLabel");
            inputTitleLabel.Text = TextCatalog.Get(currentLanguage, "InputTitle");
            inputSubtitleLabel.Text = TextCatalog.Get(currentLanguage, "InputSubtitle");
            resultTitleLabel.Text = TextCatalog.Get(currentLanguage, "ResultTitle");
            hashCaptionLabel.Text = TextCatalog.Get(currentLanguage, "HashCaption");
            fileCaptionLabel.Text = TextCatalog.Get(currentLanguage, "FileCaption");
            outputCaptionLabel.Text = TextCatalog.Get(currentLanguage, "PdfCaption");
            openFolderButton.Text = TextCatalog.Get(currentLanguage, "OpenFolderButton");
            openPdfButton.Text = TextCatalog.Get(currentLanguage, "OpenPdfButton");
            dropSurfacePanel.TitleText = TextCatalog.Get(currentLanguage, "DropTitle");
            dropSurfacePanel.HintText = TextCatalog.Get(currentLanguage, "DropHint");
            suppressOptionEvents = true;
            try
            {
                darkModeCheckBox.Checked = currentTheme == AppTheme.Dark;
                RefreshLanguageOptions();
            }
            finally
            {
                suppressOptionEvents = false;
            }

            ApplyTheme();
            RefreshVisibleState();
        }

        private void ApplyTheme()
        {
            bool dark = currentTheme == AppTheme.Dark;

            Color formBackground = dark ? Color.FromArgb(22, 26, 30) : Color.FromArgb(243, 245, 241);
            Color panelBackground = dark ? Color.FromArgb(33, 39, 45) : Color.White;
            Color headingText = dark ? Color.FromArgb(236, 242, 239) : Color.FromArgb(26, 34, 32);
            Color bodyText = dark ? Color.FromArgb(220, 229, 225) : Color.FromArgb(37, 50, 65);
            Color mutedText = dark ? Color.FromArgb(168, 180, 176) : Color.FromArgb(97, 108, 104);
            Color fieldBackground = dark ? Color.FromArgb(43, 50, 58) : Color.FromArgb(247, 249, 247);
            Color comboBackground = dark ? Color.FromArgb(43, 50, 58) : Color.White;
            Color primaryButton = dark ? Color.FromArgb(40, 151, 117) : Color.FromArgb(24, 115, 90);
            Color secondaryBorder = dark ? Color.FromArgb(73, 83, 92) : Color.FromArgb(220, 226, 222);

            panelBorderColor = secondaryBorder;
            panelTopAccentColor = dark ? Color.FromArgb(48, 118, 99) : Color.FromArgb(231, 241, 237);

            BackColor = formBackground;
            headerPanel.BackColor = formBackground;
            bodyLayout.BackColor = formBackground;
            leftColumn.BackColor = panelBackground;
            rightColumn.BackColor = formBackground;
            resultPanel.BackColor = panelBackground;
            languagePanel.BackColor = panelBackground;
            statusLabel.BackColor = formBackground;

            titleLabel.ForeColor = headingText;
            subtitleLabel.ForeColor = mutedText;
            inputTitleLabel.ForeColor = headingText;
            inputSubtitleLabel.ForeColor = mutedText;
            resultTitleLabel.ForeColor = headingText;
            hashCaptionLabel.ForeColor = mutedText;
            fileCaptionLabel.ForeColor = mutedText;
            outputCaptionLabel.ForeColor = mutedText;
            darkModeCheckBox.ForeColor = mutedText;
            languageLabel.ForeColor = mutedText;
            statusLabel.ForeColor = mutedText;

            hashTextBox.BackColor = fieldBackground;
            hashTextBox.ForeColor = bodyText;
            fileValueLabel.BackColor = fieldBackground;
            fileValueLabel.ForeColor = bodyText;
            outputValueLabel.BackColor = fieldBackground;
            outputValueLabel.ForeColor = bodyText;
            languageComboBox.BackColor = comboBackground;
            languageComboBox.ForeColor = bodyText;

            openFolderButton.BackColor = panelBackground;
            openFolderButton.ForeColor = bodyText;
            openFolderButton.FlatAppearance.BorderColor = secondaryBorder;

            openPdfButton.BackColor = primaryButton;
            openPdfButton.ForeColor = Color.White;

            dropSurfacePanel.UseDarkTheme = dark;

            headerPanel.Invalidate();
            leftColumn.Invalidate();
            languagePanel.Invalidate();
            resultPanel.Invalidate();
        }

        private void RefreshVisibleState()
        {
            if (worker.IsBusy)
            {
                hashTextBox.Text = TextCatalog.Get(currentLanguage, "BusyHashPlaceholder");
                fileValueLabel.Text = string.IsNullOrEmpty(selectedFilePath)
                    ? TextCatalog.Get(currentLanguage, "UnavailableValue")
                    : selectedFilePath;
                outputValueLabel.Text = TextCatalog.Get(currentLanguage, "BusyPdfValue");
                statusLabel.Text = TextCatalog.Get(currentLanguage, "BusyStatus");
                openFolderButton.Enabled = false;
                openPdfButton.Enabled = false;
                return;
            }

            if (lastResult != null)
            {
                hashTextBox.Text = lastResult.HashValue;
                fileValueLabel.Text = lastResult.SourceFilePath;
                outputValueLabel.Text = lastResult.OutputPdfPath;
                statusLabel.Text = TextCatalog.Get(currentLanguage, "ReadyStatus");
                openFolderButton.Enabled = true;
                openPdfButton.Enabled = true;
                return;
            }

            hashTextBox.Text = TextCatalog.Get(currentLanguage, "HashPlaceholder");
            fileValueLabel.Text = string.IsNullOrEmpty(selectedFilePath)
                ? TextCatalog.Get(currentLanguage, "FilePlaceholder")
                : selectedFilePath;
            outputValueLabel.Text = TextCatalog.Get(currentLanguage, "PdfPlaceholder");
            statusLabel.Text = TextCatalog.Get(currentLanguage, "IdleStatus");
            openFolderButton.Enabled = false;
            openPdfButton.Enabled = false;
        }

        private void ResultPanelPaint(object sender, PaintEventArgs e)
        {
            Panel target = sender as Panel;
            if (target == null)
            {
                return;
            }

            Rectangle rectangle = target.ClientRectangle;
            rectangle.Width -= 1;
            rectangle.Height -= 1;
            ControlPaint.DrawBorder(e.Graphics, rectangle, panelBorderColor, ButtonBorderStyle.Solid);
            using (SolidBrush brush = new SolidBrush(panelTopAccentColor))
            {
                e.Graphics.FillRectangle(brush, 0, 0, target.ClientSize.Width, 4);
            }
        }

        private void LanguageComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (suppressOptionEvents)
            {
                return;
            }

            LanguageItem item = languageComboBox.SelectedItem as LanguageItem;
            if (item == null || item.Language == currentLanguage)
            {
                return;
            }

            currentLanguage = item.Language;
            ApplyLanguage();
        }

        private void DarkModeCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (suppressOptionEvents)
            {
                return;
            }

            AppTheme selectedTheme = darkModeCheckBox.Checked ? AppTheme.Dark : AppTheme.Light;
            if (selectedTheme == currentTheme)
            {
                return;
            }

            currentTheme = selectedTheme;
            ApplyTheme();
        }

        private void DropSurfacePanelClick(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.CheckFileExists = true;
                dialog.Multiselect = false;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    BeginProcessing(dialog.FileName);
                }
            }
        }

        private void DropSurfacePanelFileDropped(object sender, FileDroppedEventArgs e)
        {
            BeginProcessing(e.FilePath);
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            string filePath = (string)e.Argument;
            e.Result = HashPdfService.CreateHashProof(filePath);
        }

        private void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                lastResult = null;
                RefreshVisibleState();
                ShowProcessingError(e.Error);
                statusLabel.Text = TextCatalog.Get(currentLanguage, "FailedStatus");
                return;
            }

            lastResult = e.Result as HashPdfResult;
            if (lastResult != null)
            {
                selectedFilePath = lastResult.SourceFilePath;
            }
            RefreshVisibleState();
        }

        private void BeginProcessing(string filePath)
        {
            if (worker.IsBusy)
            {
                ShowError(TextCatalog.Get(currentLanguage, "BusyError"));
                return;
            }

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                ShowError(TextCatalog.Get(currentLanguage, "MissingFileError"));
                return;
            }

            selectedFilePath = filePath;
            lastResult = null;
            worker.RunWorkerAsync(filePath);
            RefreshVisibleState();
        }

        private void OpenFolderButtonClick(object sender, EventArgs e)
        {
            if (lastResult == null || string.IsNullOrEmpty(lastResult.OutputPdfPath))
            {
                return;
            }

            try
            {
                Process.Start("explorer.exe", string.Format("/select,\"{0}\"", lastResult.OutputPdfPath));
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void OpenPdfButtonClick(object sender, EventArgs e)
        {
            if (lastResult == null || string.IsNullOrEmpty(lastResult.OutputPdfPath))
            {
                return;
            }

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(lastResult.OutputPdfPath);
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(
                this,
                message,
                TextCatalog.Get(currentLanguage, "ErrorTitle"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void ShowProcessingError(Exception error)
        {
            HashPdfException hashPdfException = error as HashPdfException;
            if (hashPdfException == null)
            {
                ShowError(TextCatalog.Get(currentLanguage, "GenericProcessingError"));
                return;
            }

            switch (hashPdfException.Code)
            {
                case HashPdfErrorCode.MissingFile:
                    ShowError(TextCatalog.Get(currentLanguage, "MissingFileError"));
                    break;
                case HashPdfErrorCode.SourceDirectoryUnavailable:
                    ShowError(TextCatalog.Get(currentLanguage, "MissingDirectoryError"));
                    break;
                case HashPdfErrorCode.CannotWritePdf:
                    ShowError(TextCatalog.Get(currentLanguage, "WritePdfError"));
                    break;
                default:
                    ShowError(TextCatalog.Get(currentLanguage, "GenericProcessingError"));
                    break;
            }
        }

        private sealed class LanguageItem
        {
            public LanguageItem(AppLanguage language, string label)
            {
                Language = language;
                Label = label;
            }

            public AppLanguage Language { get; private set; }

            public string Label { get; private set; }

            public override string ToString()
            {
                return Label;
            }
        }

        private enum AppTheme
        {
            Light,
            Dark
        }
    }
}
