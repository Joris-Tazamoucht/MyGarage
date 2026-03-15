using Service.Services;

namespace MyGarage.Views
{
    public partial class UpdateForm : Form
    {
        private readonly UpdateInfo _update;
        private readonly UpdateService _updateService;
        private ProgressBar progressBar = new ProgressBar();
        private Label lblInfo = new Label();
        private Label lblNotes = new Label();
        private RichTextBox rtbNotes = new RichTextBox();
        private Button btnUpdate = new Button();
        private Button btnLater = new Button();
        private Label lblProgress = new Label();

        public UpdateForm(UpdateInfo update)
        {
            InitializeComponent();
            _update = update;
            _updateService = new UpdateService();

            this.Text = "Mise à jour disponible";
            this.Size = new Size(500, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;

            lblInfo.Text = $"Une nouvelle version est disponible !\n\nVersion actuelle : {update.CurrentVersion}\nNouvelle version : {update.NewVersion}";
            lblInfo.Font = new Font("Segoe UI", 10F);
            lblInfo.Location = new Point(12, 12);
            lblInfo.Size = new Size(460, 70);

            lblNotes.Text = "Notes de version :";
            lblNotes.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNotes.AutoSize = true;
            lblNotes.Location = new Point(12, 90);

            rtbNotes.Text = update.ReleaseNotes;
            rtbNotes.Location = new Point(12, 112);
            rtbNotes.Size = new Size(460, 150);
            rtbNotes.ReadOnly = true;
            rtbNotes.BackColor = SystemColors.Window;
            rtbNotes.ScrollBars = RichTextBoxScrollBars.Vertical;

            progressBar.Location = new Point(12, 275);
            progressBar.Size = new Size(460, 23);
            progressBar.Visible = false;

            lblProgress.Text = string.Empty;
            lblProgress.AutoSize = true;
            lblProgress.Location = new Point(12, 303);

            btnUpdate.Text = "Télécharger et installer";
            btnUpdate.Size = new Size(180, 35);
            btnUpdate.Location = new Point(190, 325);
            btnUpdate.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnUpdate.Click += BtnUpdate_Click;

            btnLater.Text = "Plus tard";
            btnLater.Size = new Size(100, 35);
            btnLater.Location = new Point(382, 325);
            btnLater.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[]
            {
                lblInfo, lblNotes, rtbNotes,
                progressBar, lblProgress,
                btnUpdate, btnLater
            });
        }

        private async void BtnUpdate_Click(object? sender, EventArgs e)
        {
            btnUpdate.Enabled = false;
            btnLater.Enabled = false;
            progressBar.Visible = true;
            lblProgress.Text = "Téléchargement en cours...";

            try
            {
                var progress = new Progress<int>(p =>
                {
                    progressBar.Value = p;
                    lblProgress.Text = $"Téléchargement : {p}%";
                });

                string installerPath = await _updateService.DownloadUpdateAsync(_update, progress);

                lblProgress.Text = "Lancement de l'installation...";

                // Lancer le nouvel exe et fermer l'app actuelle
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = installerPath,
                    UseShellExecute = true
                });

                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du téléchargement : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnUpdate.Enabled = true;
                btnLater.Enabled = true;
                progressBar.Visible = false;
            }
        }
    }
}