using MyGarage.Styles;

namespace MyGarage.Styles
{
    public class ModernButton : Button
    {
        private Color _baseColor;
        private Color _hoverColor;
        private Color _pressColor;
        private bool _isHovered;
        private bool _isPressed;
        private int _radius = 8;

        public ModernButton(Color? baseColor = null, Color? hoverColor = null)
        {
            _baseColor = baseColor ?? AppTheme.AccentBlue;
            _hoverColor = hoverColor ?? AppTheme.AccentBlueDark;
            _pressColor = Color.FromArgb(30, 70, 160);

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            ForeColor = Color.White;
            Font = AppTheme.FontButton;
            Cursor = Cursors.Hand;
            Size = new Size(160, 40);

            AppTheme.ThemeChanged += () => Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _isPressed = true;
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias; // ← ajouter

            Color bg = _isPressed ? _pressColor : _isHovered ? _hoverColor : _baseColor;

            using var path = RoundedRect(ClientRectangle, _radius);
            using var brush = new SolidBrush(bg);
            e.Graphics.FillPath(brush, path);

            // ✅ Police Segoe UI Emoji pour supporter les emojis
            using var emojiFont = new Font("Segoe UI Emoji", Font.Size, Font.Style);
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            using var textBrush = new SolidBrush(ForeColor);
            e.Graphics.DrawString(Text, emojiFont, textBrush, ClientRectangle, sf);
        }
        private static System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        public void SetBaseColor(Color baseColor, Color hoverColor)
        {
            _baseColor = baseColor;
            _hoverColor = hoverColor;
            Invalidate();
        }
    }
}