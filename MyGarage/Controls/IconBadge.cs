using System.Drawing.Drawing2D;
using MyGarage.Styles;

namespace MyGarage.Controls
{
    /// <summary>Pastille carrée arrondie contenant une icône. Pleine ou « douce » (fond teinté).</summary>
    public class IconBadge : Control
    {
        private string _glyph = "";
        private Color _tint = AppTheme.AccentBlue;
        private bool _soft;
        private float _glyphSize = 14f;

        public string Glyph { get => _glyph; set { _glyph = value; Invalidate(); } }
        public Color Tint { get => _tint; set { _tint = value; Invalidate(); } }
        public bool Soft { get => _soft; set { _soft = value; Invalidate(); } }
        public float GlyphSize { get => _glyphSize; set { _glyphSize = value; Invalidate(); } }

        public IconBadge()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, false);
            TabStop = false;
            Size = new Size(AppTheme.S(40), AppTheme.S(40));
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            AppTheme.ThemeChanged += Invalidate;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            AppTheme.ThemeChanged -= Invalidate;
            base.OnHandleDestroyed(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e) { }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var parent = AppTheme.ResolveBackColor(this);
            g.Clear(parent);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color back = _soft ? AppTheme.Blend(parent, _tint, AppTheme.IsDark ? 0.24f : 0.14f) : _tint;
            Color fore = _soft ? (AppTheme.IsDark ? AppTheme.Blend(_tint, Color.White, 0.3f) : _tint) : Color.White;

            using (var path = AppTheme.RoundedRect(new RectangleF(0, 0, Width - 1, Height - 1), Math.Min(Width, Height) * 0.28f))
            using (var brush = new SolidBrush(back))
                g.FillPath(brush, path);

            TextRenderer.DrawText(g, _glyph, AppTheme.IconFont(_glyphSize), ClientRectangle, fore,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
        }
    }
}
