using System.Drawing.Drawing2D;
using MyGarage.Styles;

namespace MyGarage.Controls
{
    /// <summary>
    /// DataGridView stylée : en-têtes discrets, lignes aérées, pas de rectangle de focus,
    /// colonnes « pastille » optionnelles et message quand la grille est vide.
    /// </summary>
    public class ThemedGrid : DataGridView
    {
        private static readonly Color[] PillPalette =
        {
            Color.FromArgb(64, 124, 236), Color.FromArgb(232, 142, 44), Color.FromArgb(34, 168, 98),
            Color.FromArgb(139, 92, 246), Color.FromArgb(20, 160, 170), Color.FromArgb(224, 80, 130)
        };

        private readonly Dictionary<string, bool> _pillColumns = new(StringComparer.OrdinalIgnoreCase);
        private string? _emptyText;

        /// <summary>Message affiché au centre quand il n'y a aucune ligne.</summary>
        public string? EmptyText
        {
            get => _emptyText;
            set { _emptyText = value; Invalidate(); }
        }

        public ThemedGrid()
        {
            DoubleBuffered = true;
            BorderStyle = BorderStyle.None;
            RowHeadersVisible = false;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            AllowUserToResizeRows = false;
            ReadOnly = true;
            MultiSelect = false;
            StandardTab = true;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ColumnHeadersHeight = AppTheme.S(40);
            EnableHeadersVisualStyles = false;
            RowTemplate.Height = AppTheme.S(40);

            var cellPadding = new Padding(AppTheme.S(10), 0, AppTheme.S(10), 0);
            DefaultCellStyle.Font = AppTheme.FontNormal;
            DefaultCellStyle.Padding = cellPadding;
            DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ColumnHeadersDefaultCellStyle.Font = AppTheme.FontSmallBold;
            ColumnHeadersDefaultCellStyle.Padding = cellPadding;
            ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Barres de défilement sombres
            foreach (Control c in Controls)
                if (c is ScrollBar bar)
                    bar.HandleCreated += (s, e) => NativeTheme.ApplyScrollBars(bar);

            ApplyTheme();
        }

        /// <summary>Affiche les valeurs de cette colonne sous forme de pastille (colorée ou neutre).</summary>
        public void AddPillColumn(string columnName, bool colored) => _pillColumns[columnName] = colored;

        /// <summary>Configure une colonne si elle existe (à appeler après la liaison des données).</summary>
        public void SetColumn(string name, string? header = null, string? format = null,
                              bool alignRight = false, float fillWeight = 100, bool visible = true)
        {
            var col = Columns[name];
            if (col == null) return;
            col.Visible = visible;
            if (header != null) col.HeaderText = header;
            if (format != null) col.DefaultCellStyle.Format = format;
            if (alignRight)
            {
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            col.FillWeight = fillWeight;
        }

        // ── Thème ─────────────────────────────────────────────────────────
        private void ApplyTheme()
        {
            BackgroundColor = AppTheme.Surface;
            GridColor = AppTheme.Border;
            DefaultCellStyle.BackColor = AppTheme.Surface;
            DefaultCellStyle.ForeColor = AppTheme.TextPrimary;
            DefaultCellStyle.SelectionBackColor = AppTheme.GridSel;
            DefaultCellStyle.SelectionForeColor = AppTheme.GridSelText;
            AlternatingRowsDefaultCellStyle.BackColor = AppTheme.Surface;
            ColumnHeadersDefaultCellStyle.BackColor = AppTheme.Surface;
            ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.TextSub;
            ColumnHeadersDefaultCellStyle.SelectionBackColor = AppTheme.Surface;
            ColumnHeadersDefaultCellStyle.SelectionForeColor = AppTheme.TextSub;
            foreach (Control c in Controls)
                if (c is ScrollBar bar) NativeTheme.ApplyScrollBars(bar);
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

        // ── Dessin des cellules ───────────────────────────────────────────
        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            base.OnCellPainting(e);
            if (e.Handled || e.Graphics is null) return;

            // En-tête : fond uni + fine ligne en dessous
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border & ~DataGridViewPaintParts.Focus);
                using var pen = new Pen(AppTheme.Border);
                e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
                e.Handled = true;
                return;
            }
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string? text = e.FormattedValue?.ToString();
            bool colored = false;
            bool isPill = !string.IsNullOrEmpty(text) && _pillColumns.TryGetValue(Columns[e.ColumnIndex].Name, out colored);

            var parts = DataGridViewPaintParts.All & ~DataGridViewPaintParts.Focus;
            if (isPill) parts &= ~DataGridViewPaintParts.ContentForeground;
            e.Paint(e.ClipBounds, parts);

            if (isPill)
                DrawPill(e.Graphics, e.CellBounds, text!, colored, (e.State & DataGridViewElementStates.Selected) != 0);
            e.Handled = true;
        }

        private void DrawPill(Graphics g, Rectangle cell, string text, bool colored, bool selected)
        {
            const TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix;
            var font = AppTheme.FontSmallBold;
            var size = TextRenderer.MeasureText(g, text, font, Size.Empty, flags);
            int h = Math.Min(AppTheme.S(24), cell.Height - AppTheme.S(8));
            int w = Math.Min(size.Width + AppTheme.S(20), cell.Width - AppTheme.S(16));
            if (w <= 0 || h <= 0) return;

            var rect = new Rectangle(cell.X + AppTheme.S(10), cell.Y + (cell.Height - h) / 2, w, h);
            Color baseColor = selected ? AppTheme.GridSel : AppTheme.Surface;
            Color tint = colored ? PillPalette[StableIndex(text)] : AppTheme.TextSub;
            Color back = AppTheme.Blend(baseColor, tint, AppTheme.IsDark ? 0.22f : 0.12f);
            Color fore = !colored ? AppTheme.TextPrimary
                       : AppTheme.IsDark ? AppTheme.Blend(tint, Color.White, 0.35f)
                       : AppTheme.Blend(tint, Color.Black, 0.25f);

            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = AppTheme.RoundedRect(rect, h / 2f))
            using (var brush = new SolidBrush(back))
                g.FillPath(brush, path);
            g.SmoothingMode = oldMode;

            TextRenderer.DrawText(g, text, font, rect, fore,
                flags | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        // Couleur stable pour un même texte (string.GetHashCode change à chaque lancement)
        private static int StableIndex(string s)
        {
            int h = 0;
            foreach (char c in s.ToLowerInvariant()) h = (h * 31 + c) & 0x7FFFFFFF;
            return h % PillPalette.Length;
        }

        // ── État vide ─────────────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Rows.Count > 0 || string.IsNullOrEmpty(_emptyText)) return;
            int top = ColumnHeadersVisible && Columns.Count > 0 ? ColumnHeadersHeight : 0;
            var area = new Rectangle(AppTheme.S(16), top, Width - AppTheme.S(32), Height - top);
            TextRenderer.DrawText(e.Graphics, _emptyText, AppTheme.FontNormal, area, AppTheme.TextMuted,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
        }

        protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e) { base.OnDataBindingComplete(e); Invalidate(); }
        protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e) { base.OnRowsAdded(e); Invalidate(); }
        protected override void OnRowsRemoved(DataGridViewRowsRemovedEventArgs e) { base.OnRowsRemoved(e); Invalidate(); }
    }
}
