namespace MyGarage.Styles
{
    public static class AppTheme
    {
        public static bool IsDark { get; private set; } = true;

        // ── Couleurs Dark ─────────────────────────────────────────────────
        private static readonly Color DarkBackground = Color.FromArgb(18, 18, 24);
        private static readonly Color DarkSurface = Color.FromArgb(28, 28, 38);
        private static readonly Color DarkSideBar = Color.FromArgb(22, 22, 32);
        private static readonly Color DarkText = Color.FromArgb(230, 230, 240);
        private static readonly Color DarkSubText = Color.FromArgb(140, 140, 160);
        private static readonly Color DarkBorder = Color.FromArgb(45, 45, 60);
        private static readonly Color DarkGridAlt = Color.FromArgb(32, 32, 44);
        private static readonly Color DarkGridSel = Color.FromArgb(50, 80, 140);

        // ── Couleurs Light ────────────────────────────────────────────────
        private static readonly Color LightBackground = Color.FromArgb(245, 247, 252);
        private static readonly Color LightSurface = Color.FromArgb(255, 255, 255);
        private static readonly Color LightSideBar = Color.FromArgb(235, 238, 248);
        private static readonly Color LightText = Color.FromArgb(20, 20, 40);
        private static readonly Color LightSubText = Color.FromArgb(100, 100, 130);
        private static readonly Color LightBorder = Color.FromArgb(210, 215, 230);
        private static readonly Color LightGridAlt = Color.FromArgb(240, 243, 252);
        private static readonly Color LightGridSel = Color.FromArgb(180, 200, 240);

        // ── Couleurs communes ─────────────────────────────────────────────
        public static readonly Color AccentBlue = Color.FromArgb(60, 120, 220);
        public static readonly Color AccentBlueDark = Color.FromArgb(40, 90, 180);
        public static readonly Color AccentBlueLight = Color.FromArgb(100, 160, 255);
        public static readonly Color DangerRed = Color.FromArgb(220, 60, 60);
        public static readonly Color SuccessGreen = Color.FromArgb(50, 180, 100);
        public static readonly Color WarningOrange = Color.FromArgb(220, 140, 40);

        // ── Accesseurs dynamiques ─────────────────────────────────────────
        public static Color Background => IsDark ? DarkBackground : LightBackground;
        public static Color Surface => IsDark ? DarkSurface : LightSurface;
        public static Color SideBar => IsDark ? DarkSideBar : LightSideBar;
        public static Color TextPrimary => IsDark ? DarkText : LightText;
        public static Color TextSub => IsDark ? DarkSubText : LightSubText;
        public static Color Border => IsDark ? DarkBorder : LightBorder;
        public static Color GridAlt => IsDark ? DarkGridAlt : LightGridAlt;
        public static Color GridSel => IsDark ? DarkGridSel : LightGridSel;

        public static Font FontTitle => new Font("Segoe UI", 14F, FontStyle.Bold);
        public static Font FontSubtitle => new Font("Segoe UI", 10F, FontStyle.Bold);
        public static Font FontNormal => new Font("Segoe UI", 9F);
        public static Font FontSmall => new Font("Segoe UI", 8F);
        public static Font FontButton => new Font("Segoe UI", 10F, FontStyle.Bold);

        public static event Action? ThemeChanged;

        public static void Toggle()
        {
            IsDark = !IsDark;
            ThemeChanged?.Invoke();
        }

        // ── Helpers d'application du thème ───────────────────────────────
        public static void ApplyToDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = Surface;
            dgv.GridColor = Border;
            dgv.BorderStyle = BorderStyle.None;
            dgv.DefaultCellStyle.BackColor = Surface;
            dgv.DefaultCellStyle.ForeColor = TextPrimary;
            dgv.DefaultCellStyle.Font = FontNormal;
            dgv.DefaultCellStyle.SelectionBackColor = GridSel;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = GridAlt;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = TextPrimary;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = AccentBlue;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = FontSubtitle;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 8, 8, 8);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeight = 40;  // ← hauteur fixe en pixels
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersVisible = false;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.RowTemplate.Height = 32;
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.AlternatingRowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        public static void ApplyToForm(Form form)
        {
            form.BackColor = Background;
            form.ForeColor = TextPrimary;
            ApplyToControls(form.Controls);
        }

        public static void ApplyToControls(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                switch (c)
                {
                    case Panel p:
                        p.BackColor = Surface;
                        ApplyToControls(p.Controls);
                        break;
                    case Label l:
                        l.ForeColor = TextPrimary;
                        l.BackColor = Color.Transparent;
                        break;
                    case TextBox t:
                        t.BackColor = Background;
                        t.ForeColor = TextPrimary;
                        t.BorderStyle = BorderStyle.FixedSingle;
                        break;
                    case ComboBox cb:
                        cb.BackColor = Background;
                        cb.ForeColor = TextPrimary;
                        cb.FlatStyle = FlatStyle.Flat;
                        break;
                    case CheckBox chk:
                        chk.ForeColor = TextPrimary;
                        chk.BackColor = Color.Transparent;
                        break;
                    case CheckedListBox clb:
                        clb.BackColor = Background;
                        clb.ForeColor = TextPrimary;
                        clb.BorderStyle = BorderStyle.FixedSingle;
                        break;
                    case RichTextBox rtb:
                        rtb.BackColor = Background;
                        rtb.ForeColor = TextPrimary;
                        rtb.BorderStyle = BorderStyle.FixedSingle;
                        break;
                    case DateTimePicker dtp:
                        dtp.CalendarForeColor = TextPrimary;
                        dtp.CalendarMonthBackground = Background;
                        break;
                    case DataGridView dgv:
                        ApplyToDataGridView(dgv);
                        break;
                }
            }
        }
    }
}