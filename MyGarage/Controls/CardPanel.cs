using System.Drawing.Drawing2D;
using MyGarage.Styles;

namespace MyGarage.Controls
{
    /// <summary>Carte à coins arrondis avec bordure fine, couleur Surface.</summary>
    public class CardPanel : Panel
    {
        private int _radius = AppTheme.S(12);

        public int Radius
        {
            get => _radius;
            set { _radius = value; Invalidate(); }
        }

        public bool ShowBorder { get; set; } = true;

        public CardPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            Padding = new Padding(AppTheme.S(16));
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            BackColor = AppTheme.Surface;
            Invalidate(true);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            AppTheme.ThemeChanged += ApplyTheme;
            ApplyTheme();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            AppTheme.ThemeChanged -= ApplyTheme;
            base.OnHandleDestroyed(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(AppTheme.ResolveBackColor(this));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = AppTheme.RoundedRect(new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f), _radius);
            using (var brush = new SolidBrush(BackColor)) g.FillPath(brush, path);
            if (ShowBorder)
            {
                using var pen = new Pen(AppTheme.Border);
                g.DrawPath(pen, path);
            }
        }
    }
}
