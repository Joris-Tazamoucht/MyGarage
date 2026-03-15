using Models.Models;
using MyGarage.Styles;

namespace MyGarage.Views
{
    public partial class AddEntretienForm : Form
    {
        private readonly int _vehicleId;
        private Panel pnlHeader = new Panel();
        private Panel pnlContent = new Panel();
        private Panel pnlFooter = new Panel();
        private Label lblTitle = new Label();
        private Label lblSubtitle = new Label();

        private DateTimePicker dtpDate = new DateTimePicker();
        private ComboBox cboType = new ComboBox();
        private TextBox txtKilometrage = new TextBox();
        private TextBox txtCout = new TextBox();
        private TextBox txtNotes = new TextBox();

        private ModernButton btnConfirm = new ModernButton();
        private ModernButton btnCancel = new ModernButton(Color.FromArgb(80, 80, 100), Color.FromArgb(60, 60, 80));

        public Entretien? Entretien { get; private set; }

        public AddEntretienForm(int vehicleId, string vehicleLabel)
        {
            InitializeComponent();
            _vehicleId = vehicleId;
            BuildUI(vehicleLabel);
            AppTheme.ThemeChanged += () => AppTheme.ApplyToForm(this);
        }

        private void BuildUI(string vehicleLabel)
        {
            this.Text = "Ajouter un entretien";
            this.Size = new Size(460, 540);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.BackColor = AppTheme.Background;

            // ── Header ────────────────────────────────────────────────────
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 70;
            pnlHeader.BackColor = AppTheme.Surface;

            lblTitle.Text = "🔧  Ajouter un entretien";
            lblTitle.Font = AppTheme.FontTitle;
            lblTitle.ForeColor = AppTheme.TextPrimary;
            lblTitle.BackColor = AppTheme.Surface;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 12);

            lblSubtitle.Text = vehicleLabel;
            lblSubtitle.Font = AppTheme.FontSmall;
            lblSubtitle.ForeColor = AppTheme.AccentBlueLight;
            lblSubtitle.BackColor = AppTheme.Surface;
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(20, 42);

            pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblSubtitle });

            // ── Contenu ───────────────────────────────────────────────────
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = AppTheme.Background;
            pnlContent.Padding = new Padding(20, 16, 20, 0);

            cboType.Items.AddRange(new string[]
            {
                "Vidange", "Révision", "Freins", "Pneus", "Courroie",
                "Filtres", "Batterie", "Amortisseurs", "Autre"
            });
            cboType.DropDownStyle = ComboBoxStyle.DropDown;
            cboType.BackColor = AppTheme.Surface;
            cboType.ForeColor = AppTheme.TextPrimary;
            cboType.FlatStyle = FlatStyle.Flat;

            dtpDate.Format = DateTimePickerFormat.Short;
            dtpDate.Value = DateTime.Today;
            dtpDate.Font = AppTheme.FontNormal;

            txtNotes.Multiline = true;
            txtNotes.Height = 60;

            int y = 16;
            pnlContent.Controls.Add(MakeField("Date", dtpDate, ref y));
            pnlContent.Controls.Add(MakeField("Type *", cboType, ref y));
            pnlContent.Controls.Add(MakeField("Kilométrage", txtKilometrage, ref y));
            pnlContent.Controls.Add(MakeField("Coût (€)", txtCout, ref y));
            pnlContent.Controls.Add(MakeFieldTall("Notes", txtNotes, ref y));

            // ── Footer ────────────────────────────────────────────────────
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Height = 60;
            pnlFooter.BackColor = AppTheme.Surface;

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

        private Panel MakeField(string label, Control ctrl, ref int y)
        {
            var panel = new Panel { Location = new Point(0, y), Size = new Size(400, 54), BackColor = Color.Transparent };
            var lbl = new Label { Text = label, Font = AppTheme.FontSmall, ForeColor = AppTheme.TextSub, AutoSize = true, Location = new Point(0, 0) };
            ctrl.Font = AppTheme.FontNormal;
            ctrl.Size = new Size(400, 28);
            ctrl.Location = new Point(0, 20);
            if (ctrl is TextBox tb) { tb.BackColor = AppTheme.Surface; tb.ForeColor = AppTheme.TextPrimary; tb.BorderStyle = BorderStyle.FixedSingle; }
            panel.Controls.AddRange(new Control[] { lbl, ctrl });
            y += 58;
            return panel;
        }

        private Panel MakeFieldTall(string label, TextBox txt, ref int y)
        {
            var panel = new Panel { Location = new Point(0, y), Size = new Size(400, 84), BackColor = Color.Transparent };
            var lbl = new Label { Text = label, Font = AppTheme.FontSmall, ForeColor = AppTheme.TextSub, AutoSize = true, Location = new Point(0, 0) };
            txt.Font = AppTheme.FontNormal;
            txt.BackColor = AppTheme.Surface;
            txt.ForeColor = AppTheme.TextPrimary;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Size = new Size(400, 60);
            txt.Location = new Point(0, 20);
            panel.Controls.AddRange(new Control[] { lbl, txt });
            y += 88;
            return panel;
        }

        private void BtnConfirm_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cboType.Text))
            {
                MessageBox.Show("Le type d'entretien est obligatoire.", "Champ manquant", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(txtKilometrage.Text, out int km))
            {
                MessageBox.Show("Le kilométrage doit être un nombre entier.", "Valeur invalide", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!float.TryParse(txtCout.Text.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out float cout))
            {
                MessageBox.Show("Le coût doit être un nombre (ex: 149.90).", "Valeur invalide", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Entretien = new Entretien
            {
                vehicle_id = _vehicleId,
                date_etretien = dtpDate.Value.ToString("yyyy-MM-dd"),
                type_entretien = cboType.Text.Trim(),
                kilometrage = km,
                cout = cout,
                notes = txtNotes.Text.Trim()
            };
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}