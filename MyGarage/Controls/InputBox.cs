using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;
using MyGarage.Styles;

namespace MyGarage.Controls
{
    /// <summary>
    /// Champ texte arrondi : bordure fine, bordure bleue au focus, rouge en erreur,
    /// icône cliquable optionnelle à droite.
    /// </summary>
    public class InputBox : Control
    {
        private bool _focused, _hover, _error, _trailHover;
        private string? _trailingGlyph;

        public TextBox Inner { get; } = new() { BorderStyle = BorderStyle.None };

        /// <summary>Clic sur l'icône de droite.</summary>
        public event EventHandler? TrailingClick;

        public InputBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, false);
            TabStop = false;

            Inner.Font = AppTheme.FontNormal;
            Inner.GotFocus += (s, e) => { _focused = true; Invalidate(); };
            Inner.LostFocus += (s, e) => { _focused = false; Invalidate(); };
            Inner.TextChanged += (s, e) =>
            {
                if (_error) { _error = false; Invalidate(); }
                OnTextChanged(EventArgs.Empty);
            };
            Inner.MouseEnter += (s, e) => SetHover(true);
            Inner.MouseLeave += (s, e) => SetHover(ClientRectangle.Contains(PointToClient(Cursor.Position)));
            Controls.Add(Inner);

            Size = new Size(AppTheme.S(200), AppTheme.S(38));
            Cursor = Cursors.IBeam;
            ApplyTheme();
        }

        // ── Propriétés ────────────────────────────────────────────────────
        [AllowNull]
        public override string Text
        {
            get => Inner.Text;
            set => Inner.Text = value ?? "";
        }

        public string PlaceholderText
        {
            get => Inner.PlaceholderText;
            set => Inner.PlaceholderText = value;
        }

        public bool Multiline
        {
            get => Inner.Multiline;
            set
            {
                Inner.Multiline = value;
                Inner.AcceptsReturn = value;
                PerformLayout();
            }
        }

        /// <summary>Affiche la bordure en rouge. Effacé automatiquement à la saisie.</summary>
        public bool HasError
        {
            get => _error;
            set { _error = value; Invalidate(); }
        }

        public string? TrailingGlyph
        {
            get => _trailingGlyph;
            set { _trailingGlyph = value; PerformLayout(); Invalidate(); }
        }

        private int TrailWidth => _trailingGlyph == null ? 0 : AppTheme.S(34);
        private Rectangle TrailRect => new(Width - TrailWidth - AppTheme.S(2), 0, TrailWidth, Height);

        // ── Thème ─────────────────────────────────────────────────────────
        private void ApplyTheme()
        {
            Inner.BackColor = AppTheme.InputBack;
            Inner.ForeColor = AppTheme.TextPrimary;
            Invalidate();
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

        // ── Mise en page ──────────────────────────────────────────────────
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            int padX = AppTheme.S(11);
            int width = Math.Max(10, Width - padX * 2 - TrailWidth);
            if (Inner.Multiline)
                Inner.SetBounds(padX, AppTheme.S(9), width, Math.Max(10, Height - AppTheme.S(18)));
            else
                Inner.SetBounds(padX, Math.Max(0, (Height - Inner.Height) / 2), width, Inner.Height);
        }

        // ── Souris ────────────────────────────────────────────────────────
        private void SetHover(bool value)
        {
            if (_hover == value) return;
            _hover = value;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e) { SetHover(true); base.OnMouseEnter(e); }

        protected override void OnMouseLeave(EventArgs e)
        {
            _trailHover = false;
            SetHover(ClientRectangle.Contains(PointToClient(Cursor.Position)));
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            bool over = _trailingGlyph != null && TrailRect.Contains(e.Location);
            if (over != _trailHover)
            {
                _trailHover = over;
                Cursor = over ? Cursors.Hand : Cursors.IBeam;
                Invalidate();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (_trailingGlyph != null && TrailRect.Contains(e.Location))
                TrailingClick?.Invoke(this, EventArgs.Empty);
            else
                Inner.Focus();
        }

        protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }

        // ── Dessin ────────────────────────────────────────────────────────
        protected override void OnPaintBackground(PaintEventArgs e) { }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var parent = AppTheme.ResolveBackColor(this);
            g.Clear(parent);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color back = Enabled ? AppTheme.InputBack : AppTheme.Blend(AppTheme.InputBack, parent, .5f);
            Color border = _error ? AppTheme.DangerRed
                         : _focused ? AppTheme.AccentBlue
                         : _hover ? AppTheme.BorderStrong
                         : AppTheme.Border;
            float penWidth = _focused || _error ? 1.6f : 1f;

            using (var path = AppTheme.RoundedRect(new RectangleF(0.75f, 0.75f, Width - 1.5f, Height - 1.5f), AppTheme.S(8)))
            {
                using (var brush = new SolidBrush(back)) g.FillPath(brush, path);
                using var pen = new Pen(border, penWidth);
                g.DrawPath(pen, path);
            }

            if (_trailingGlyph != null)
            {
                TextRenderer.DrawText(g, _trailingGlyph, AppTheme.IconFont(11f), TrailRect,
                    _trailHover ? AppTheme.AccentBlue : AppTheme.TextSub,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }
        }
    }
}
