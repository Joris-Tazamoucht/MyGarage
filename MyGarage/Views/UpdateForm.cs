using MyGarage.Controls;
using MyGarage.Styles;
using Service.Services;

namespace MyGarage.Views
{
    public partial class UpdateForm : DialogBase
    {
        private readonly UpdateInfo _update;
        private readonly UpdateService _updateService = new();
        private readonly RichTextBox rtbNotes = new();
        private readonly Panel pnlProgress = new() { Height = S(40), Visible = false };
        private readonly ThemedProgressBar progressBar = new();
        private readonly ThemedLabel lblProgress = new() { Role = TextRole.Secondary, Font = AppTheme.FontSmall };
        private ThemedButton btnUpdate = null!;
        private ThemedButton btnLater = null!;

        public UpdateForm(UpdateInfo update)
        {
            InitializeComponent();
            _update = update;
            BuildUI();
        }

        private void BuildUI()
        {
            SetHeader(Icons.Sync, "Mise à jour disponible",
                $"Version {_update.CurrentVersion}  →  {_update.NewVersion}", accentSubtitle: true);
            SetDialogWidth(540);
            UseFieldColumns(1);

            // Notes de version dans une carte
            var notesCard = new CardPanel { Height = S(210), Padding = new Padding(S(14), S(12), S(6), S(12)) };
            rtbNotes.Dock = DockStyle.Fill;
            rtbNotes.BorderStyle = BorderStyle.None;
            rtbNotes.ReadOnly = true;
            rtbNotes.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbNotes.Font = AppTheme.FontNormal;
            rtbNotes.BackColor = AppTheme.Surface;
            rtbNotes.ForeColor = AppTheme.TextPrimary;
            rtbNotes.Text = _update.ReleaseNotes ?? "";
            rtbNotes.HandleCreated += (s, e) => NativeTheme.ApplyScrollBars(rtbNotes);
            notesCard.Controls.Add(rtbNotes);
            AddField("Notes de version", notesCard);

            // Progression (masquée tant que le téléchargement n'a pas commencé)
            progressBar.Location = new Point(0, S(2));
            progressBar.Width = S(200);
            progressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblProgress.Location = new Point(0, S(16));
            pnlProgress.Width = S(200);
            pnlProgress.Controls.AddRange(new Control[] { progressBar, lblProgress });
            AddField(null, pnlProgress, bottomGap: S(4));

            btnUpdate = AddFooterButton("Télécharger et installer", ButtonVariant.Primary, Icons.Download, BtnUpdate_Click);
            btnLater = AddFooterButton("Plus tard", ButtonVariant.Secondary, null, (s, e) => Close());
            AcceptButton = btnUpdate;
            CancelButton = btnLater;
        }

        private async void BtnUpdate_Click(object? sender, EventArgs e)
        {
            ClearError();
            btnUpdate.Enabled = false;
            btnLater.Enabled = false;
            pnlProgress.Visible = true;
            RefitHeight();
            lblProgress.Text = "Téléchargement en cours…";

            try
            {
                var progress = new Progress<int>(p =>
                {
                    progressBar.Value = p;
                    lblProgress.Text = $"Téléchargement : {p} %";
                });

                string installerPath = await _updateService.DownloadUpdateAsync(_update, progress);
                lblProgress.Text = "Lancement de l'installation…";

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = installerPath,
                    UseShellExecute = true
                });
                Application.Exit();
            }
            catch (Exception ex)
            {
                ShowError($"Échec du téléchargement : {ex.Message}");
                btnUpdate.Enabled = true;
                btnLater.Enabled = true;
                pnlProgress.Visible = false;
                RefitHeight();
            }
        }
    }
}
