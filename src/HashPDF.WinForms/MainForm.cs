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
        private readonly Label inputTitleLabel;
        private readonly Label inputSubtitleLabel;
        private readonly DropSurfacePanel dropSurfacePanel;
        private readonly Panel resultPanel;
        private readonly Label resultTitleLabel;
        private readonly Label hashCaptionLabel;
        private readonly Label fileCaptionLabel;
        private readonly Label outputCaptionLabel;
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
            MinimumSize = new Size(1040, 680);
            ClientSize = new Size(1180, 760);
            BackColor = Color.FromArgb(243, 245, 241);
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 161);

            worker = new BackgroundWorker();
            worker.DoWork += WorkerDoWork;
            worker.RunWorkerCompleted += WorkerRunWorkerCompleted;

            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 128;
            headerPanel.Padding = new Padding(32, 28, 32, 12);
            Controls.Add(headerPanel);

            titleLabel = new Label();
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI Semibold", 28F, FontStyle.Bold, GraphicsUnit.Point, 161);
            titleLabel.ForeColor = Color.FromArgb(26, 34, 32);
            headerPanel.Controls.Add(titleLabel);

            subtitleLabel = new Label();
            subtitleLabel.AutoSize = false;
            subtitleLabel.Left = 2;
            subtitleLabel.Top = 60;
            subtitleLabel.Width = 660;
            subtitleLabel.Height = 46;
            subtitleLabel.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 161);
            subtitleLabel.ForeColor = Color.FromArgb(97, 108, 104);
            headerPanel.Controls.Add(subtitleLabel);

            Panel languagePanel = new Panel();
            languagePanel.Dock = DockStyle.Right;
            languagePanel.Width = 214;
            languagePanel.BackColor = Color.White;
            languagePanel.Paint += ResultPanelPaint;
            headerPanel.Controls.Add(languagePanel);

            languageLabel = new Label();
            languageLabel.AutoSize = true;
            languageLabel.Left = 18;
            languageLabel.Top = 16;
            languageLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 161);
            languageLabel.ForeColor = Color.FromArgb(97, 108, 104);
            languagePanel.Controls.Add(languageLabel);

            languageComboBox = new ComboBox();
            languageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            languageComboBox.FlatStyle = FlatStyle.Flat;
            languageComboBox.Left = 18;
            languageComboBox.Top = 42;
            languageComboBox.Width = 176;
            languageComboBox.Height = 32;
            languageComboBox.SelectedIndexChanged += LanguageComboBoxSelectedIndexChanged;
            languagePanel.Controls.Add(languageComboBox);

            TableLayoutPanel bodyLayout = new TableLayoutPanel();
            bodyLayout.Dock = DockStyle.Fill;
            bodyLayout.Padding = new Padding(32, 0, 32, 20);
            bodyLayout.ColumnCount = 2;
            bodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
            bodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
            Controls.Add(bodyLayout);

            Panel leftColumn = new Panel();
            leftColumn.Dock = DockStyle.Fill;
            leftColumn.Margin = new Padding(0, 0, 14, 0);
            leftColumn.Padding = new Padding(24, 24, 24, 24);
            leftColumn.BackColor = Color.White;
            leftColumn.Paint += ResultPanelPaint;
            bodyLayout.Controls.Add(leftColumn, 0, 0);

            inputTitleLabel = new Label();
            inputTitleLabel.AutoSize = true;
            inputTitleLabel.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold, GraphicsUnit.Point, 161);
            inputTitleLabel.ForeColor = Color.FromArgb(26, 34, 32);
            inputTitleLabel.Left = 24;
            inputTitleLabel.Top = 24;
            leftColumn.Controls.Add(inputTitleLabel);

            inputSubtitleLabel = new Label();
            inputSubtitleLabel.Left = 24;
            inputSubtitleLabel.Top = 58;
            inputSubtitleLabel.Width = 470;
            inputSubtitleLabel.Height = 42;
            inputSubtitleLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 161);
            inputSubtitleLabel.ForeColor = Color.FromArgb(97, 108, 104);
            leftColumn.Controls.Add(inputSubtitleLabel);

            dropSurfacePanel = new DropSurfacePanel();
            dropSurfacePanel.Dock = DockStyle.None;
            dropSurfacePanel.Left = 24;
            dropSurfacePanel.Top = 118;
            dropSurfacePanel.Width = 470;
            dropSurfacePanel.Height = 420;
            dropSurfacePanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dropSurfacePanel.FileDropped += DropSurfacePanelFileDropped;
            dropSurfacePanel.Click += DropSurfacePanelClick;
            leftColumn.Controls.Add(dropSurfacePanel);

            Panel rightColumn = new Panel();
            rightColumn.Dock = DockStyle.Fill;
            rightColumn.Margin = new Padding(14, 0, 0, 0);
            bodyLayout.Controls.Add(rightColumn, 1, 0);

            resultPanel = new Panel();
            resultPanel.Dock = DockStyle.Fill;
            resultPanel.BackColor = Color.White;
            resultPanel.Padding = new Padding(24, 24, 24, 24);
            resultPanel.Paint += ResultPanelPaint;
            rightColumn.Controls.Add(resultPanel);

            resultTitleLabel = new Label();
            resultTitleLabel.AutoSize = true;
            resultTitleLabel.Left = 24;
            resultTitleLabel.Top = 24;
            resultTitleLabel.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold, GraphicsUnit.Point, 161);
            resultTitleLabel.ForeColor = Color.FromArgb(26, 34, 32);
            resultPanel.Controls.Add(resultTitleLabel);

            hashCaptionLabel = new Label();
            hashCaptionLabel.AutoSize = true;
            hashCaptionLabel.Left = 24;
            hashCaptionLabel.Top = 62;
            hashCaptionLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 161);
            hashCaptionLabel.ForeColor = Color.FromArgb(97, 108, 104);
            resultPanel.Controls.Add(hashCaptionLabel);

            hashTextBox = new TextBox();
            hashTextBox.Multiline = true;
            hashTextBox.ReadOnly = true;
            hashTextBox.WordWrap = false;
            hashTextBox.ScrollBars = ScrollBars.Horizontal;
            hashTextBox.BorderStyle = BorderStyle.FixedSingle;
            hashTextBox.Font = new Font("Consolas", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 161);
            hashTextBox.BackColor = Color.FromArgb(247, 249, 247);
            hashTextBox.ForeColor = Color.FromArgb(37, 50, 65);
            hashTextBox.Left = 24;
            hashTextBox.Top = 86;
            hashTextBox.Width = 416;
            hashTextBox.Height = 116;
            hashTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            resultPanel.Controls.Add(hashTextBox);

            fileCaptionLabel = CreateSectionLabel(224);
            fileCaptionLabel.Left = 24;
            resultPanel.Controls.Add(fileCaptionLabel);

            fileValueLabel = CreateMetaLabel(246);
            fileValueLabel.Left = 24;
            resultPanel.Controls.Add(fileValueLabel);

            outputCaptionLabel = CreateSectionLabel(330);
            outputCaptionLabel.Left = 24;
            resultPanel.Controls.Add(outputCaptionLabel);

            outputValueLabel = CreateMetaLabel(352);
            outputValueLabel.Left = 24;
            resultPanel.Controls.Add(outputValueLabel);

            openFolderButton = CreateSecondaryButton();
            openFolderButton.Top = 426;
            openFolderButton.Left = 24;
            openFolderButton.Width = 168;
            openFolderButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            openFolderButton.Enabled = false;
            openFolderButton.Click += OpenFolderButtonClick;
            resultPanel.Controls.Add(openFolderButton);

            openPdfButton = CreatePrimaryButton();
            openPdfButton.Top = 426;
            openPdfButton.Left = 208;
            openPdfButton.Width = 168;
            openPdfButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            openPdfButton.Enabled = false;
            openPdfButton.Click += OpenPdfButtonClick;
            resultPanel.Controls.Add(openPdfButton);

            statusLabel = new Label();
            statusLabel.Dock = DockStyle.Bottom;
            statusLabel.Height = 44;
            statusLabel.Padding = new Padding(32, 0, 32, 12);
            statusLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 161);
            statusLabel.ForeColor = Color.FromArgb(97, 108, 104);
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
