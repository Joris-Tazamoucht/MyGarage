using Models.Models;
using MyGarage.Controls;
using MyGarage.Styles;

namespace MyGarage.Views
{
    public partial class AddVehicleForm : DialogBase
    {
        private readonly InputBox txtMarque = new() { PlaceholderText = "ex. Peugeot" };
        private readonly InputBox txtModele = new() { PlaceholderText = "ex. 208" };
        private readonly InputBox txtImmatriculation = new() { PlaceholderText = "AB-123-CD" };
        private readonly InputBox txtKilometrage = new() { PlaceholderText = "ex. 85 000" };
        private readonly InputBox txtAnnee = new() { PlaceholderText = "ex. 2019" };

        public Vehicle? Vehicle { get; private set; }

        public AddVehicleForm()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            SetHeader(Icons.Car, "Nouveau véhicule", "Renseignez les informations du véhicule");
            SetDialogWidth(480);
            UseFieldColumns(2);

            txtImmatriculation.Inner.CharacterCasing = CharacterCasing.Upper;
            txtAnnee.Inner.MaxLength = 4;

            AddField("Marque", txtMarque, required: true);
            AddField("Modèle", txtModele, required: true);
            AddField("Immatriculation", txtImmatriculation, span: 2, required: true);
            AddField("Kilométrage", txtKilometrage, required: true);
            AddField("Année", txtAnnee, required: true);

            var confirm = AddFooterButton("Ajouter", ButtonVariant.Primary, Icons.Accept, BtnConfirm_Click);
            var cancel = AddFooterButton("Annuler", ButtonVariant.Secondary, null,
                (s, e) => { DialogResult = DialogResult.Cancel; Close(); });
            AcceptButton = confirm;
            CancelButton = cancel;
        }

        private void BtnConfirm_Click(object? sender, EventArgs e)
        {
            ClearError();
            if (string.IsNullOrWhiteSpace(txtMarque.Text)) { ShowError("La marque est obligatoire.", txtMarque); return; }
            if (string.IsNullOrWhiteSpace(txtModele.Text)) { ShowError("Le modèle est obligatoire.", txtModele); return; }
            if (string.IsNullOrWhiteSpace(txtImmatriculation.Text)) { ShowError("L'immatriculation est obligatoire.", txtImmatriculation); return; }
            if (!TryParseInt(txtKilometrage.Text, out int km) || km < 0)
            {
                ShowError("Kilométrage : nombre entier attendu.", txtKilometrage);
                return;
            }
            int maxYear = DateTime.Today.Year + 1;
            if (!TryParseInt(txtAnnee.Text, out int annee) || annee < 1900 || annee > maxYear)
            {
                ShowError($"Année : entre 1900 et {maxYear}.", txtAnnee);
                return;
            }

            Vehicle = new Vehicle
            {
                Marque = txtMarque.Text.Trim(),
                Modele = txtModele.Text.Trim(),
                Immatriculation = txtImmatriculation.Text.Trim().ToUpper(),
                Kilometrage = km,
                Annee = annee
            };
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
