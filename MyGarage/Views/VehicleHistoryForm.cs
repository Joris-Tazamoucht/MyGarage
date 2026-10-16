using Models.Models;
using MyGarage.Controls;
using MyGarage.Styles;

namespace MyGarage.Views
{
    public partial class VehicleHistoryForm : DialogBase
    {
        private readonly VehicleHistory _history;
        private readonly ThemedGrid dgv = new() { Dock = DockStyle.Fill, EmptyText = "Aucun entretien enregistré pour ce véhicule." };

        public VehicleHistoryForm(VehicleHistory history)
        {
            InitializeComponent();
            _history = history;
            BuildUI();
        }

        private void BuildUI()
        {
            var v = _history.Vehicle;
            AutoFitHeight = false;
            SetHeader(Icons.History, $"{v?.Marque} {v?.Modele}".Trim(),
                $"{v?.Immatriculation}  •  {v?.Annee}  •  {v?.Kilometrage:N0} km", accentSubtitle: true);
            Text = "Historique d'entretien";

            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            ClientSize = new Size(S(940), S(640));
            MinimumSize = new Size(S(760), S(500));

            // ── Statistiques ──────────────────────────────────────────────
            var list = _history.Historique?.ToList() ?? new List<Entretien>();
            int nb = list.Count;
            float total = list.Sum(e => e.cout ?? 0);
            float moyenne = nb > 0 ? total / nb : 0;
            var dates = list.Select(e => DateTime.TryParse(e.date_etretien, out var d) ? d : (DateTime?)null)
                            .Where(d => d.HasValue).ToList();
            string dernier = dates.Count > 0 ? dates.Max()!.Value.ToString("dd/MM/yyyy") : "—";

            var stats = new TableLayoutPanel { Dock = DockStyle.Top, Height = S(96), ColumnCount = 4, RowCount = 1, Margin = Padding.Empty };
            for (int i = 0; i < 4; i++) stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            stats.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var cards = new[]
            {
                new StatCard(Icons.Repair, AppTheme.WarningOrange, "Entretiens") { Value = nb.ToString(), Detail = "au total" },
                new StatCard(Icons.Money, AppTheme.SuccessGreen, "Dépenses totales") { Value = $"{total:N2} €" },
                new StatCard(Icons.Calculator, AppTheme.AccentBlue, "Coût moyen") { Value = $"{moyenne:N2} €", Detail = "par entretien" },
                new StatCard(Icons.Calendar, AppTheme.Purple, "Dernier entretien") { Value = dernier }
            };
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].Dock = DockStyle.Fill;
                cards[i].Margin = new Padding(i == 0 ? 0 : S(6), 0, i == cards.Length - 1 ? 0 : S(6), 0);
                stats.Controls.Add(cards[i], i, 0);
            }

            // ── Grille ────────────────────────────────────────────────────
            var gridCard = new CardPanel { Dock = DockStyle.Fill, Padding = new Padding(S(8), S(6), S(8), S(8)) };
            gridCard.Controls.Add(dgv);
            dgv.AddPillColumn("Type", colored: true);
            dgv.DataBindingComplete += (s, e) =>
            {
                dgv.SetColumn("Date", fillWeight: 70);
                dgv.SetColumn("Type", fillWeight: 90);
                dgv.SetColumn("Kilométrage", alignRight: true, fillWeight: 80);
                dgv.SetColumn("Coût", alignRight: true, fillWeight: 70);
                dgv.SetColumn("Notes", fillWeight: 170);
            };

            var rows = list
                .OrderByDescending(ent => DateTime.TryParse(ent.date_etretien, out var d) ? d : DateTime.MinValue)
                .Select(ent => new
                {
                    Date = DateTime.TryParse(ent.date_etretien, out var d) ? d.ToString("dd/MM/yyyy") : ent.date_etretien,
                    Type = ent.type_entretien,
                    Kilométrage = ent.kilometrage?.ToString("N0") + " km",
                    Coût = ent.cout?.ToString("N2") + " €",
                    Notes = ent.notes
                }).ToList();
            if (rows.Count > 0) dgv.DataSource = rows;

            // Ordre d'ajout important pour le docking : Fill d'abord, puis Top
            Body.Controls.Add(gridCard);
            Body.Controls.Add(new Panel { Dock = DockStyle.Top, Height = S(16) });
            Body.Controls.Add(stats);
            Body.Padding = new Padding(S(24), S(20), S(24), S(20));

            var close = AddFooterButton("Fermer", ButtonVariant.Secondary, null, (s, e) => Close());
            CancelButton = close;
        }
    }
}
