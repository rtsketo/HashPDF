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
    public class MainForm : Form
    {
        private readonly BackgroundWorker worker;
        private readonly Label titleLabel;
        private readonly Label subtitleLabel;
        private readonly Label languageLabel;
        private readonly ComboBox languageComboBox;
        private readonly DropSurfacePanel dropSurfacePanel;
        private readonly Button browseButton;
        private readonly Panel resultPanel;
        private readonly Label resultTitleLabel;
        private readonly TextBox hashTextBox;
        private readonly Label fileValueLabel;
        private readonly Label outputValueLabel;
        private readonly Button openFolderButton;
        private readonly Button openPdfButton;
        private readonly Label statusLabel;

        private AppLanguage currentLanguage;
        private string selectedFilePath;
        private HashPdfResult lastResult;

        public MainForm()
        {
            Text = "HashPDF";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(960, 640);
            ClientSize = new Size(1100, 720);
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 161);

            worker = new BackgroundWorker();
            worker.DoWork += WorkerDoWork;
            worker.RunWorkerCompleted += WorkerRunWorkerCompleted;

            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 110;
            headerPanel.Padding = new Padding(28, 24, 28, 8);
            Controls.Add(headerPanel);

            titleLabel = new Label();
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold, GraphicsUnit.Point, 161);
            titleLabel.ForeColor = Color.FromArgb(20, 31, 48);
            headerPanel.Controls.Add(titleLabel);

            subtitleLabel = new Label();
            subtitleLabel.AutoSize = true;
            subtitleLabel.Top = 52;
            subtitleLabel.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point, 161);
            subtitleLabel.ForeColor = Color.FromArgb(88, 101, 119);
            headerPanel.Controls.Add(subtitleLabel);

            Panel languagePanel = new Panel();
            languagePanel.Dock = DockStyle.Right;
            languagePanel.Width = 220;
            headerPanel.Controls.Add(languagePanel);

            languageLabel = new Label();
            languageLabel.AutoSize = true;
            languageLabel.Left = 16;
            languageLabel.Top = 12;
            languageLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 161);
            languageLabel.ForeColor = Color.FromArgb(63, 77, 97);
            languagePanel.Controls.Add(languageLabel);

            languageComboBox = new ComboBox();
            languageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            languageComboBox.FlatStyle = FlatStyle.Flat;
            languageComboBox.Left = 16;
            languageComboBox.Top = 36;
            languageComboBox.Width = 180;
            languageComboBox.SelectedIndexChanged += LanguageComboBoxSelectedIndexChanged;
            languagePanel.Controls.Add(languageComboBox);

            TableLayoutPanel bodyLayout = new TableLayoutPanel();
            bodyLayout.Dock = DockStyle.Fill;
            bodyLayout.Padding = new Padding(28, 4, 28, 20);
            bodyLayout.ColumnCount = 2;
            bodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 57F));
            bodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43F));
            Controls.Add(bodyLayout);

            Panel leftColumn = new Panel();
            leftColumn.Dock = DockStyle.Fill;
            leftColumn.Margin = new Padding(0, 0, 18, 0);
            bodyLayout.Controls.Add(leftColumn, 0, 0);

            dropSurfacePanel = new DropSurfacePanel();
            dropSurfacePanel.Dock = DockStyle.Top;
            dropSurfacePanel.Height = 420;
            dropSurfacePanel.FileDropped += DropSurfacePanelFileDropped;
            leftColumn.Controls.Add(dropSurfacePanel);

            browseButton = CreatePrimaryButton();
            browseButton.Top = 440;
            browseButton.Left = 0;
            browseButton.Width = 190;
            browseButton.Click += BrowseButtonClick;
            leftColumn.Controls.Add(browseButton);

            Panel rightColumn = new Panel();
            rightColumn.Dock = DockStyle.Fill;
            rightColumn.Margin = new Padding(18, 0, 0, 0);
            bodyLayout.Controls.Add(rightColumn, 1, 0);

            resultPanel = new Panel();
            resultPanel.Dock = DockStyle.Fill;
            resultPanel.BackColor = Color.White;
            resultPanel.Padding = new Padding(24, 24, 24, 24);
            resultPanel.Paint += ResultPanelPaint;
            rightColumn.Controls.Add(resultPanel);

            resultTitleLabel = new Label();
            resultTitleLabel.AutoSize = true;
            resultTitleLabel.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point, 161);
            resultTitleLabel.ForeColor = Color.FromArgb(20, 31, 48);
            resultPanel.Controls.Add(resultTitleLabel);

            hashTextBox = new TextBox();
            hashTextBox.Multiline = true;
            hashTextBox.ReadOnly = true;
            hashTextBox.ScrollBars = ScrollBars.Vertical;
            hashTextBox.BorderStyle = BorderStyle.FixedSingle;
            hashTextBox.Font = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point, 161);
            hashTextBox.BackColor = Color.FromArgb(248, 250, 252);
            hashTextBox.ForeColor = Color.FromArgb(37, 50, 65);
            hashTextBox.Left = 0;
            hashTextBox.Top = 42;
            hashTextBox.Width = 380;
            hashTextBox.Height = 220;
            hashTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            resultPanel.Controls.Add(hashTextBox);

            fileValueLabel = CreateMetaLabel(282);
            resultPanel.Controls.Add(fileValueLabel);

            outputValueLabel = CreateMetaLabel(332);
            resultPanel.Controls.Add(outputValueLabel);

            openFolderButton = CreateSecondaryButton();
            openFolderButton.Top = 396;
            openFolderButton.Left = 0;
            openFolderButton.Width = 170;
            openFolderButton.Enabled = false;
            openFolderButton.Click += OpenFolderButtonClick;
            resultPanel.Controls.Add(openFolderButton);

            openPdfButton = CreatePrimaryButton();
            openPdfButton.Top = 396;
            openPdfButton.Left = 186;
            openPdfButton.Width = 170;
            openPdfButton.Enabled = false;
            openPdfButton.Click += OpenPdfButtonClick;
            resultPanel.Controls.Add(openPdfButton);

            statusLabel = new Label();
            statusLabel.Dock = DockStyle.Bottom;
            statusLabel.Height = 34;
            statusLabel.Padding = new Padding(28, 0, 28, 10);
            statusLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 161);
            statusLabel.ForeColor = Color.FromArgb(88, 101, 119);
            Controls.Add(statusLabel);

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
            button.BackColor = Color.FromArgb(20, 137, 125);
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 161);
            button.Cursor = Cursors.Hand;
            return button;
        }

        private static Button CreateSecondaryButton()
        {
            Button button = CreatePrimaryButton();
            button.BackColor = Color.FromArgb(226, 232, 240);
            button.ForeColor = Color.FromArgb(37, 50, 65);
            return button;
        }

        private static Label CreateMetaLabel(int top)
        {
            Label label = new Label();
            label.AutoEllipsis = true;
            label.Left = 0;
            label.Top = top;
            label.Width = 380;
            label.Height = 36;
            label.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 161);
            label.ForeColor = Color.FromArgb(74, 85, 104);
            label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
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
            browseButton.Text = TextCatalog.Get(currentLanguage, "BrowseButton");
            resultTitleLabel.Text = TextCatalog.Get(currentLanguage, "ResultTitle");
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
                fileValueLabel.Text = string.Format(
                    "{0} {1}",
                    TextCatalog.Get(currentLanguage, "FileLabel"),
                    string.IsNullOrEmpty(selectedFilePath) ? TextCatalog.Get(currentLanguage, "UnavailableValue") : selectedFilePath);
                outputValueLabel.Text = string.Format(
                    "{0} {1}",
                    TextCatalog.Get(currentLanguage, "PdfLabel"),
                    TextCatalog.Get(currentLanguage, "BusyPdfValue"));
                statusLabel.Text = TextCatalog.Get(currentLanguage, "BusyStatus");
                openFolderButton.Enabled = false;
                openPdfButton.Enabled = false;
                return;
            }

            if (lastResult != null)
            {
                hashTextBox.Text = lastResult.HashValue;
                fileValueLabel.Text = string.Format("{0} {1}", TextCatalog.Get(currentLanguage, "FileLabel"), lastResult.SourceFilePath);
                outputValueLabel.Text = string.Format("{0} {1}", TextCatalog.Get(currentLanguage, "PdfLabel"), lastResult.OutputPdfPath);
                statusLabel.Text = TextCatalog.Get(currentLanguage, "ReadyStatus");
                openFolderButton.Enabled = true;
                openPdfButton.Enabled = true;
                return;
            }

            hashTextBox.Text = TextCatalog.Get(currentLanguage, "HashPlaceholder");
            fileValueLabel.Text = string.IsNullOrEmpty(selectedFilePath)
                ? TextCatalog.Get(currentLanguage, "FilePlaceholder")
                : string.Format("{0} {1}", TextCatalog.Get(currentLanguage, "FileLabel"), selectedFilePath);
            outputValueLabel.Text = TextCatalog.Get(currentLanguage, "PdfPlaceholder");
            statusLabel.Text = TextCatalog.Get(currentLanguage, "IdleStatus");
            openFolderButton.Enabled = false;
            openPdfButton.Enabled = false;
        }

        private void ResultPanelPaint(object sender, PaintEventArgs e)
        {
            Rectangle rectangle = resultPanel.ClientRectangle;
            rectangle.Width -= 1;
            rectangle.Height -= 1;
            ControlPaint.DrawBorder(e.Graphics, rectangle, Color.FromArgb(224, 229, 236), ButtonBorderStyle.Solid);
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

        private void BrowseButtonClick(object sender, EventArgs e)
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
