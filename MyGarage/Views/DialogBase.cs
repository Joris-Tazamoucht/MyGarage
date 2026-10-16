using System.Globalization;
using MyGarage.Controls;
using MyGarage.Styles;

namespace MyGarage.Views
{
    /// <summary>
    /// Base commune des fenêtres de dialogue : en-tête (icône + titre + sous-titre),
    /// grille de champs, pied avec message d'erreur à gauche et boutons à droite.
    /// </summary>
    public class DialogBase : Form
    {
        protected readonly ThemedPanel Header = new() { Role = SurfaceRole.Surface, Edges = AnchorStyles.Bottom };
        protected readonly IconBadge HeaderIcon = new() { GlyphSize = 16f };
        protected readonly ThemedLabel HeaderTitle = new() { Font = AppTheme.FontTitle };
        protected readonly ThemedLabel HeaderSubtitle = new() { Role = TextRole.Secondary };
        protected readonly ThemedPanel Body = new() { Role = SurfaceRole.Background };
        protected readonly ThemedPanel Footer = new() { Role = SurfaceRole.Surface, Edges = AnchorStyles.Top };

        private readonly FlowLayoutPanel _buttons = new();
        private readonly ThemedLabel _error = new()
        {
            Role = TextRole.Danger,
            Font = AppTheme.FontMedium,
            AutoSize = false,
            AutoEllipsis = true,
            TextAlign = ContentAlignment.MiddleLeft
        };
        private TableLayoutPanel? _fields;
        private int _columns = 2, _col, _row = -1;

        /// <summary>Si vrai, la hauteur de la fenêtre s'ajuste aux champs à l'ouverture.</summary>
        protected bool AutoFitHeight { get; set; } = true;

        protected static int S(int value) => AppTheme.S(value);

        public DialogBase()
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;

            // En-tête
            Header.Dock = DockStyle.Top;
            Header.Height = S(80);
            HeaderIcon.Size = new Size(S(44), S(44));
            HeaderIcon.Location = new Point(S(24), S(18));
            HeaderTitle.Location = new Point(S(82), S(16));
            HeaderSubtitle.Location = new Point(S(83), S(45));
            Header.Controls.AddRange(new Control[] { HeaderIcon, HeaderTitle, HeaderSubtitle });

            // Pied
            Footer.Dock = DockStyle.Bottom;
            Footer.Height = S(68);
            _error.Dock = DockStyle.Fill;
            _error.Padding = new Padding(S(24), 0, S(8), 0);
            _buttons.Dock = DockStyle.Right;
            _buttons.AutoSize = true;
            _buttons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _buttons.FlowDirection = FlowDirection.RightToLeft;
            _buttons.WrapContents = false;
            _buttons.Padding = new Padding(0, S(15), S(24), 0);
            Footer.Controls.Add(_error);
            Footer.Controls.Add(_buttons);

            // Corps
            Body.Dock = DockStyle.Fill;
            Body.Padding = new Padding(S(24), S(20), S(24), S(8));

            Controls.Add(Body);
            Controls.Add(Footer);
            Controls.Add(Header);
            BackColor = AppTheme.Background;
        }

        // ── Construction ──────────────────────────────────────────────────
        protected void SetHeader(string glyph, string title, string? subtitle, Color? tint = null, bool accentSubtitle = false)
        {
            HeaderIcon.Glyph = glyph;
            HeaderIcon.Tint = tint ?? AppTheme.AccentBlue;
            HeaderTitle.Text = title;
            HeaderSubtitle.Text = subtitle ?? "";
            HeaderSubtitle.Role = accentSubtitle ? TextRole.Accent : TextRole.Secondary;
            Text = title;
        }

        protected void SetDialogWidth(int width) => ClientSize = new Size(S(width), ClientSize.Height);

        /// <summary>Nombre de colonnes de la grille de champs (à appeler avant le premier AddField).</summary>
        protected void UseFieldColumns(int columns) => _columns = Math.Max(1, columns);

        private TableLayoutPanel Fields
        {
            get
            {
                if (_fields != null) return _fields;
                _fields = new TableLayoutPanel
                {
                    Dock = DockStyle.Top,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    ColumnCount = _columns,
                    Margin = Padding.Empty
                };
                for (int i = 0; i < _columns; i++)
                    _fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / _columns));
                Body.Controls.Add(_fields);
                return _fields;
            }
        }

        /// <summary>
        /// Ajoute un champ dans la grille. <paramref name="caption"/> null = contrôle seul, sans libellé.
        /// <paramref name="span"/> = nombre de colonnes occupées.
        /// </summary>
        protected T AddField<T>(string? caption, T editor, int span = 1, bool required = false, int bottomGap = -1) where T : Control
        {
            var table = Fields;
            span = Math.Clamp(span, 1, _columns);
            if (_row < 0 || _col + span > _columns)
            {
                _row++;
                _col = 0;
                table.RowCount = _row + 1;
                table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            Control cell = caption == null ? editor : new FormField(caption + (required ? " *" : ""), editor);
            cell.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            int gap = S(8);
            cell.Margin = new Padding(
                _col == 0 ? 0 : gap, 0,
                _col + span == _columns ? 0 : gap,
                bottomGap >= 0 ? bottomGap : S(16));

            table.Controls.Add(cell, _col, _row);
            if (span > 1) table.SetColumnSpan(cell, span);
            _col += span;
            return editor;
        }

        protected ThemedButton AddFooterButton(string text, ButtonVariant variant, string? glyph, EventHandler onClick)
        {
            var button = new ThemedButton
            {
                Text = text,
                Variant = variant,
                Glyph = glyph,
                Height = S(38),
                Margin = new Padding(S(10), 0, 0, 0)
            };
            button.FitWidth(S(104));
            button.Click += onClick;
            _buttons.Controls.Add(button);   // le premier ajouté est le plus à droite
            return button;
        }

        // ── Erreurs de saisie ─────────────────────────────────────────────
        protected void ShowError(string message, InputBox? field = null)
        {
            _error.Text = message;
            if (field != null)
            {
                field.HasError = true;
                field.Inner.Focus();
            }
        }

        protected void ClearError() => _error.Text = "";

        protected static bool TryParseInt(string text, out int value)
            => int.TryParse(StripSpaces(text), NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

        protected static bool TryParseFloat(string text, out float value)
            => float.TryParse(StripSpaces(text).Replace("€", "").Replace(',', '.'),
                              NumberStyles.Float, CultureInfo.InvariantCulture, out value);

        private static string StripSpaces(string s) => new(s.Where(c => !char.IsWhiteSpace(c)).ToArray());

        // ── Hauteur automatique ───────────────────────────────────────────
        protected void RefitHeight()
        {
            if (_fields == null) return;
            int inner = ClientSize.Width - Body.Padding.Horizontal;
            _fields.Width = inner;
            _fields.PerformLayout();
            _fields.PerformLayout();
            int fieldsHeight = _fields.GetPreferredSize(new Size(inner, 0)).Height;
            ClientSize = new Size(ClientSize.Width, Header.Height + Footer.Height + Body.Padding.Vertical + fieldsHeight);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (AutoFitHeight) RefitHeight();
        }

        // ── Thème ─────────────────────────────────────────────────────────
        private void ApplyTheme()
        {
            BackColor = AppTheme.Background;
            NativeTheme.ApplyTitleBar(this);
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
    }
}
