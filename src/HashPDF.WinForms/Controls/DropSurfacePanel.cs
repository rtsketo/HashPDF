using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HashPDF.WinForms.Controls
{
    public sealed class DropSurfacePanel : Panel
    {
        private bool isDragActive;
        private string titleText;
        private string hintText;

        public DropSurfacePanel()
        {
            titleText = "Drop a file";
            hintText = "Drop one file here to create a hash PDF.";
            AllowDrop = true;
            DoubleBuffered = true;
            BackColor = Color.White;
            DragEnter += OnDragEnterInternal;
            DragLeave += OnDragLeaveInternal;
            DragDrop += OnDragDropInternal;
            DragOver += OnDragOverInternal;
            Cursor = Cursors.Hand;
        }

        public event EventHandler<FileDroppedEventArgs> FileDropped;

        public string TitleText
        {
            get { return titleText; }
            set
            {
                titleText = value;
                Invalidate();
            }
        }

        public string HintText
        {
            get { return hintText; }
            set
            {
                hintText = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle bounds = ClientRectangle;
            bounds.Inflate(-1, -1);

            using (GraphicsPath path = CreateRoundedRectangle(bounds, 28))
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    bounds,
                    Color.FromArgb(250, 252, 255),
                    Color.FromArgb(242, 247, 250),
                    LinearGradientMode.ForwardDiagonal))
                {
                    e.Graphics.FillPath(brush, path);
                }

                using (Pen borderPen = new Pen(isDragActive ? Color.FromArgb(20, 137, 125) : Color.FromArgb(212, 220, 228), isDragActive ? 2.5F : 1.5F))
                {
                    borderPen.DashStyle = DashStyle.Dash;
                    e.Graphics.DrawPath(borderPen, path);
                }
            }

            DrawIcon(e.Graphics);
            DrawText(e.Graphics);
        }

        private void DrawIcon(Graphics graphics)
        {
            Rectangle circleRect = new Rectangle((Width / 2) - 36, 96, 72, 72);
            Color circleColor = isDragActive ? Color.FromArgb(217, 246, 242) : Color.FromArgb(228, 244, 241);
            Color strokeColor = Color.FromArgb(20, 137, 125);

            using (SolidBrush brush = new SolidBrush(circleColor))
            {
                graphics.FillEllipse(brush, circleRect);
            }

            using (Pen pen = new Pen(strokeColor, 4F))
            {
                graphics.DrawLine(pen, Width / 2, 112, Width / 2, 142);
                graphics.DrawLine(pen, Width / 2 - 14, 130, Width / 2, 146);
                graphics.DrawLine(pen, Width / 2 + 14, 130, Width / 2, 146);
                graphics.DrawLine(pen, Width / 2 - 20, 160, Width / 2 + 20, 160);
            }
        }

        private void DrawText(Graphics graphics)
        {
            Rectangle titleRect = new Rectangle(48, 204, Width - 96, 46);
            Rectangle hintRect = new Rectangle(72, 254, Width - 144, 76);

            TextRenderer.DrawText(
                graphics,
                titleText,
                new Font("Segoe UI Semibold", 16F, FontStyle.Bold, GraphicsUnit.Point, 161),
                titleRect,
                Color.FromArgb(20, 31, 48),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);

            TextRenderer.DrawText(
                graphics,
                hintText,
                new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 161),
                hintRect,
                Color.FromArgb(88, 101, 119),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak);
        }

        private void OnDragEnterInternal(object sender, DragEventArgs e)
        {
            if (HasSingleFile(e))
            {
                e.Effect = DragDropEffects.Copy;
                isDragActive = true;
                Invalidate();
                return;
            }

            e.Effect = DragDropEffects.None;
        }

        private void OnDragOverInternal(object sender, DragEventArgs e)
        {
            e.Effect = HasSingleFile(e) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void OnDragLeaveInternal(object sender, EventArgs e)
        {
            isDragActive = false;
            Invalidate();
        }

        private void OnDragDropInternal(object sender, DragEventArgs e)
        {
            isDragActive = false;
            Invalidate();

            if (!HasSingleFile(e))
            {
                return;
            }

            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (paths.Length == 1)
            {
                OnFileDropped(paths[0]);
            }
        }

        private static bool HasSingleFile(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return false;
            }

            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            return paths != null && paths.Length == 1;
        }

        private void OnFileDropped(string filePath)
        {
            EventHandler<FileDroppedEventArgs> handler = FileDropped;
            if (handler != null)
            {
                handler(this, new FileDroppedEventArgs(filePath));
            }
        }

        private static GraphicsPath CreateRoundedRectangle(Rectangle rectangle, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rectangle.Left, rectangle.Top, diameter, diameter, 180, 90);
            path.AddArc(rectangle.Right - diameter, rectangle.Top, diameter, diameter, 270, 90);
            path.AddArc(rectangle.Right - diameter, rectangle.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rectangle.Left, rectangle.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    public sealed class FileDroppedEventArgs : EventArgs
    {
        public FileDroppedEventArgs(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; private set; }
    }
}
