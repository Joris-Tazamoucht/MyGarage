using Models.Models;
using MyGarage.Styles;

namespace MyGarage.Views
{
    public partial class VehicleHistoryForm : Form
    {
        private readonly VehicleHistory _history;
        private Panel pnlHeader = new Panel();
        private Panel pnlStats = new Panel();
        private Panel pnlContent = new Panel();
        private Panel pnlFooter = new Panel();
        private Label lblTitle = new Label();
        private Label lblSubtitle = new Label();
        private Label lblStatNb = new Label();
        private Label lblStatTotal = new Label();
        private Label lblStatMoy = new Label();
        private Label lblStatLast = new Label();
        private DataGridView dgv = new DataGridView();
        private ModernButton btnClose = new ModernButton(Color.FromArgb(80, 80, 100), Color.FromArgb(60, 60, 80));

        public VehicleHistoryForm(VehicleHistory history)
        {
            InitializeComponent();
            _history = history;
            BuildUI();
            AppTheme.ThemeChanged += () => AppTheme.ApplyToForm(this);
        }

        private void BuildUI()
        {
            var v = _history.Vehicle;
            this.Text = "Historique d'entretien";
            this.Size = new Size(860, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.BackColor = AppTheme.Background;

            // ── Header ────────────────────────────────────────────────────
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 70;
            pnlHeader.BackColor = AppTheme.Surface;

            lblTitle.Text = $"📋  {v?.Marque} {v?.Modele}";
            lblTitle.Font = AppTheme.FontTitle;
            lblTitle.ForeColor = AppTheme.TextPrimary;
            lblTitle.BackColor = AppTheme.Surface;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 12);

            lblSubtitle.Text = $"{v?.Immatriculation}  •  {v?.Annee}  •  {v?.Kilometrage:N0} km";
            lblSubtitle.Font = AppTheme.FontSmall;
            lblSubtitle.ForeColor = AppTheme.AccentBlueLight;
            lblSubtitle.BackColor = AppTheme.Surface;
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(20, 44);

            pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblSubtitle });

            // ── Stats ─────────────────────────────────────────────────────
            pnlStats.Dock = DockStyle.Top;
            pnlStats.Height = 60;
            pnlStats.BackColor = AppTheme.Background;
            pnlStats.Padding = new Padding(16, 8, 16, 8);

            int nb = _history.Historique?.Count ?? 0;
            float total = _history.Historique?.Sum(e => e.cout ?? 0) ?? 0;
            float moyenne = nb > 0 ? total / nb : 0;
            var dernier = _history.Historique?.FirstOrDefault();
            string dernierDate = dernier != null && DateTime.TryParse(dernier.date_etretien, out var d)
                ? d.ToString("dd/MM/yyyy") : "N/A";

            lblStatNb = MakeStat($"🔧 {nb} entretien(s)", 0);
            lblStatTotal = MakeStat($"💶 Total : {total:N2} €", 200);
            lblStatMoy = MakeStat($"📊 Moyenne : {moyenne:N2} €", 400);
            lblStatLast = MakeStat($"📅 Dernier : {dernierDate}", 600);

            pnlStats.Controls.AddRange(new Control[] { lblStatNb, lblStatTotal, lblStatMoy, lblStatLast });

            // ── Grille ────────────────────────────────────────────────────
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = AppTheme.Background;
            pnlContent.Padding = new Padding(16, 8, 16, 8);

            dgv.Dock = DockStyle.Fill;
            AppTheme.ApplyToDataGridView(dgv);

            var rows = _history.Historique?.Select(ent => new
            {
                Date = DateTime.TryParse(ent.date_etretien, out var dd) ? dd.ToString("dd/MM/yyyy") : ent.date_etretien,
                Type = ent.type_entretien,
                Kilométrage = ent.kilometrage?.ToString("N0") + " km",
                Coût = ent.cout?.ToString("N2") + " €",
                Notes = ent.notes
            }).ToList();

            dgv.DataSource = rows;
            pnlContent.Controls.Add(dgv);

            // ── Footer ────────────────────────────────────────────────────
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Height = 55;
            pnlFooter.BackColor = AppTheme.Surface;

            btnClose.Text = "✖  Fermer";
            btnClose.Size = new Size(120, 36);
            btnClose.Location = new Point(720, 10);
            btnClose.Click += (s, e) => this.Close();

            pnlFooter.Controls.Add(btnClose);
            this.Controls.AddRange(new Control[] { pnlContent, pnlFooter, pnlStats, pnlHeader });
        }

        private Label MakeStat(string text, int x) => new Label
        {
            Text = text,
            Font = AppTheme.FontNormal,
            ForeColor = AppTheme.TextPrimary,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(x, 16)
        };
    }
}