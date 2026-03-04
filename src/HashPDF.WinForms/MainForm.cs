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
        private AppLanguage currentLanguage;
        private string selectedFilePath;
        private HashPdfResult lastResult;

        public MainForm()
        {
            InitializeComponent();

            worker = new BackgroundWorker();
            worker.DoWork += WorkerDoWork;
            worker.RunWorkerCompleted += WorkerRunWorkerCompleted;

            currentLanguage = AppLanguage.Greek;
            RefreshLanguageOptions();
            ApplyLanguage();
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
            RefreshLanguageOptions();
            RefreshVisibleState();
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
            ControlPaint.DrawBorder(e.Graphics, rectangle, Color.FromArgb(220, 226, 222), ButtonBorderStyle.Solid);
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(231, 241, 237)))
            {
                e.Graphics.FillRectangle(brush, 0, 0, target.ClientSize.Width, 4);
            }
        }

        private void LanguageComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            LanguageItem item = languageComboBox.SelectedItem as LanguageItem;
            if (item == null || item.Language == currentLanguage)
            {
                return;
            }

            currentLanguage = item.Language;
            ApplyLanguage();
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
    }
}
