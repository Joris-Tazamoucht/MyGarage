using System.Drawing.Drawing2D;
using MyGarage.Styles;

namespace MyGarage.Controls
{
    /// <summary>Barre de progression fine et arrondie (0 à 100).</summary>
    public class ThemedProgressBar : Control
    {
        private int _value;

        public int Value
        {
            get => _value;
            set { _value = Math.Clamp(value, 0, 100); Invalidate(); }
        }

        public ThemedProgressBar()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, false);
            TabStop = false;
            Height = AppTheme.S(8);
        }

        protected override void OnPaintBackground(PaintEventArgs e) { }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(AppTheme.ResolveBackColor(this));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            float radius = Height / 2f;

            using (var track = AppTheme.RoundedRect(new RectangleF(0, 0, Width - 1, Height - 1), radius))
            using (var brush = new SolidBrush(AppTheme.Border))
                g.FillPath(brush, track);

            float fill = (Width - 1) * _value / 100f;
            if (fill < 1) return;
            using var bar = AppTheme.RoundedRect(new RectangleF(0, 0, Math.Max(fill, Height), Height - 1), radius);
            using var accent = new SolidBrush(AppTheme.AccentBlue);
            g.FillPath(accent, bar);
        }
    }
}
