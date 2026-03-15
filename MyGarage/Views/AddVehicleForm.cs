using Models.Models;
using MyGarage.Styles;

namespace MyGarage.Views
{
    public partial class AddVehicleForm : Form
    {
        private Panel pnlHeader = new Panel();
        private Panel pnlContent = new Panel();
        private Panel pnlFooter = new Panel();
        private Label lblTitle = new Label();
        private Label lblSubtitle = new Label();

        private TextBox txtMarque = new TextBox();
        private TextBox txtModele = new TextBox();
        private TextBox txtImmatriculation = new TextBox();
        private TextBox txtKilometrage = new TextBox();
        private TextBox txtAnnee = new TextBox();

        private ModernButton btnConfirm = new ModernButton();
        private ModernButton btnCancel = new ModernButton(Color.FromArgb(80, 80, 100), Color.FromArgb(60, 60, 80));

        public Vehicle? Vehicle { get; private set; }

        public AddVehicleForm()
        {
            InitializeComponent();
            BuildUI();
            AppTheme.ThemeChanged += () => AppTheme.ApplyToForm(this);
        }

        private void BuildUI()
        {
            this.Text = "Ajouter un véhicule";
            this.Size = new Size(460, 480);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.BackColor = AppTheme.Background;

            // ── Header ────────────────────────────────────────────────────
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 70;
            pnlHeader.BackColor = AppTheme.Surface;
            pnlHeader.Padding = new Padding(20, 0, 20, 0);

            lblTitle.Text = "➕  Ajouter un véhicule";
            lblTitle.Font = AppTheme.FontTitle;
            lblTitle.ForeColor = AppTheme.TextPrimary;
            lblTitle.BackColor = AppTheme.Surface;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 12);

            lblSubtitle.Text = "Renseignez les informations du véhicule";
            lblSubtitle.Font = AppTheme.FontSmall;
            lblSubtitle.ForeColor = AppTheme.TextSub;
            lblSubtitle.BackColor = AppTheme.Surface;
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(20, 42);

            pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblSubtitle });

            // ── Contenu ───────────────────────────────────────────────────
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = AppTheme.Background;
            pnlContent.Padding = new Padding(20, 16, 20, 0);

            int y = 16;
            pnlContent.Controls.Add(MakeField("Marque *", txtMarque, ref y));
            pnlContent.Controls.Add(MakeField("Modèle *", txtModele, ref y));
            pnlContent.Controls.Add(MakeField("Immatriculation *", txtImmatriculation, ref y));
            pnlContent.Controls.Add(MakeField("Kilométrage", txtKilometrage, ref y));
            pnlContent.Controls.Add(MakeField("Année", txtAnnee, ref y));

            // ── Footer ────────────────────────────────────────────────────
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Height = 60;
            pnlFooter.BackColor = AppTheme.Surface;
            pnlFooter.Padding = new Padding(20, 10, 20, 10);

            btnConfirm.Text = "✔  Ajouter";
            btnConfirm.Size = new Size(130, 38);
            btnConfirm.Location = new Point(200, 11);
            btnConfirm.Click += BtnConfirm_Click;

            btnCancel.Text = "Annuler";
            btnCancel.Size = new Size(100, 38);
            btnCancel.Location = new Point(340, 11);
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            pnlFooter.Controls.AddRange(new Control[] { btnConfirm, btnCancel });

            this.Controls.AddRange(new Control[] { pnlContent, pnlFooter, pnlHeader });
            this.AcceptButton = btnConfirm;
        }

        private Panel MakeField(string label, TextBox txt, ref int y)
        {
            var panel = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(400, 54),
                BackColor = Color.Transparent
            };

            var lbl = new Label
            {
                Text = label,
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSub,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            txt.Font = AppTheme.FontNormal;
            txt.BackColor = AppTheme.Surface;
            txt.ForeColor = AppTheme.TextPrimary;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Size = new Size(400, 28);
            txt.Location = new Point(0, 20);

            panel.Controls.AddRange(new Control[] { lbl, txt });
            y += 58;
            return panel;
        }

        private void BtnConfirm_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMarque.Text) ||
                string.IsNullOrWhiteSpace(txtModele.Text) ||
                string.IsNullOrWhiteSpace(txtImmatriculation.Text))
            {
                MessageBox.Show("Marque, modèle et immatriculation sont obligatoires.",
                    "Champs manquants", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(txtKilometrage.Text, out int km))
            {
                MessageBox.Show("Le kilométrage doit être un nombre entier.",
                    "Valeur invalide", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(txtAnnee.Text, out int annee))
            {
                MessageBox.Show("L'année doit être un nombre entier.",
                    "Valeur invalide", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}