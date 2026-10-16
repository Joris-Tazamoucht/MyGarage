using System.Drawing.Drawing2D;
using MyGarage.Styles;

namespace MyGarage.Controls
{
    public enum ButtonVariant
    {
        Primary,    // action principale (bleu plein)
        Secondary,  // action neutre (fond discret + bordure)
        Success,    // action positive (vert plein)
        Danger,     // action destructive (rouge plein)
        Ghost,      // lien discret, fond au survol uniquement
        Nav,        // élément de menu latéral, aligné à gauche
        Chip        // pastille sélectionnable
    }

    /// <summary>Bouton arrondi dessiné à la main, avec icône optionnelle, survol et état actif.</summary>
    public class ThemedButton : Button
    {
        private const TextFormatFlags Flags = TextFormatFlags.NoPadding | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix;

        private bool _hover, _pressed, _active;
        private ButtonVariant _variant = ButtonVariant.Primary;
        private string? _glyph;

        public ButtonVariant Variant { get => _variant; set { _variant = value; Invalidate(); } }

        /// <summary>Icône (voir <see cref="Icons"/>) affichée avant le texte.</summary>
        public string? Glyph { get => _glyph; set { _glyph = value; Invalidate(); } }

        /// <summary>État sélectionné (pour Nav et Chip).</summary>
        public bool Active { get => _active; set { _active = value; Invalidate(); } }

        public ThemedButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;
            Font = AppTheme.FontButton;
            Cursor = Cursors.Hand;
            Size = new Size(AppTheme.S(120), AppTheme.S(38));
        }

        private Font GlyphFont => AppTheme.IconFont(_variant == ButtonVariant.Chip ? 9f : 10.5f);
        private int HorizontalPadding => AppTheme.S(_variant == ButtonVariant.Chip ? 14 : 18);
        private float Radius => _variant == ButtonVariant.Chip ? Height / 2f : AppTheme.S(8);

        /// <summary>Ajuste la largeur au contenu.</summary>
        public void FitWidth(int minWidth = 0) => Width = Math.Max(minWidth, GetPreferredSize(Size.Empty).Width);

        public override Size GetPreferredSize(Size proposedSize)
            => new(MeasureContent().Width + HorizontalPadding * 2, Height);

        private Size MeasureContent()
        {
            int w = 0;
            if (!string.IsNullOrEmpty(_glyph))
                w += TextRenderer.MeasureText(_glyph, GlyphFont, Size.Empty, Flags).Width;
            if (!string.IsNullOrEmpty(Text))
            {
                if (w > 0) w += AppTheme.S(8);
                w += TextRenderer.MeasureText(Text, Font, Size.Empty, Flags).Width;
            }
            return new Size(w, 0);
        }

        // ── États souris ──────────────────────────────────────────────────
        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = _pressed = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { if (e.Button == MouseButtons.Left) { _pressed = true; Invalidate(); } base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { _pressed = false; Invalidate(); base.OnMouseUp(e); }
        protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }

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

        // ── Couleurs selon variante et état ───────────────────────────────
        private (Color back, Color fore, Color border) GetColors(Color parent)
        {
            Color back, fore, border = Color.Empty;
            switch (_variant)
            {
                case ButtonVariant.Primary:
                    back = _pressed ? AppTheme.AccentPressed : _hover ? AppTheme.AccentBlueDark : AppTheme.AccentBlue;
                    fore = Color.White;
                    break;
                case ButtonVariant.Success:
                    back = _pressed ? AppTheme.Blend(AppTheme.SuccessHover, Color.Black, .12f) : _hover ? AppTheme.SuccessHover : AppTheme.SuccessGreen;
                    fore = Color.White;
                    break;
                case ButtonVariant.Danger:
                    back = _pressed ? AppTheme.Blend(AppTheme.DangerHover, Color.Black, .12f) : _hover ? AppTheme.DangerHover : AppTheme.DangerRed;
                    fore = Color.White;
                    break;
                case ButtonVariant.Secondary:
                    back = _pressed ? AppTheme.Blend(AppTheme.Hover, AppTheme.TextPrimary, .08f) : _hover ? AppTheme.Hover : AppTheme.SurfaceAlt;
                    fore = AppTheme.TextPrimary;
                    border = _hover ? AppTheme.BorderStrong : AppTheme.Border;
                    break;
                case ButtonVariant.Chip:
                    if (_active) { back = AppTheme.AccentSoft; fore = AppTheme.AccentText; border = AppTheme.AccentBlue; }
                    else
                    {
                        back = _hover ? AppTheme.Hover : AppTheme.Surface;
                        fore = AppTheme.TextPrimary;
                        border = _hover ? AppTheme.BorderStrong : AppTheme.Border;
                    }
                    break;
                case ButtonVariant.Nav:
                    if (_active) { back = AppTheme.AccentSoft; fore = AppTheme.AccentText; }
                    else
                    {
                        back = _pressed ? AppTheme.Blend(AppTheme.Hover, AppTheme.TextPrimary, .06f) : _hover ? AppTheme.Hover : parent;
                        fore = _hover ? AppTheme.TextPrimary : AppTheme.TextSub;
                    }
                    break;
                default: // Ghost
                    back = _pressed ? AppTheme.Blend(AppTheme.Hover, AppTheme.TextPrimary, .06f) : _hover ? AppTheme.Hover : parent;
                    fore = AppTheme.AccentText;
                    break;
            }

            if (!Enabled)
            {
                back = AppTheme.Blend(back, parent, .55f);
                fore = AppTheme.Blend(fore, parent, .55f);
                if (!border.IsEmpty) border = AppTheme.Blend(border, parent, .55f);
            }
            return (back, fore, border);
        }

        // ── Dessin ────────────────────────────────────────────────────────
        protected override void OnPaintBackground(PaintEventArgs e) { }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var parent = AppTheme.ResolveBackColor(this);
            var (back, fore, border) = GetColors(parent);

            g.Clear(parent);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path = AppTheme.RoundedRect(new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f), Radius))
            {
                using (var brush = new SolidBrush(back)) g.FillPath(brush, path);
                if (!border.IsEmpty)
                {
                    using var pen = new Pen(border);
                    g.DrawPath(pen, path);
                }
            }

            // Contenu : icône + texte
            var content = MeasureContent();
            int x = _variant == ButtonVariant.Nav ? AppTheme.S(12) : (Width - content.Width) / 2;

            if (!string.IsNullOrEmpty(_glyph))
            {
                var glyphSize = TextRenderer.MeasureText(g, _glyph, GlyphFont, Size.Empty, Flags);
                TextRenderer.DrawText(g, _glyph, GlyphFont, new Rectangle(x, 0, glyphSize.Width, Height), fore,
                    Flags | TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                x += glyphSize.Width + (string.IsNullOrEmpty(Text) ? 0 : AppTheme.S(8));
            }
            if (!string.IsNullOrEmpty(Text))
            {
                TextRenderer.DrawText(g, Text, Font, new Rectangle(x, 0, Math.Max(1, Width - x - AppTheme.S(6)), Height), fore,
                    Flags | TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }

            // Anneau de focus clavier
            if (Focused && ShowFocusCues)
            {
                bool filled = _variant is ButtonVariant.Primary or ButtonVariant.Success or ButtonVariant.Danger;
                using var pen = new Pen(filled ? Color.FromArgb(200, Color.White) : AppTheme.AccentBlue, 1.5f);
                using var ring = AppTheme.RoundedRect(new RectangleF(2f, 2f, Width - 4.5f, Height - 4.5f), Math.Max(1, Radius - 2));
                g.DrawPath(pen, ring);
            }
        }
    }
}
