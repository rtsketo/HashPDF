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
        private readonly BackgroundWorker dxfWorker;
        private readonly BackgroundWorker updateWorker;
        private static readonly string PreferencesDirectoryPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "HashPDF");
        private static readonly string PreferencesFilePath = Path.Combine(PreferencesDirectoryPath, "ui-preferences.ini");
        private const int LeftColumnContentLeft = 27;
        private const int LeftColumnContentRight = 27;
        private const int DropSurfaceTop = 118;
        private const int LeftColumnBottomMargin = 24;
        private const int DxfToolsGapFromDrop = 24;
        private const int DxfToolsDescriptionGap = 5;
        private const int DxfButtonsGap = 12;
        private const int DxfButtonHeight = 44;
        private const int DxfButtonGap = 16;
        private DropSurfacePanel dropSurfacePanel;
        private AppLanguage currentLanguage;
        private AppTheme currentTheme;
        private string selectedFilePath;
        private HashPdfResult lastResult;
        private bool suppressOptionEvents;
        private bool updateCheckStarted;
        private Color panelBorderColor;
        private Color panelTopAccentColor;

        public MainForm()
        {
            InitializeComponent();
            ApplyWindowIcon();
            InitializeDropSurfacePanel();

            leftColumn.Resize += LeftColumnResize;

            worker = new BackgroundWorker();
            worker.DoWork += WorkerDoWork;
            worker.RunWorkerCompleted += WorkerRunWorkerCompleted;

            dxfWorker = new BackgroundWorker();
            dxfWorker.DoWork += DxfWorkerDoWork;
            dxfWorker.RunWorkerCompleted += DxfWorkerRunWorkerCompleted;

            updateWorker = new BackgroundWorker();
            updateWorker.DoWork += UpdateWorkerDoWork;
            updateWorker.RunWorkerCompleted += UpdateWorkerRunWorkerCompleted;

            SetDxfLayoutImmediate(false);

            LoadUserPreferences();
            ApplyLanguage();
            ApplyTheme();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            BeginStartupUpdateCheck();
        }

        private void ApplyWindowIcon()
        {
            try
            {
                Icon applicationIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                if (applicationIcon != null)
                {
                    Icon = applicationIcon;
                }
            }
            catch
            {
                // Ignore icon assignment failures and keep default icon.
            }
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

        private void RefreshThemeOptions()
        {
            themeComboBox.Items.Clear();
            themeComboBox.Items.Add(new ThemeItem(AppTheme.Light, TextCatalog.Get(currentLanguage, "ThemeLight")));
            themeComboBox.Items.Add(new ThemeItem(AppTheme.Dark, TextCatalog.Get(currentLanguage, "ThemeDark")));
            themeComboBox.SelectedIndex = currentTheme == AppTheme.Light ? 0 : 1;
        }

        private void ApplyLanguage()
        {
            titleLabel.Text = TextCatalog.Get(currentLanguage, "HeaderTitle");
            subtitleLabel.Text = TextCatalog.Get(currentLanguage, "HeaderSubtitle");
            themeLabel.Text = TextCatalog.Get(currentLanguage, "ThemeLabel");
            languageLabel.Text = TextCatalog.Get(currentLanguage, "LanguageLabel");
            inputTitleLabel.Text = TextCatalog.Get(currentLanguage, "InputTitle");
            inputSubtitleLabel.Text = TextCatalog.Get(currentLanguage, "InputSubtitle");
            dxfToolsTitleLabel.Text = TextCatalog.Get(currentLanguage, "DxfToolsTitle");
            dxfToolsDescriptionLabel.Text = TextCatalog.Get(currentLanguage, "DxfToolsDescription");
            resultTitleLabel.Text = TextCatalog.Get(currentLanguage, "ResultTitle");
            hashCaptionLabel.Text = TextCatalog.Get(currentLanguage, "HashCaption");
            fileCaptionLabel.Text = TextCatalog.Get(currentLanguage, "FileCaption");
            outputCaptionLabel.Text = TextCatalog.Get(currentLanguage, "PdfCaption");
            openFolderButton.Text = TextCatalog.Get(currentLanguage, "OpenFolderButton");
            openPdfButton.Text = TextCatalog.Get(currentLanguage, "OpenPdfButton");
            convertDxfKaekButton.Text = TextCatalog.Get(currentLanguage, "DxfConvertKaekButton");
            convertDxfAllButton.Text = TextCatalog.Get(currentLanguage, "DxfConvertAllButton");
            suppressOptionEvents = true;
            try
            {
                RefreshThemeOptions();
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
            dxfToolsTitleLabel.ForeColor = headingText;
            dxfToolsDescriptionLabel.ForeColor = mutedText;
            resultTitleLabel.ForeColor = headingText;
            hashCaptionLabel.ForeColor = mutedText;
            fileCaptionLabel.ForeColor = mutedText;
            outputCaptionLabel.ForeColor = mutedText;
            themeLabel.ForeColor = mutedText;
            languageLabel.ForeColor = mutedText;
            statusLabel.ForeColor = mutedText;

            hashTextBox.BackColor = fieldBackground;
            hashTextBox.ForeColor = bodyText;
            fileValueLabel.BackColor = fieldBackground;
            fileValueLabel.ForeColor = bodyText;
            outputValueLabel.BackColor = fieldBackground;
            outputValueLabel.ForeColor = bodyText;
            themeComboBox.BackColor = comboBackground;
            themeComboBox.ForeColor = bodyText;
            languageComboBox.BackColor = comboBackground;
            languageComboBox.ForeColor = bodyText;

            openFolderButton.BackColor = panelBackground;
            openFolderButton.ForeColor = dark ? Color.White : bodyText;
            openFolderButton.FlatAppearance.BorderColor = secondaryBorder;

            ApplyDxfButtonTheme();

            openPdfButton.BackColor = primaryButton;
            openPdfButton.ForeColor = Color.White;

            dropSurfacePanel.UseDarkTheme = dark;

            headerPanel.Invalidate();
            leftColumn.Invalidate();
            languagePanel.Invalidate();
            resultPanel.Invalidate();
        }

        private void LoadUserPreferences()
        {
            currentLanguage = AppLanguage.Greek;
            currentTheme = AppTheme.Dark;

            try
            {
                if (!File.Exists(PreferencesFilePath))
                {
                    return;
                }

                string[] lines = File.ReadAllLines(PreferencesFilePath);
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    string trimmedLine = line.Trim();
                    if (trimmedLine.StartsWith("#"))
                    {
                        continue;
                    }

                    int separatorIndex = trimmedLine.IndexOf('=');
                    if (separatorIndex <= 0 || separatorIndex >= trimmedLine.Length - 1)
                    {
                        continue;
                    }

                    string key = trimmedLine.Substring(0, separatorIndex).Trim();
                    string value = trimmedLine.Substring(separatorIndex + 1).Trim();
                    if (key.Equals("language", StringComparison.OrdinalIgnoreCase))
                    {
                        AppLanguage parsedLanguage;
                        if (TryParseLanguage(value, out parsedLanguage))
                        {
                            currentLanguage = parsedLanguage;
                        }
                    }
                    else if (key.Equals("theme", StringComparison.OrdinalIgnoreCase))
                    {
                        AppTheme parsedTheme;
                        if (TryParseTheme(value, out parsedTheme))
                        {
                            currentTheme = parsedTheme;
                        }
                    }
                }
            }
            catch
            {
                currentLanguage = AppLanguage.Greek;
                currentTheme = AppTheme.Dark;
            }
        }

        private void SaveUserPreferences()
        {
            try
            {
                Directory.CreateDirectory(PreferencesDirectoryPath);
                string content = string.Format(
                    "language={0}{2}theme={1}{2}",
                    SerializeLanguage(currentLanguage),
                    SerializeTheme(currentTheme),
                    Environment.NewLine);
                File.WriteAllText(PreferencesFilePath, content);
            }
            catch
            {
                // Ignore preference write failures.
            }
        }

        private static bool TryParseLanguage(string value, out AppLanguage language)
        {
            if (value.Equals("greek", StringComparison.OrdinalIgnoreCase))
            {
                language = AppLanguage.Greek;
                return true;
            }

            if (value.Equals("english", StringComparison.OrdinalIgnoreCase))
            {
                language = AppLanguage.English;
                return true;
            }

            language = AppLanguage.Greek;
            return false;
        }

        private static bool TryParseTheme(string value, out AppTheme theme)
        {
            if (value.Equals("dark", StringComparison.OrdinalIgnoreCase))
            {
                theme = AppTheme.Dark;
                return true;
            }

            if (value.Equals("light", StringComparison.OrdinalIgnoreCase))
            {
                theme = AppTheme.Light;
                return true;
            }

            theme = AppTheme.Dark;
            return false;
        }

        private static string SerializeLanguage(AppLanguage language)
        {
            return language == AppLanguage.English ? "english" : "greek";
        }

        private static string SerializeTheme(AppTheme theme)
        {
            return theme == AppTheme.Light ? "light" : "dark";
        }

        private void RefreshVisibleState()
        {
            RefreshDropSurfaceText();
            RefreshDxfLayout(true);

            if (worker.IsBusy)
            {
                hashTextBox.Text = TextCatalog.Get(currentLanguage, "BusyHashPlaceholder");
                fileValueLabel.Text = string.IsNullOrEmpty(selectedFilePath)
                    ? TextCatalog.Get(currentLanguage, "UnavailableValue")
                    : selectedFilePath;
                outputValueLabel.Text = TextCatalog.Get(currentLanguage, "BusyPdfValue");
                statusLabel.Text = TextCatalog.Get(currentLanguage, "BusyStatus");
                UpdateActionButtons(false);
                return;
            }

            if (dxfWorker.IsBusy)
            {
                if (lastResult != null)
                {
                    hashTextBox.Text = lastResult.HashValue;
                    fileValueLabel.Text = lastResult.SourceFilePath;
                    outputValueLabel.Text = lastResult.OutputPdfPath;
                    UpdateActionButtons(true);
                }
                else
                {
                    hashTextBox.Text = TextCatalog.Get(currentLanguage, "HashPlaceholder");
                    fileValueLabel.Text = string.IsNullOrEmpty(selectedFilePath)
                        ? TextCatalog.Get(currentLanguage, "FilePlaceholder")
                        : selectedFilePath;
                    outputValueLabel.Text = TextCatalog.Get(currentLanguage, "PdfPlaceholder");
                    UpdateActionButtons(false);
                }

                statusLabel.Text = TextCatalog.Get(currentLanguage, "DxfBusyStatus");
                return;
            }

            if (lastResult != null)
            {
                hashTextBox.Text = lastResult.HashValue;
                fileValueLabel.Text = lastResult.SourceFilePath;
                outputValueLabel.Text = lastResult.OutputPdfPath;
                statusLabel.Text = TextCatalog.Get(currentLanguage, "ReadyStatus");
                UpdateActionButtons(true);
                return;
            }

            hashTextBox.Text = TextCatalog.Get(currentLanguage, "HashPlaceholder");
            fileValueLabel.Text = string.IsNullOrEmpty(selectedFilePath)
                ? TextCatalog.Get(currentLanguage, "FilePlaceholder")
                : selectedFilePath;
            outputValueLabel.Text = TextCatalog.Get(currentLanguage, "PdfPlaceholder");
            statusLabel.Text = TextCatalog.Get(currentLanguage, "IdleStatus");
            UpdateActionButtons(false);
        }

        private void BeginStartupUpdateCheck()
        {
            if (updateCheckStarted || updateWorker.IsBusy)
            {
                return;
            }

            updateCheckStarted = true;
            updateWorker.RunWorkerAsync();
        }

        private void UpdateWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = UpdateCheckService.CheckForUpdate();
        }

        private void UpdateWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null || IsDisposed)
            {
                return;
            }

            UpdateInfo updateInfo = e.Result as UpdateInfo;
            if (updateInfo == null)
            {
                return;
            }

            bool shouldInstall = ShowUpdateAvailableDialog(updateInfo);
            if (!shouldInstall)
            {
                return;
            }

            try
            {
                UpdateLauncher.Launch(updateInfo, currentLanguage);
                BeginInvoke(new MethodInvoker(delegate
                {
                    Application.Exit();
                }));
            }
            catch (Exception ex)
            {
                ShowError(string.Format(TextCatalog.Get(currentLanguage, "UpdateLaunchError"), ex.Message));
            }
        }

        private bool ShowUpdateAvailableDialog(UpdateInfo updateInfo)
        {
            bool install = false;
            using (Form dialog = CreateModalDialog(TextCatalog.Get(currentLanguage, "UpdateAvailableTitle"), 500, 160))
            {
                Label messageLabel = CreateDialogLabel(
                    string.Format(TextCatalog.Get(currentLanguage, "UpdateAvailableMessage"), updateInfo.Version),
                    24,
                    16,
                    452,
                    58);
                dialog.Controls.Add(messageLabel);

                Button laterButton = CreateSecondaryButton();
                laterButton.Text = TextCatalog.Get(currentLanguage, "UpdateLaterButton");
                laterButton.Location = new Point(194, 96);
                laterButton.Size = new Size(138, 42);
                laterButton.DialogResult = DialogResult.Cancel;
                ApplyDialogButtonTheme(laterButton, false);
                dialog.Controls.Add(laterButton);
                dialog.CancelButton = laterButton;

                Button installButton = CreatePrimaryButton();
                installButton.Text = TextCatalog.Get(currentLanguage, "UpdateInstallButton");
                installButton.Location = new Point(342, 96);
                installButton.Size = new Size(138, 42);
                installButton.Click += delegate
                {
                    install = true;
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
                };
                ApplyDialogButtonTheme(installButton, true);
                dialog.Controls.Add(installButton);
                dialog.AcceptButton = installButton;

                dialog.ShowDialog(this);
            }

            return install;
        }

        private void UpdateActionButtons(bool canOpenFiles)
        {
            bool keepWhiteTextInDarkMode = currentTheme == AppTheme.Dark;
            bool enableButtons = canOpenFiles || keepWhiteTextInDarkMode;
            openFolderButton.Enabled = enableButtons;
            openPdfButton.Enabled = enableButtons;
            convertDxfKaekButton.Enabled = true;
            convertDxfAllButton.Enabled = true;
            ApplyDxfButtonTheme();
        }

        private bool CanConvertLoadedDxf()
        {
            return !worker.IsBusy
                && !dxfWorker.IsBusy
                && IsLoadedDxfFile();
        }

        private bool IsLoadedDxfFile()
        {
            return !string.IsNullOrEmpty(selectedFilePath)
                && File.Exists(selectedFilePath)
                && Path.GetExtension(selectedFilePath).Equals(".dxf", StringComparison.OrdinalIgnoreCase);
        }

        private void ApplyDxfButtonTheme()
        {
            bool dark = currentTheme == AppTheme.Dark;
            Color textColor = dark ? Color.White : Color.FromArgb(37, 50, 65);
            Color backColor = dark ? Color.FromArgb(33, 39, 45) : Color.White;
            Color borderColor = dark ? Color.FromArgb(73, 83, 92) : Color.FromArgb(220, 226, 222);
            Cursor cursor = CanConvertLoadedDxf() ? Cursors.Hand : Cursors.Default;

            ApplyDxfButtonTheme(convertDxfKaekButton, textColor, backColor, borderColor, cursor);
            ApplyDxfButtonTheme(convertDxfAllButton, textColor, backColor, borderColor, cursor);
        }

        private static void ApplyDxfButtonTheme(
            Button button,
            Color textColor,
            Color backColor,
            Color borderColor,
            Cursor cursor)
        {
            button.ForeColor = textColor;
            button.BackColor = backColor;
            button.FlatAppearance.BorderColor = borderColor;
            button.Cursor = cursor;
        }

        private void RefreshDxfLayout(bool animate)
        {
            bool shouldShowTools = IsLoadedDxfFile();
            SetDxfLayoutImmediate(shouldShowTools);
        }

        private void SetDxfLayoutImmediate(bool showTools)
        {
            ApplyDxfToolsDimensions();
            dropSurfaceHostPanel.Bounds = GetDropSurfaceBounds(showTools);

            if (showTools)
            {
                dxfToolsTitleLabel.Location = GetDxfToolsTitleLocation();
                dxfToolsDescriptionLabel.Location = GetDxfToolsDescriptionLocation();
                convertDxfKaekButton.Location = GetDxfKaekButtonLocation();
                convertDxfAllButton.Location = GetDxfAllButtonLocation();
                SetDxfToolsVisible(true);
            }
            else
            {
                dxfToolsTitleLabel.Location = GetHiddenDxfToolsLocation(dxfToolsTitleLabel.Left);
                dxfToolsDescriptionLabel.Location = GetHiddenDxfToolsLocation(dxfToolsDescriptionLabel.Left);
                convertDxfKaekButton.Location = GetHiddenDxfToolsLocation(convertDxfKaekButton.Left);
                convertDxfAllButton.Location = GetHiddenDxfToolsLocation(convertDxfAllButton.Left);
                SetDxfToolsVisible(false);
            }
        }

        private void SetDxfToolsVisible(bool visible)
        {
            dxfToolsTitleLabel.Visible = visible;
            dxfToolsDescriptionLabel.Visible = visible;
            convertDxfKaekButton.Visible = visible;
            convertDxfAllButton.Visible = visible;
        }

        private void ApplyDxfToolsDimensions()
        {
            int contentWidth = GetLeftColumnContentWidth();
            int buttonWidth = (contentWidth - DxfButtonGap) / 2;
            dxfToolsDescriptionLabel.Width = contentWidth;
            convertDxfKaekButton.Size = new Size(buttonWidth, DxfButtonHeight);
            convertDxfAllButton.Size = new Size(buttonWidth, DxfButtonHeight);
        }

        private Rectangle GetDropSurfaceBounds(bool showTools)
        {
            int contentWidth = GetLeftColumnContentWidth();
            int height = showTools ? GetCompactDropSurfaceHeight() : GetExpandedDropSurfaceHeight();
            return new Rectangle(LeftColumnContentLeft, DropSurfaceTop, contentWidth, height);
        }

        private int GetLeftColumnContentWidth()
        {
            return Math.Max(220, leftColumn.ClientSize.Width - LeftColumnContentLeft - LeftColumnContentRight);
        }

        private int GetExpandedDropSurfaceHeight()
        {
            return Math.Max(240, leftColumn.ClientSize.Height - DropSurfaceTop - LeftColumnBottomMargin);
        }

        private int GetCompactDropSurfaceHeight()
        {
            return Math.Max(180, GetDxfToolsTitleLocation().Y - DxfToolsGapFromDrop - DropSurfaceTop);
        }

        private Point GetDxfToolsTitleLocation()
        {
            return new Point(
                LeftColumnContentLeft,
                GetDxfToolsDescriptionLocation().Y - dxfToolsTitleLabel.Height - DxfToolsDescriptionGap);
        }

        private Point GetDxfToolsDescriptionLocation()
        {
            return new Point(
                LeftColumnContentLeft,
                GetDxfKaekButtonLocation().Y - dxfToolsDescriptionLabel.Height - DxfButtonsGap);
        }

        private Point GetDxfKaekButtonLocation()
        {
            return new Point(
                LeftColumnContentLeft,
                leftColumn.ClientSize.Height - DxfButtonHeight - LeftColumnBottomMargin);
        }

        private Point GetDxfAllButtonLocation()
        {
            return new Point(LeftColumnContentLeft + convertDxfKaekButton.Width + DxfButtonGap, GetDxfKaekButtonLocation().Y);
        }

        private Point GetHiddenDxfToolsLocation(int left)
        {
            return new Point(left, leftColumn.ClientSize.Height + 12);
        }

        private void LeftColumnResize(object sender, EventArgs e)
        {
            SetDxfLayoutImmediate(IsLoadedDxfFile());
        }

        private void RefreshDropSurfaceText()
        {
            if (dropSurfacePanel == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(selectedFilePath))
            {
                dropSurfacePanel.HasLoadedFile = false;
                dropSurfacePanel.TitleText = TextCatalog.Get(currentLanguage, "DropTitle");
                dropSurfacePanel.HintText = TextCatalog.Get(currentLanguage, "DropHint");
                return;
            }

            dropSurfacePanel.HasLoadedFile = true;
            string fileName = Path.GetFileName(selectedFilePath);
            if (worker.IsBusy)
            {
                dropSurfacePanel.TitleText = TextCatalog.Get(currentLanguage, "LoadedFileBusyDropTitle");
            }
            else
            {
                dropSurfacePanel.TitleText = TextCatalog.Get(currentLanguage, "LoadedFileDropTitle");
            }

            dropSurfacePanel.HintText = string.Format(
                TextCatalog.Get(currentLanguage, "LoadedFileDropHint"),
                string.IsNullOrEmpty(fileName) ? selectedFilePath : fileName,
                selectedFilePath);
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
            SaveUserPreferences();
        }

        private void ThemeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (suppressOptionEvents)
            {
                return;
            }

            ThemeItem item = themeComboBox.SelectedItem as ThemeItem;
            if (item == null || item.Theme == currentTheme)
            {
                return;
            }

            currentTheme = item.Theme;
            ApplyTheme();
            SaveUserPreferences();
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
            if (worker.IsBusy || dxfWorker.IsBusy)
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

        private void ConvertDxfKaekButtonClick(object sender, EventArgs e)
        {
            BeginDxfConversionForLoadedFile(DxfConversionMode.KaekLayer);
        }

        private void ConvertDxfAllButtonClick(object sender, EventArgs e)
        {
            BeginDxfConversionForLoadedFile(DxfConversionMode.AllLayers);
        }

        private void BeginDxfConversionForLoadedFile(DxfConversionMode mode)
        {
            if (worker.IsBusy || dxfWorker.IsBusy)
            {
                ShowError(TextCatalog.Get(currentLanguage, "BusyError"));
                return;
            }

            if (string.IsNullOrEmpty(selectedFilePath)
                || !File.Exists(selectedFilePath)
                || !Path.GetExtension(selectedFilePath).Equals(".dxf", StringComparison.OrdinalIgnoreCase))
            {
                ShowError(TextCatalog.Get(currentLanguage, "DxfNoLoadedFileError"));
                return;
            }

            BeginDxfConversion(selectedFilePath, mode);
        }

        private void BeginDxfConversion(string filePath, DxfConversionMode mode)
        {
            if (worker.IsBusy || dxfWorker.IsBusy)
            {
                ShowError(TextCatalog.Get(currentLanguage, "BusyError"));
                return;
            }

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                ShowError(TextCatalog.Get(currentLanguage, "MissingFileError"));
                return;
            }

            dxfWorker.RunWorkerAsync(new DxfConversionRequest(filePath, mode));
            RefreshVisibleState();
        }

        private void DxfWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            DxfConversionRequest request = (DxfConversionRequest)e.Argument;
            e.Result = DxfConversionService.ConvertMTextToText(request);
        }

        private void DxfWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RefreshVisibleState();

            if (e.Error != null)
            {
                ShowDxfConversionError(e.Error);
                return;
            }

            DxfConversionResult result = e.Result as DxfConversionResult;
            if (result == null)
            {
                statusLabel.Text = TextCatalog.Get(currentLanguage, "DxfFailedStatus");
                ShowError(TextCatalog.Get(currentLanguage, "GenericDxfError"));
                return;
            }

            statusLabel.Text = string.Format(
                TextCatalog.Get(currentLanguage, "DxfReadyStatus"),
                result.ConvertedEntityCount,
                result.OutputFilePath);

            DxfCompletionAction action = ShowDxfCompletionDialog(result);
            if (action == DxfCompletionAction.LoadGeneratedFile)
            {
                BeginProcessing(result.OutputFilePath);
            }
            else if (action == DxfCompletionAction.OpenFolder)
            {
                OpenFileInFolder(result.OutputFilePath);
            }
        }

        private DxfCompletionAction ShowDxfCompletionDialog(DxfConversionResult result)
        {
            DxfCompletionAction selectedAction = DxfCompletionAction.None;
            using (Form dialog = CreateModalDialog(TextCatalog.Get(currentLanguage, "DxfCompleteTitle"), 500, 174))
            {
                Label messageLabel = CreateDialogLabel(
                    string.Format(
                        TextCatalog.Get(currentLanguage, "DxfCompleteMessage"),
                        Path.GetFileName(result.OutputFilePath),
                        result.ConvertedEntityCount),
                    24,
                    24,
                    452,
                    56);
                dialog.Controls.Add(messageLabel);

                Button closeButton = CreateSecondaryButton();
                closeButton.Text = TextCatalog.Get(currentLanguage, "CloseButton");
                closeButton.Location = new Point(46, 104);
                closeButton.Size = new Size(138, 42);
                closeButton.DialogResult = DialogResult.Cancel;
                ApplyDialogButtonTheme(closeButton, false);
                dialog.Controls.Add(closeButton);
                dialog.CancelButton = closeButton;

                Button openFolderButton = CreateSecondaryButton();
                openFolderButton.Text = TextCatalog.Get(currentLanguage, "DxfOpenFolderButton");
                openFolderButton.Location = new Point(194, 104);
                openFolderButton.Size = new Size(138, 42);
                openFolderButton.Click += delegate
                {
                    selectedAction = DxfCompletionAction.OpenFolder;
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
                };
                ApplyDialogButtonTheme(openFolderButton, false);
                dialog.Controls.Add(openFolderButton);

                Button loadButton = CreatePrimaryButton();
                loadButton.Text = TextCatalog.Get(currentLanguage, "DxfLoadButton");
                loadButton.Location = new Point(342, 104);
                loadButton.Size = new Size(138, 42);
                loadButton.Click += delegate
                {
                    selectedAction = DxfCompletionAction.LoadGeneratedFile;
                    dialog.DialogResult = DialogResult.OK;
                    dialog.Close();
                };
                ApplyDialogButtonTheme(loadButton, true);
                dialog.Controls.Add(loadButton);
                dialog.AcceptButton = loadButton;

                dialog.ShowDialog(this);
            }

            return selectedAction;
        }

        private Form CreateModalDialog(string title, int width, int height)
        {
            bool dark = currentTheme == AppTheme.Dark;
            Form dialog = new Form();
            dialog.AutoScaleMode = AutoScaleMode.None;
            dialog.BackColor = dark ? Color.FromArgb(33, 39, 45) : Color.White;
            dialog.ClientSize = new Size(width, height);
            dialog.Font = Font;
            dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            dialog.MaximizeBox = false;
            dialog.MinimizeBox = false;
            dialog.ShowInTaskbar = false;
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.Text = title;
            return dialog;
        }

        private Label CreateDialogLabel(string text, int left, int top, int width, int height)
        {
            bool dark = currentTheme == AppTheme.Dark;
            Label label = new Label();
            label.BackColor = dark ? Color.FromArgb(33, 39, 45) : Color.White;
            label.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 161);
            label.ForeColor = dark ? Color.FromArgb(220, 229, 225) : Color.FromArgb(37, 50, 65);
            label.Location = new Point(left, top);
            label.Size = new Size(width, height);
            label.Text = text;
            return label;
        }

        private void ApplyDialogButtonTheme(Button button, bool primary)
        {
            bool dark = currentTheme == AppTheme.Dark;
            Color bodyText = dark ? Color.FromArgb(220, 229, 225) : Color.FromArgb(37, 50, 65);
            Color panelBackground = dark ? Color.FromArgb(33, 39, 45) : Color.White;
            Color secondaryBorder = dark ? Color.FromArgb(73, 83, 92) : Color.FromArgb(220, 226, 222);

            if (primary)
            {
                button.BackColor = dark ? Color.FromArgb(40, 151, 117) : Color.FromArgb(24, 115, 90);
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderSize = 0;
                return;
            }

            button.BackColor = panelBackground;
            button.ForeColor = bodyText;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = secondaryBorder;
        }

        private void OpenFolderButtonClick(object sender, EventArgs e)
        {
            if (lastResult == null || string.IsNullOrEmpty(lastResult.OutputPdfPath))
            {
                return;
            }

            try
            {
                OpenFileInFolder(lastResult.OutputPdfPath);
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

        private void OpenFileInFolder(string filePath)
        {
            Process.Start("explorer.exe", string.Format("/select,\"{0}\"", filePath));
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

        private void ShowInformation(string message)
        {
            MessageBox.Show(
                this,
                message,
                TextCatalog.Get(currentLanguage, "InfoTitle"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ShowDxfConversionError(Exception error)
        {
            DxfConversionException dxfException = error as DxfConversionException;
            if (dxfException == null)
            {
                statusLabel.Text = TextCatalog.Get(currentLanguage, "DxfFailedStatus");
                ShowError(TextCatalog.Get(currentLanguage, "GenericDxfError"));
                return;
            }

            switch (dxfException.Code)
            {
                case DxfConversionErrorCode.MissingFile:
                    statusLabel.Text = TextCatalog.Get(currentLanguage, "DxfFailedStatus");
                    ShowError(TextCatalog.Get(currentLanguage, "MissingFileError"));
                    break;
                case DxfConversionErrorCode.SourceDirectoryUnavailable:
                    statusLabel.Text = TextCatalog.Get(currentLanguage, "DxfFailedStatus");
                    ShowError(TextCatalog.Get(currentLanguage, "MissingDirectoryError"));
                    break;
                case DxfConversionErrorCode.UnsupportedFileType:
                    statusLabel.Text = TextCatalog.Get(currentLanguage, "DxfFailedStatus");
                    ShowError(TextCatalog.Get(currentLanguage, "DxfUnsupportedFileError"));
                    break;
                case DxfConversionErrorCode.UnsupportedBinaryDxf:
                    statusLabel.Text = TextCatalog.Get(currentLanguage, "DxfFailedStatus");
                    ShowError(TextCatalog.Get(currentLanguage, "DxfBinaryError"));
                    break;
                case DxfConversionErrorCode.MalformedDxf:
                    statusLabel.Text = TextCatalog.Get(currentLanguage, "DxfFailedStatus");
                    ShowError(TextCatalog.Get(currentLanguage, "DxfMalformedError"));
                    break;
                case DxfConversionErrorCode.CannotWriteDxf:
                    statusLabel.Text = TextCatalog.Get(currentLanguage, "DxfFailedStatus");
                    ShowError(TextCatalog.Get(currentLanguage, "DxfWriteError"));
                    break;
                case DxfConversionErrorCode.NoMatchingMText:
                    statusLabel.Text = TextCatalog.Get(currentLanguage, "DxfNoMatchesStatus");
                    ShowInformation(TextCatalog.Get(currentLanguage, "DxfNoMatchesMessage"));
                    break;
                default:
                    statusLabel.Text = TextCatalog.Get(currentLanguage, "DxfFailedStatus");
                    ShowError(TextCatalog.Get(currentLanguage, "GenericDxfError"));
                    break;
            }
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

        private sealed class ThemeItem
        {
            public ThemeItem(AppTheme theme, string label)
            {
                Theme = theme;
                Label = label;
            }

            public AppTheme Theme { get; private set; }

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

        private enum DxfCompletionAction
        {
            None,
            LoadGeneratedFile,
            OpenFolder
        }
    }
}
