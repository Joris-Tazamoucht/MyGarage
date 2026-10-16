using Models.Models;
using MyGarage.Controls;
using MyGarage.Styles;

namespace MyGarage.Views
{
    public partial class AddEntretienForm : DialogBase
    {
        private static readonly string[] Types =
        {
            "Vidange", "Révision", "Freins", "Pneus", "Courroie",
            "Filtres", "Batterie", "Amortisseurs", "Autre"
        };

        private readonly int _vehicleId;
        private readonly InputBox txtType = new() { PlaceholderText = "Choisissez ci-dessous ou saisissez un type" };
        private readonly ChipSelector chipsType = new(Types);
        private readonly DateField dtDate = new();
        private readonly InputBox txtKilometrage = new() { PlaceholderText = "ex. 92 500" };
        private readonly InputBox txtCout = new() { PlaceholderText = "ex. 149,90" };
        private readonly InputBox txtNotes = new() { Multiline = true, PlaceholderText = "Pièces changées, garage, remarques…" };

        public Entretien? Entretien { get; private set; }

        public AddEntretienForm(int vehicleId, string vehicleLabel)
        {
            InitializeComponent();
            _vehicleId = vehicleId;
            BuildUI(vehicleLabel);
        }

        private void BuildUI(string vehicleLabel)
        {
            SetHeader(Icons.Repair, "Nouvel entretien", vehicleLabel, AppTheme.WarningOrange, accentSubtitle: true);
            SetDialogWidth(540);
            UseFieldColumns(3);

            dtDate.Value = DateTime.Today;
            txtNotes.Height = S(84);
            txtType.TextChanged += (s, e) => chipsType.SelectValue(txtType.Text);
            chipsType.ChipClicked += (s, type) =>
            {
                txtType.Text = type;
                txtType.Inner.SelectionStart = type.Length;
            };

            AddField("Type d'entretien", txtType, span: 3, required: true, bottomGap: S(10));
            AddField(null, chipsType, span: 3, bottomGap: S(20));
            AddField("Date", dtDate, required: true);
            AddField("Kilométrage", txtKilometrage, required: true);
            AddField("Coût (€)", txtCout, required: true);
            AddField("Notes", txtNotes, span: 3);

            var confirm = AddFooterButton("Ajouter", ButtonVariant.Primary, Icons.Accept, BtnConfirm_Click);
            var cancel = AddFooterButton("Annuler", ButtonVariant.Secondary, null,
                (s, e) => { DialogResult = DialogResult.Cancel; Close(); });
            AcceptButton = confirm;
            CancelButton = cancel;
        }

        private void BtnConfirm_Click(object? sender, EventArgs e)
        {
            ClearError();
            if (string.IsNullOrWhiteSpace(txtType.Text)) { ShowError("Le type d'entretien est obligatoire.", txtType); return; }
            if (dtDate.Value is not DateTime date) { ShowError("Date invalide (jj/mm/aaaa).", dtDate); return; }
            if (!TryParseInt(txtKilometrage.Text, out int km) || km < 0)
            {
                ShowError("Kilométrage : nombre entier attendu.", txtKilometrage);
                return;
            }
            if (!TryParseFloat(txtCout.Text, out float cout) || cout < 0)
            {
                ShowError("Coût : nombre attendu (ex. 149,90).", txtCout);
                return;
            }

            Entretien = new Entretien
            {
                vehicle_id = _vehicleId,
                date_etretien = date.ToString("yyyy-MM-dd"),
                type_entretien = txtType.Text.Trim(),
                kilometrage = km,
                cout = cout,
                notes = txtNotes.Text.Trim()
            };
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
