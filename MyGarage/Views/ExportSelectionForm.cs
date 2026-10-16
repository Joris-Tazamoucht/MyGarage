using System.Net.Mail;
using Models.Models;
using MyGarage.Controls;
using MyGarage.Styles;

namespace MyGarage.Views
{
    public partial class ExportSelectionForm : DialogBase
    {
        private readonly List<Vehicle> _vehicles;
        private readonly List<CheckRow> _rows = new();
        private readonly ThemedLabel lblCount = new() { Role = TextRole.Muted, Font = AppTheme.FontSmall };
        private readonly CheckRow chkEmail = new() { Tile = true, Text = "Envoyer par email", Detail = "Le rapport Excel sera joint au message" };
        private readonly InputBox txtEmail = new() { PlaceholderText = "destinataire@exemple.fr" };
        private readonly CheckRow chkSms = new() { Tile = true, Text = "Envoyer un SMS (Free Mobile)", Detail = "Identifiants lus depuis appsettings.json" };
        private ThemedButton? btnExport;

        public List<Vehicle> SelectedVehicles { get; private set; } = new();
        public bool SendEmail => chkEmail.Checked;
        public bool SendSms => chkSms.Checked;
        public string Email => txtEmail.Text.Trim();

        public ExportSelectionForm(List<Vehicle> vehicles)
        {
            InitializeComponent();
            _vehicles = vehicles;
            BuildUI();
        }

        private void BuildUI()
        {
            SetHeader(Icons.Document, "Exporter un rapport", "Sélectionnez les véhicules et les options d'envoi", AppTheme.SuccessGreen);
            SetDialogWidth(520);
            UseFieldColumns(1);

            AddField(null, BuildSectionHeader(), bottomGap: S(8));
            AddField(null, BuildVehicleList(), bottomGap: S(22));
            AddField(null, new ThemedLabel { Text = "Options d'envoi", Font = AppTheme.FontSubtitle }, bottomGap: S(10));
            AddField(null, chkEmail, bottomGap: S(8));
            AddField(null, txtEmail);
            txtEmail.Margin = new Padding(S(30), 0, 0, S(10));
            txtEmail.Visible = false;
            AddField(null, chkSms, bottomGap: S(4));

            chkEmail.CheckedChanged += (s, e) =>
            {
                txtEmail.Visible = chkEmail.Checked;
                RefitHeight();
                if (chkEmail.Checked) txtEmail.Inner.Focus();
            };

            btnExport = AddFooterButton("Exporter", ButtonVariant.Success, Icons.Download, BtnExport_Click);
            var cancel = AddFooterButton("Annuler", ButtonVariant.Secondary, null,
                (s, e) => { DialogResult = DialogResult.Cancel; Close(); });
            AcceptButton = btnExport;
            CancelButton = cancel;

            UpdateCount();
        }

        private Control BuildSectionHeader()
        {
            var panel = new Panel { Height = S(34) };
            var title = new ThemedLabel { Text = "Véhicules", Font = AppTheme.FontSubtitle, Location = new Point(0, S(6)) };
            var all = new ThemedButton { Text = "Tout", Variant = ButtonVariant.Ghost, Height = S(30), Font = AppTheme.FontSmallBold };
            var none = new ThemedButton { Text = "Aucun", Variant = ButtonVariant.Ghost, Height = S(30), Font = AppTheme.FontSmallBold };
            all.FitWidth();
            none.FitWidth();
            all.Click += (s, e) => SetAll(true);
            none.Click += (s, e) => SetAll(false);

            panel.Controls.AddRange(new Control[] { title, lblCount, all, none });
            panel.Layout += (s, e) =>
            {
                lblCount.Location = new Point(title.Right + S(8), title.Top + (title.Height - lblCount.Height) / 2 + S(1));
                none.Location = new Point(panel.Width - none.Width, S(2));
                all.Location = new Point(none.Left - all.Width - S(4), S(2));
            };
            return panel;
        }

        private Control BuildVehicleList()
        {
            var card = new CardPanel { Height = S(200), Padding = new Padding(S(6)) };
            var scroller = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            scroller.HandleCreated += (s, e) => NativeTheme.ApplyScrollBars(scroller);

            if (_vehicles.Count == 0)
            {
                scroller.Controls.Add(new ThemedLabel
                {
                    Text = "Aucun véhicule enregistré.",
                    Role = TextRole.Muted,
                    AutoSize = false,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                });
            }

            // Dock = Top : on ajoute à l'envers pour que le premier véhicule soit en haut
            for (int i = _vehicles.Count - 1; i >= 0; i--)
            {
                var v = _vehicles[i];
                var row = new CheckRow
                {
                    Text = $"{v.Marque} {v.Modele}",
                    Detail = v.Immatriculation,
                    DetailInline = true,
                    Dock = DockStyle.Top
                };
                row.CheckedChanged += (s, e) => UpdateCount();
                scroller.Controls.Add(row);
                _rows.Insert(0, row);
            }

            card.Controls.Add(scroller);
            return card;
        }

        private void SetAll(bool value)
        {
            foreach (var row in _rows) row.Checked = value;
        }

        private void UpdateCount()
        {
            int n = _rows.Count(r => r.Checked);
            lblCount.Text = $"{n} / {_rows.Count} sélectionné{(n > 1 ? "s" : "")}";
            if (btnExport != null) btnExport.Enabled = n > 0;
        }

        private void BtnExport_Click(object? sender, EventArgs e)
        {
            ClearError();
            var selected = _rows.Select((row, i) => (row, i)).Where(x => x.row.Checked).Select(x => _vehicles[x.i]).ToList();

            if (selected.Count == 0) { ShowError("Sélectionnez au moins un véhicule."); return; }
            if (chkEmail.Checked && !MailAddress.TryCreate(Email, out _))
            {
                ShowError("Adresse email invalide.", txtEmail);
                return;
            }

            SelectedVehicles = selected;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
