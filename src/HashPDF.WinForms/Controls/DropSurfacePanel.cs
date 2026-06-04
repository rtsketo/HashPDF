using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HashPDF.WinForms.Controls
{
    public sealed class DropSurfacePanel : Panel
    {
        private bool isDragActive;
        private bool useDarkTheme;
        private bool hasLoadedFile;
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

        public bool UseDarkTheme
        {
            get { return useDarkTheme; }
            set
            {
                useDarkTheme = value;
                BackColor = useDarkTheme ? Color.FromArgb(33, 39, 45) : Color.White;
                Invalidate();
            }
        }

        public bool HasLoadedFile
        {
            get { return hasLoadedFile; }
            set
            {
                hasLoadedFile = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle bounds = ClientRectangle;
            bounds.Inflate(-2, -2);

            using (GraphicsPath path = CreateRoundedRectangle(bounds, 28))
            {
                Color gradientStart = useDarkTheme ? Color.FromArgb(40, 47, 54) : Color.FromArgb(252, 253, 252);
                Color gradientEnd = useDarkTheme ? Color.FromArgb(30, 35, 41) : Color.FromArgb(240, 246, 242);
                Color borderColor = useDarkTheme ? Color.FromArgb(78, 89, 99) : Color.FromArgb(214, 222, 218);
                Color activeBorderColor = useDarkTheme ? Color.FromArgb(40, 151, 117) : Color.FromArgb(24, 115, 90);

                using (LinearGradientBrush brush = new LinearGradientBrush(
                    bounds,
                    gradientStart,
                    gradientEnd,
                    LinearGradientMode.ForwardDiagonal))
                {
                    e.Graphics.FillPath(brush, path);
                }

                using (Pen borderPen = new Pen(isDragActive ? activeBorderColor : borderColor, isDragActive ? 2.5F : 1.5F))
                {
                    borderPen.DashStyle = isDragActive ? DashStyle.Solid : DashStyle.Dash;
                    e.Graphics.DrawPath(borderPen, path);
                }
            }

            DrawIcon(e.Graphics);
            DrawText(e.Graphics);
        }

        private void DrawIcon(Graphics graphics)
        {
            Rectangle badgeRect = new Rectangle((Width / 2) - 42, GetContentTop(), 84, 84);
            Color badgeColor = useDarkTheme
                ? (isDragActive || hasLoadedFile ? Color.FromArgb(58, 88, 79) : Color.FromArgb(53, 66, 72))
                : (isDragActive || hasLoadedFile ? Color.FromArgb(217, 241, 233) : Color.FromArgb(231, 241, 237));
            Color strokeColor = useDarkTheme ? Color.FromArgb(99, 208, 172) : Color.FromArgb(24, 115, 90);

            using (GraphicsPath badgePath = CreateRoundedRectangle(badgeRect, 22))
            {
                using (SolidBrush brush = new SolidBrush(badgeColor))
                {
                    graphics.FillPath(brush, badgePath);
                }
            }

            using (Pen pen = new Pen(strokeColor, 3F))
            {
                int iconTop = badgeRect.Top + 16;
                graphics.DrawRectangle(pen, (Width / 2) - 20, iconTop, 40, 50);
                graphics.DrawLine(pen, (Width / 2) + 4, iconTop, (Width / 2) + 20, iconTop + 16);
                graphics.DrawLine(pen, (Width / 2) + 4, iconTop, (Width / 2) + 4, iconTop + 16);
                graphics.DrawLine(pen, (Width / 2) + 4, iconTop + 16, (Width / 2) + 20, iconTop + 16);
                if (hasLoadedFile)
                {
                    graphics.DrawLine(pen, (Width / 2) - 11, iconTop + 31, (Width / 2) - 1, iconTop + 41);
                    graphics.DrawLine(pen, (Width / 2) - 1, iconTop + 41, (Width / 2) + 13, iconTop + 23);
                }
                else
                {
                    graphics.DrawLine(pen, (Width / 2) - 10, iconTop + 24, (Width / 2) + 10, iconTop + 24);
                    graphics.DrawLine(pen, (Width / 2) - 10, iconTop + 36, (Width / 2) + 10, iconTop + 36);
                }
            }
        }

        private void DrawText(Graphics graphics)
        {
            int titleTop = GetContentTop() + 108;
            int hintTop = titleTop + 46;
            Rectangle titleRect = new Rectangle(48, titleTop, Width - 96, 42);
            Rectangle hintRect = new Rectangle(68, hintTop, Width - 136, Math.Max(54, Height - hintTop - 18));
            Color titleColor = useDarkTheme ? Color.FromArgb(235, 242, 239) : Color.FromArgb(26, 34, 32);
            Color hintColor = useDarkTheme ? Color.FromArgb(173, 186, 181) : Color.FromArgb(97, 108, 104);

            TextRenderer.DrawText(
                graphics,
                titleText,
                new Font("Segoe UI Semibold", 15F, FontStyle.Bold, GraphicsUnit.Point, 161),
                titleRect,
                titleColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);

            TextRenderer.DrawText(
                graphics,
                hintText,
                new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 161),
                hintRect,
                hintColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak);
        }

        private int GetContentTop()
        {
            return Math.Max(28, (Height / 2) - 122);
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
