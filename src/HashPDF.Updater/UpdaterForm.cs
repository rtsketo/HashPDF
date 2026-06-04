using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HashPDF.Updater
{
    internal sealed class UpdaterForm : Form
    {
        private readonly UpdateArguments arguments;
        private readonly UpdaterText text;
        private readonly BackgroundWorker worker;
        private readonly Label titleLabel;
        private readonly Label detailLabel;
        private readonly ProgressBar progressBar;
        private readonly Button closeButton;

        public UpdaterForm(UpdateArguments arguments)
        {
            this.arguments = arguments;
            text = new UpdaterText(arguments.Language);

            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(33, 39, 45);
            ClientSize = new Size(520, 170);
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 161);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = text.WindowTitle;

            titleLabel = new Label();
            titleLabel.AutoEllipsis = true;
            titleLabel.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 161);
            titleLabel.ForeColor = Color.FromArgb(236, 242, 239);
            titleLabel.Location = new Point(24, 22);
            titleLabel.Size = new Size(472, 28);
            titleLabel.Text = text.Starting;
            Controls.Add(titleLabel);

            detailLabel = new Label();
            detailLabel.AutoEllipsis = true;
            detailLabel.ForeColor = Color.FromArgb(168, 180, 176);
            detailLabel.Location = new Point(24, 56);
            detailLabel.Size = new Size(472, 24);
            detailLabel.Text = arguments.TargetVersion;
            Controls.Add(detailLabel);

            progressBar = new ProgressBar();
            progressBar.Location = new Point(24, 90);
            progressBar.Size = new Size(472, 20);
            progressBar.Style = ProgressBarStyle.Continuous;
            Controls.Add(progressBar);

            closeButton = new Button();
            closeButton.BackColor = Color.FromArgb(33, 39, 45);
            closeButton.Cursor = Cursors.Hand;
            closeButton.FlatAppearance.BorderColor = Color.FromArgb(73, 83, 92);
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.ForeColor = Color.White;
            closeButton.Location = new Point(358, 124);
            closeButton.Size = new Size(138, 34);
            closeButton.Text = text.Close;
            closeButton.Visible = false;
            closeButton.Click += delegate { Close(); };
            Controls.Add(closeButton);

            worker = new BackgroundWorker();
            worker.DoWork += WorkerDoWork;
            worker.RunWorkerCompleted += WorkerRunWorkerCompleted;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            worker.RunWorkerAsync();
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            UpdaterEngine engine = new UpdaterEngine(ReportProgress, text);
            e.Result = engine.Run(arguments);
        }

        private void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                titleLabel.Text = text.Failed;
                detailLabel.Text = e.Error.Message;
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Value = 0;
                closeButton.Visible = true;
                return;
            }

            UpdateRunResult result = (UpdateRunResult)e.Result;
            if (result == UpdateRunResult.ElevationStarted)
            {
                Close();
                return;
            }

            Timer closeTimer = new Timer();
            closeTimer.Interval = 900;
            closeTimer.Tick += delegate
            {
                closeTimer.Stop();
                closeTimer.Dispose();
                Close();
            };
            closeTimer.Start();
        }

        private void ReportProgress(UpdateProgress progress)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<UpdateProgress>(ReportProgress), progress);
                return;
            }

            titleLabel.Text = progress.Title;
            detailLabel.Text = progress.Detail;
            progressBar.Style = progress.Indeterminate ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous;
            if (!progress.Indeterminate)
            {
                progressBar.Value = Math.Max(progressBar.Minimum, Math.Min(progressBar.Maximum, progress.Percent));
            }
        }
    }
}
