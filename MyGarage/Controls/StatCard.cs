using MyGarage.Styles;

namespace MyGarage.Controls
{
    /// <summary>Carte de statistique : libellé, grande valeur, détail, icône teintée.</summary>
    public class StatCard : CardPanel
    {
        private readonly IconBadge _icon = new() { Soft = true, GlyphSize = 13f };
        private readonly ThemedLabel _caption = new() { Role = TextRole.Secondary, Font = AppTheme.FontSmallBold, AutoSize = false, AutoEllipsis = true };
        private readonly ThemedLabel _value = new() { Font = AppTheme.FontStat, AutoSize = false, AutoEllipsis = true };
        private readonly ThemedLabel _detail = new() { Role = TextRole.Muted, Font = AppTheme.FontSmall, AutoSize = false, AutoEllipsis = true };

        public StatCard(string glyph, Color tint, string caption)
        {
            _icon.Glyph = glyph;
            _icon.Tint = tint;
            _icon.Size = new Size(AppTheme.S(38), AppTheme.S(38));
            _caption.Text = caption;
            _value.Text = "—";
            Controls.AddRange(new Control[] { _icon, _caption, _value, _detail });
            Height = AppTheme.S(96);
        }

        public string Value { get => _value.Text; set => _value.Text = value; }
        public string Detail { get => _detail.Text; set => _detail.Text = value; }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            int pad = AppTheme.S(16);
            _icon.Location = new Point(Width - pad - _icon.Width, pad);
            int textWidth = Math.Max(10, _icon.Left - pad - AppTheme.S(8));
            _caption.SetBounds(pad, AppTheme.S(14), textWidth, AppTheme.S(20));
            _value.SetBounds(pad - AppTheme.S(1), AppTheme.S(34), textWidth, AppTheme.S(32));
            _detail.SetBounds(pad, AppTheme.S(68), Math.Max(10, Width - pad * 2), AppTheme.S(18));
        }
    }
}
