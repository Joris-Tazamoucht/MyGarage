using Models.Models;
using MyGarage.Styles;

namespace MyGarage.Views
{
    public partial class ExportSelectionForm : Form
    {
        private readonly List<Vehicle> _vehicles;
        private Panel pnlHeader = new Panel();
        private Panel pnlContent = new Panel();
        private Panel pnlFooter = new Panel();
        private Label lblTitle = new Label();
        private Label lblSubtitle = new Label();

        private CheckedListBox clbVehicles = new CheckedListBox();
        private ModernButton btnSelectAll = new ModernButton(Color.FromArgb(60, 80, 120), Color.FromArgb(40, 60, 100));
        private ModernButton btnDeselectAll = new ModernButton(Color.FromArgb(80, 80, 100), Color.FromArgb(60, 60, 80));
        private CheckBox chkEmail = new CheckBox();
        private CheckBox chkSms = new CheckBox();
        private TextBox txtEmail = new TextBox();
        private Panel pnlEmail = new Panel();
        private Panel pnlSms = new Panel();
        private ModernButton btnExportForm = new ModernButton(AppTheme.SuccessGreen, Color.FromArgb(30, 140, 70));
        private ModernButton btnCancel = new ModernButton(Color.FromArgb(80, 80, 100), Color.FromArgb(60, 60, 80));

        public List<Vehicle> SelectedVehicles { get; private set; } = new();
        public bool SendEmail => chkEmail.Checked;
        public bool SendSms => chkSms.Checked;
        public string Email => txtEmail.Text.Trim();

        public ExportSelectionForm(List<Vehicle> vehicles)
        {
            InitializeComponent();
            _vehicles = vehicles;
            BuildUI();
            AppTheme.ThemeChanged += () => AppTheme.ApplyToForm(this);
        }

        private void BuildUI()
        {
            this.Text = "Exporter un rapport";
            this.Size = new Size(500, 520);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.BackColor = AppTheme.Background;

            // ── Header ────────────────────────────────────────────────────
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 70;
            pnlHeader.BackColor = AppTheme.Surface;

            lblTitle.Text = "📊  Exporter un rapport";
            lblTitle.Font = AppTheme.FontTitle;
            lblTitle.ForeColor = AppTheme.TextPrimary;
            lblTitle.BackColor = AppTheme.Surface;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 12);

            lblSubtitle.Text = "Sélectionnez les véhicules et les options d'envoi";
            lblSubtitle.Font = AppTheme.FontSmall;
            lblSubtitle.ForeColor = AppTheme.TextSub;
            lblSubtitle.BackColor = AppTheme.Surface;
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(20, 44);

            pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblSubtitle });

            // ── Contenu ───────────────────────────────────────────────────
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = AppTheme.Background;
            pnlContent.Padding = new Padding(20, 12, 20, 0);

            var lblVehicles = new Label
            {
                Text = "VÉHICULES",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSub,
                AutoSize = true,
                Location = new Point(0, 12)
            };

            clbVehicles.Location = new Point(0, 34);
            clbVehicles.Size = new Size(450, 140);
            clbVehicles.CheckOnClick = true;
            clbVehicles.BackColor = AppTheme.Surface;
            clbVehicles.ForeColor = AppTheme.TextPrimary;
            clbVehicles.BorderStyle = BorderStyle.FixedSingle;
            clbVehicles.Font = AppTheme.FontNormal;

            foreach (var v in _vehicles)  // ← vehicles → _vehicles
                clbVehicles.Items.Add($"{v.Marque} {v.Modele}  —  {v.Immatriculation}", false);

            btnSelectAll.Text = "Tout sélectionner";
            btnSelectAll.Size = new Size(150, 32);
            btnSelectAll.Location = new Point(0, 182);
            btnSelectAll.Click += (s, e) => { for (int i = 0; i < clbVehicles.Items.Count; i++) clbVehicles.SetItemChecked(i, true); };

            btnDeselectAll.Text = "Désélectionner";
            btnDeselectAll.Size = new Size(140, 32);
            btnDeselectAll.Location = new Point(158, 182);
            btnDeselectAll.Click += (s, e) => { for (int i = 0; i < clbVehicles.Items.Count; i++) clbVehicles.SetItemChecked(i, false); };

            var lblEnvoi = new Label
            {
                Text = "OPTIONS D'ENVOI",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSub,
                AutoSize = true,
                Location = new Point(0, 228)
            };

            chkEmail.Text = "Envoyer par email";
            chkEmail.Font = AppTheme.FontNormal;
            chkEmail.ForeColor = AppTheme.TextPrimary;
            chkEmail.BackColor = Color.Transparent;
            chkEmail.AutoSize = true;
            chkEmail.Location = new Point(0, 252);
            chkEmail.CheckedChanged += (s, e) => pnlEmail.Visible = chkEmail.Checked;

            pnlEmail.Location = new Point(0, 278);
            pnlEmail.Size = new Size(450, 42);
            pnlEmail.Visible = false;
            pnlEmail.BackColor = AppTheme.Surface;
            pnlEmail.BorderStyle = BorderStyle.FixedSingle;
            pnlEmail.Padding = new Padding(8);

            var lblEmailDest = new Label { Text = "Destinataire :", Font = AppTheme.FontSmall, ForeColor = AppTheme.TextSub, AutoSize = true, Location = new Point(8, 12) };
            txtEmail.Font = AppTheme.FontNormal;
            txtEmail.BackColor = AppTheme.Background;
            txtEmail.ForeColor = AppTheme.TextPrimary;
            txtEmail.BorderStyle = BorderStyle.None;
            txtEmail.Location = new Point(120, 10);
            txtEmail.Width = 315;
            pnlEmail.Controls.AddRange(new Control[] { lblEmailDest, txtEmail });

            chkSms.Text = "Envoyer par SMS (Free Mobile)";
            chkSms.Font = AppTheme.FontNormal;
            chkSms.ForeColor = AppTheme.TextPrimary;
            chkSms.BackColor = Color.Transparent;
            chkSms.AutoSize = true;
            chkSms.Location = new Point(0, 332);
            chkSms.CheckedChanged += (s, e) => pnlSms.Visible = chkSms.Checked;

            pnlSms.Location = new Point(0, 358);
            pnlSms.Size = new Size(450, 32);
            pnlSms.Visible = false;
            pnlSms.BackColor = AppTheme.Surface;
            pnlSms.BorderStyle = BorderStyle.FixedSingle;

            pnlSms.Controls.Add(new Label
            {
                Text = "✅  Identifiants Free Mobile lus depuis appsettings.json",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.SuccessGreen,
                AutoSize = true,
                Location = new Point(8, 8)
            });

            pnlContent.Controls.AddRange(new Control[]
            {
                lblVehicles, clbVehicles, btnSelectAll, btnDeselectAll,
                lblEnvoi, chkEmail, pnlEmail, chkSms, pnlSms
            });

            // ── Footer ────────────────────────────────────────────────────
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Height = 60;
            pnlFooter.BackColor = AppTheme.Surface;

            btnExportForm.Text = "📊  Exporter";
            btnExportForm.Size = new Size(130, 38);
            btnExportForm.Location = new Point(240, 11);
            btnExportForm.Click += BtnExportForm_Click;

            btnCancel.Text = "Annuler";
            btnCancel.Size = new Size(100, 38);
            btnCancel.Location = new Point(380, 11);
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            pnlFooter.Controls.AddRange(new Control[] { btnExportForm, btnCancel });
            this.Controls.AddRange(new Control[] { pnlContent, pnlFooter, pnlHeader });
            this.AcceptButton = btnExportForm;
        }

        private void BtnExportForm_Click(object? sender, EventArgs e)
        {
            var selected = new List<Vehicle>();
            for (int i = 0; i < clbVehicles.CheckedIndices.Count; i++)
                selected.Add(_vehicles[clbVehicles.CheckedIndices[i]]);

            if (!selected.Any())
            {
                MessageBox.Show("Veuillez sélectionner au moins un véhicule.",
                    "Aucun véhicule", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (chkEmail.Checked && string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Veuillez saisir un email destinataire.",
                    "Champ manquant", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectedVehicles = selected;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}