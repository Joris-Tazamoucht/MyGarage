using Models.Models;

namespace MyGarage.Views
{
    public partial class ExportSelectionForm : Form
    {
        private readonly List<Vehicle> _vehicles;
        private CheckedListBox clbVehicles = new CheckedListBox();
        private Button btnSelectAll = new Button();
        private Button btnDeselectAll = new Button();
        private Button btnExportForm = new Button();
        private Button btnCancel = new Button();
        private CheckBox chkEmail = new CheckBox();
        private CheckBox chkSms = new CheckBox();
        private TextBox txtEmail = new TextBox();
        private Panel pnlEmail = new Panel();
        private Panel pnlSms = new Panel();

        public List<Vehicle> SelectedVehicles { get; private set; } = new();
        public bool SendEmail => chkEmail.Checked;
        public bool SendSms => chkSms.Checked;
        public string Email => txtEmail.Text.Trim();

        public ExportSelectionForm(List<Vehicle> vehicles)
        {
            InitializeComponent();
            _vehicles = vehicles;

            this.Text = "Exporter un rapport";
            this.Size = new Size(480, 450);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;

            // ── Liste des véhicules ───────────────────────────────────────
            var lblVehicles = new Label
            {
                Text = "Sélectionnez les véhicules à exporter :",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 12)
            };

            clbVehicles.Location = new Point(12, 38);
            clbVehicles.Size = new Size(440, 150);
            clbVehicles.CheckOnClick = true;

            foreach (var v in vehicles)
                clbVehicles.Items.Add($"{v.Marque} {v.Modele} — {v.Immatriculation}", false);

            btnSelectAll.Text = "Tout sélectionner";
            btnSelectAll.Location = new Point(12, 195);
            btnSelectAll.Size = new Size(140, 30);
            btnSelectAll.Click += (s, e) =>
            {
                for (int i = 0; i < clbVehicles.Items.Count; i++)
                    clbVehicles.SetItemChecked(i, true);
            };

            btnDeselectAll.Text = "Désélectionner";
            btnDeselectAll.Location = new Point(165, 195);
            btnDeselectAll.Size = new Size(130, 30);
            btnDeselectAll.Click += (s, e) =>
            {
                for (int i = 0; i < clbVehicles.Items.Count; i++)
                    clbVehicles.SetItemChecked(i, false);
            };

            // ── Options d'envoi ───────────────────────────────────────────
            var lblEnvoi = new Label
            {
                Text = "Options d'envoi :",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 240)
            };

            chkEmail.Text = "Envoyer par email";
            chkEmail.Location = new Point(12, 265);
            chkEmail.AutoSize = true;
            chkEmail.CheckedChanged += (s, e) => pnlEmail.Visible = chkEmail.Checked;

            // Panel email — uniquement destinataire, SMTP depuis appsettings.json
            pnlEmail.Location = new Point(12, 290);
            pnlEmail.Size = new Size(440, 38);
            pnlEmail.Visible = false;
            pnlEmail.BorderStyle = BorderStyle.FixedSingle;

            pnlEmail.Controls.Add(MakeLabel("Email destinataire :", 8, 8));
            txtEmail.Location = new Point(160, 5);
            txtEmail.Width = 265;
            pnlEmail.Controls.Add(txtEmail);

            chkSms.Text = "Envoyer par SMS (Free Mobile)";
            chkSms.Location = new Point(12, 340);
            chkSms.AutoSize = true;
            chkSms.CheckedChanged += (s, e) => pnlSms.Visible = chkSms.Checked;

            // Panel SMS — identifiants depuis appsettings.json
            pnlSms.Location = new Point(12, 365);
            pnlSms.Size = new Size(440, 30);
            pnlSms.Visible = false;
            pnlSms.BorderStyle = BorderStyle.FixedSingle;

            pnlSms.Controls.Add(new Label
            {
                Text = "Identifiants Free Mobile lus depuis appsettings.json",
                AutoSize = true,
                ForeColor = Color.Gray,
                Location = new Point(8, 8)
            });

            // ── Boutons ───────────────────────────────────────────────────
            btnExportForm.Text = "Exporter";
            btnExportForm.Size = new Size(110, 35);
            btnExportForm.Location = new Point(240, 390);
            btnExportForm.DialogResult = DialogResult.OK;
            btnExportForm.Click += BtnExportForm_Click;

            btnCancel.Text = "Annuler";
            btnCancel.Size = new Size(110, 35);
            btnCancel.Location = new Point(362, 390);
            btnCancel.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[]
            {
                lblVehicles, clbVehicles, btnSelectAll, btnDeselectAll,
                lblEnvoi, chkEmail, pnlEmail, chkSms, pnlSms,
                btnExportForm, btnCancel
            });

            this.AcceptButton = btnExportForm;
            this.CancelButton = btnCancel;
        }

        private static Label MakeLabel(string text, int x, int y) => new Label
        {
            Text = text,
            AutoSize = true,
            Location = new Point(x, y)
        };

        private void BtnExportForm_Click(object? sender, EventArgs e)
        {
            var selected = new List<Vehicle>();
            for (int i = 0; i < clbVehicles.CheckedIndices.Count; i++)
                selected.Add(_vehicles[clbVehicles.CheckedIndices[i]]);

            if (!selected.Any())
            {
                MessageBox.Show("Veuillez sélectionner au moins un véhicule.",
                    "Aucun véhicule", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (chkEmail.Checked && string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Veuillez saisir un email destinataire.",
                    "Champ manquant", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            SelectedVehicles = selected;
        }
    }
}