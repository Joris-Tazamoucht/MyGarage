using System.Drawing.Drawing2D;
using MyGarage.Styles;

namespace MyGarage.Controls
{
    /// <summary>
    /// Ligne cochable : case arrondie + titre + détail.
    /// Mode « Tile » : carte entière cliquable avec bordure, pour les options.
    /// </summary>
    public class CheckRow : Control
    {
        private const TextFormatFlags Flags = TextFormatFlags.NoPadding | TextFormatFlags.SingleLine |
                                              TextFormatFlags.EndEllipsis | TextFormatFlags.Left | TextFormatFlags.NoPrefix;
        private bool _checked, _hover, _tile, _detailInline;
        private string? _detail;

        public event EventHandler? CheckedChanged;

        public CheckRow()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
            TabStop = true;
            Cursor = Cursors.Hand;
            Font = AppTheme.FontNormal;
            Height = AppTheme.S(42);
        }

        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked == value) return;
                _checked = value;
                Invalidate();
                CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>Texte secondaire (sous le titre, ou à droite si DetailInline).</summary>
        public string? Detail { get => _detail; set { _detail = value; Invalidate(); } }

        public bool DetailInline { get => _detailInline; set { _detailInline = value; Invalidate(); } }

        public bool Tile
        {
            get => _tile;
            set { _tile = value; Height = AppTheme.S(value ? 60 : 42); Invalidate(); }
        }

        // ── Interaction ───────────────────────────────────────────────────
        protected override void OnClick(EventArgs e) { Checked = !Checked; Focus(); base.OnClick(e); }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space) { Checked = !Checked; e.Handled = true; }
            base.OnKeyDown(e);
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnGotFocus(EventArgs e) { Invalidate(); base.OnGotFocus(e); }
        protected override void OnLostFocus(EventArgs e) { Invalidate(); base.OnLostFocus(e); }
        protected override void OnTextChanged(EventArgs e) { Invalidate(); base.OnTextChanged(e); }

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

        // ── Dessin ────────────────────────────────────────────────────────
        protected override void OnPaintBackground(PaintEventArgs e) { }

        protected override void OnPaint(PaintEventArgs e)
        {
            static int S(int v) => AppTheme.S(v);
            var g = e.Graphics;
            g.Clear(AppTheme.ResolveBackColor(this));
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var bounds = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);

            if (_tile)
            {
                Color back = _checked ? AppTheme.AccentSoft : _hover ? AppTheme.Hover : AppTheme.Surface;
                Color border = _checked ? AppTheme.AccentBlue : AppTheme.Border;
                using var path = AppTheme.RoundedRect(bounds, S(10));
                using (var brush = new SolidBrush(back)) g.FillPath(brush, path);
                using var pen = new Pen(border);
                g.DrawPath(pen, path);
            }
            else if (_hover)
            {
                using var path = AppTheme.RoundedRect(bounds, S(8));
                using var brush = new SolidBrush(AppTheme.Hover);
                g.FillPath(brush, path);
            }

            // Case à cocher
            int box = S(18);
            var boxRect = new Rectangle(S(_tile ? 16 : 12), (Height - box) / 2, box, box);
            using (var path = AppTheme.RoundedRect(boxRect, S(5)))
            {
                if (_checked)
                {
                    using var brush = new SolidBrush(AppTheme.AccentBlue);
                    g.FillPath(brush, path);
                }
                else
                {
                    using var brush = new SolidBrush(AppTheme.InputBack);
                    g.FillPath(brush, path);
                    using var pen = new Pen(AppTheme.BorderStrong, 1.2f);
                    g.DrawPath(pen, path);
                }
            }
            if (_checked)
            {
                TextRenderer.DrawText(g, Icons.CheckMark, AppTheme.IconFont(8f), boxRect, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }

            // Textes
            int textX = boxRect.Right + S(12);
            int right = Width - S(14);
            var titleFont = _tile ? AppTheme.FontMedium : AppTheme.FontNormal;

            if (string.IsNullOrEmpty(_detail))
            {
                TextRenderer.DrawText(g, Text, titleFont, new Rectangle(textX, 0, right - textX, Height),
                    AppTheme.TextPrimary, Flags | TextFormatFlags.VerticalCenter);
            }
            else if (_detailInline)
            {
                var detailSize = TextRenderer.MeasureText(g, _detail, AppTheme.FontSmall, Size.Empty, Flags);
                var detailRect = new Rectangle(right - detailSize.Width, 0, detailSize.Width, Height);
                TextRenderer.DrawText(g, _detail, AppTheme.FontSmall, detailRect, AppTheme.TextSub, Flags | TextFormatFlags.VerticalCenter);
                TextRenderer.DrawText(g, Text, titleFont, new Rectangle(textX, 0, Math.Max(1, detailRect.Left - S(12) - textX), Height),
                    AppTheme.TextPrimary, Flags | TextFormatFlags.VerticalCenter);
            }
            else
            {
                int titleH = TextRenderer.MeasureText(g, "Ag", titleFont, Size.Empty, Flags).Height;
                int detailH = TextRenderer.MeasureText(g, "Ag", AppTheme.FontSmall, Size.Empty, Flags).Height;
                int gap = S(2);
                int y = (Height - titleH - gap - detailH) / 2;
                TextRenderer.DrawText(g, Text, titleFont, new Rectangle(textX, y, right - textX, titleH), AppTheme.TextPrimary, Flags);
                TextRenderer.DrawText(g, _detail, AppTheme.FontSmall, new Rectangle(textX, y + titleH + gap, right - textX, detailH), AppTheme.TextSub, Flags);
            }

            if (Focused && ShowFocusCues)
            {
                using var pen = new Pen(AppTheme.AccentBlue, 1.5f);
                using var ring = AppTheme.RoundedRect(new RectangleF(1.5f, 1.5f, Width - 3f, Height - 3f), S(_tile ? 9 : 7));
                g.DrawPath(pen, ring);
            }
        }
    }
}
