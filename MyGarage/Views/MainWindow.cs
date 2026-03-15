using BackgroundWorker;
using Manager.Manager;
using Models.Models;
using MyGarage.Styles;
using Repository.Repository;
using Service.Abstractions;
using Service.Services;
using System.Reflection;

namespace MyGarage.Views
{
    public partial class MainWindow : Form
    {
        private readonly IVehicleService _vehicleService;
        private readonly MaintenanceCheckerService _maintenanceChecker;

        // ── Layout panels ─────────────────────────────────────────────────
        private Panel pnlSideBar = new Panel();
        private Panel pnlHeader = new Panel();
        private Panel pnlContent = new Panel();
        private Panel pnlVehicles = new Panel();
        private Panel pnlEntretiens = new Panel();

        // ── Header ────────────────────────────────────────────────────────
        private Label lblAppTitle = new Label();
        private Label lblVersion = new Label();
        private ModernButton btnTheme = new ModernButton();

        // ── Sidebar ───────────────────────────────────────────────────────
        private ModernButton btnAddVehicle = new ModernButton();
        private ModernButton btnAddEntretien = new ModernButton();
        private ModernButton btnHistory = new ModernButton();
        private ModernButton btnExportBtn = new ModernButton(AppTheme.SuccessGreen, Color.FromArgb(30, 140, 70));

        // ── Grilles ───────────────────────────────────────────────────────
        private DataGridView dgvVehicles = new DataGridView();
        private DataGridView dgvEntretiens = new DataGridView();
        private Label lblVehiclesTitle = new Label();
        private Label lblEntretiensTitle = new Label();

        public MainWindow()
        {
            InitializeComponent();
            BuildUI();
            WireEvents();

            string dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "MyGarage", "MyGarage.db");
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
            string connectionString = $"Data Source={dbPath}";

            var repository = new MyGarageRepository(connectionString);
            var manager = new MyGarageManager(repository);
            _vehicleService = new VehicleService(manager);

            _maintenanceChecker = new MaintenanceCheckerService(manager, intervalMinutes: 60);
            _maintenanceChecker.MaintenanceAlert += msg =>
            {
                if (this.IsHandleCreated)
                    this.Invoke(() => MessageBox.Show(msg, "Entretien à prévoir",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning));
            };

            AppTheme.ThemeChanged += ApplyTheme;
            _ = LoadVehiclesAsync();
        }

        private void BuildUI()
        {
            this.Text = "MyGarage";
            this.Size = new Size(1300, 700);
            this.MinimumSize = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "mygarage.ico"));

            // ── Header ────────────────────────────────────────────────────
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 60;
            pnlHeader.Padding = new Padding(16, 0, 16, 0);

            lblAppTitle.Text = "🚗  MyGarage";
            lblAppTitle.Font = AppTheme.FontTitle;
            lblAppTitle.AutoSize = true;
            lblAppTitle.Location = new Point(16, 16);

            lblVersion.AutoSize = true;
            lblVersion.Font = AppTheme.FontSmall;
            lblVersion.Location = new Point(16, 40);

            btnTheme.Text = "🌙  Dark";
            btnTheme.Size = new Size(110, 34);
            btnTheme.Anchor = AnchorStyles.Right | AnchorStyles.Top;

            pnlHeader.Controls.AddRange(new Control[] { lblAppTitle, lblVersion, btnTheme });

            // ── Sidebar ───────────────────────────────────────────────────
            pnlSideBar.Dock = DockStyle.Left;
            pnlSideBar.Width = 200;
            pnlSideBar.Padding = new Padding(12, 20, 12, 20);

            var sideLabel = new Label
            {
                Text = "ACTIONS",
                Font = AppTheme.FontSmall,
                AutoSize = true,
                Location = new Point(12, 12)
            };

            btnAddVehicle.Text = "➕  Ajouter véhicule";
            btnAddVehicle.Size = new Size(176, 42);
            btnAddVehicle.Location = new Point(12, 40);

            btnAddEntretien.Text = "🔧  Ajouter entretien";
            btnAddEntretien.Size = new Size(176, 42);
            btnAddEntretien.Location = new Point(12, 94);

            btnHistory.Text = "📋  Historique";
            btnHistory.Size = new Size(176, 42);
            btnHistory.Location = new Point(12, 148);
            btnHistory.SetBaseColor(Color.FromArgb(80, 80, 120), Color.FromArgb(60, 60, 100));

            btnExportBtn.Text = "📊  Exporter rapport";
            btnExportBtn.Size = new Size(176, 42);
            btnExportBtn.Location = new Point(12, 202);

            pnlSideBar.Controls.AddRange(new Control[]
            {
                sideLabel, btnAddVehicle, btnAddEntretien, btnHistory, btnExportBtn
            });

            // ── Contenu principal ─────────────────────────────────────────
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Padding = new Padding(16);

            // Panel véhicules
            pnlVehicles.Dock = DockStyle.Left;
            pnlVehicles.Width = 550;
            pnlVehicles.Padding = new Padding(0, 0, 12, 0);

            lblVehiclesTitle.Text = "🚗  Véhicules";
            lblVehiclesTitle.Font = AppTheme.FontSubtitle;
            lblVehiclesTitle.AutoSize = true;
            lblVehiclesTitle.Dock = DockStyle.Top;
            lblVehiclesTitle.Height = 30;

            dgvVehicles.Dock = DockStyle.Fill;
            AppTheme.ApplyToDataGridView(dgvVehicles);

            pnlVehicles.Controls.Add(dgvVehicles);
            pnlVehicles.Controls.Add(lblVehiclesTitle);

            // Panel entretiens
            pnlEntretiens.Dock = DockStyle.Fill;
            pnlEntretiens.Padding = new Padding(12, 0, 0, 0);

            lblEntretiensTitle.Text = "🔧  Entretiens récents";
            lblEntretiensTitle.Font = AppTheme.FontSubtitle;
            lblEntretiensTitle.AutoSize = true;
            lblEntretiensTitle.Dock = DockStyle.Top;
            lblEntretiensTitle.Height = 30;

            dgvEntretiens.Dock = DockStyle.Fill;
            AppTheme.ApplyToDataGridView(dgvEntretiens);

            pnlEntretiens.Controls.Add(dgvEntretiens);
            pnlEntretiens.Controls.Add(lblEntretiensTitle);

            pnlContent.Controls.Add(pnlEntretiens);
            pnlContent.Controls.Add(pnlVehicles);

            // ── Assemblage ────────────────────────────────────────────────
            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlSideBar);
            this.Controls.Add(pnlHeader);

            ApplyTheme();
        }

        private void WireEvents()
        {
            this.Load += MainWindow_Load;
            btnTheme.Click += (s, e) => ToggleTheme();
            btnAddVehicle.Click += button1_Click;
            btnAddEntretien.Click += button2_Click;
            btnHistory.Click += button3_Click;
            btnExportBtn.Click += btnExport_Click;
            dgvVehicles.SelectionChanged += dataGridView2_SelectionChanged;
        }

        private void ApplyTheme()
        {
            this.BackColor = AppTheme.Background;
            pnlHeader.BackColor = AppTheme.Surface;
            pnlSideBar.BackColor = AppTheme.SideBar;
            pnlContent.BackColor = AppTheme.Background;
            pnlVehicles.BackColor = AppTheme.Background;
            pnlEntretiens.BackColor = AppTheme.Background;

            lblAppTitle.ForeColor = AppTheme.TextPrimary;
            lblAppTitle.BackColor = AppTheme.Surface;
            lblVersion.ForeColor = AppTheme.TextSub;
            lblVersion.BackColor = AppTheme.Surface;

            lblVehiclesTitle.ForeColor = AppTheme.TextPrimary;
            lblVehiclesTitle.BackColor = AppTheme.Background;
            lblEntretiensTitle.ForeColor = AppTheme.TextPrimary;
            lblEntretiensTitle.BackColor = AppTheme.Background;

            foreach (Control c in pnlSideBar.Controls)
                if (c is Label l) { l.ForeColor = AppTheme.TextSub; l.BackColor = AppTheme.SideBar; }

            AppTheme.ApplyToDataGridView(dgvVehicles);
            AppTheme.ApplyToDataGridView(dgvEntretiens);

            btnTheme.Text = AppTheme.IsDark ? "☀️  Light" : "🌙  Dark";

            // Repositionner btnTheme à droite
            btnTheme.Location = new Point(pnlHeader.Width - btnTheme.Width - 16, 13);

            this.Refresh();
        }

        private void ToggleTheme()
        {
            AppTheme.Toggle();
            ApplyTheme();
        }

        private void MainWindow_Load(object? sender, EventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version is not null)
                lblVersion.Text = $"v{version.Major}.{version.Minor}.{version.Build}";

            btnTheme.Location = new Point(pnlHeader.Width - btnTheme.Width - 16, 13);
            _maintenanceChecker.Start();
            _ = CheckForUpdatesAsync();
        }

        private async Task LoadVehiclesAsync()
        {
            try
            {
                var vehicles = await _vehicleService.GetAllVehiclesAsync();
                dgvVehicles.DataSource = vehicles;
                if (dgvVehicles.Columns["ID"] != null)
                    dgvVehicles.Columns["ID"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            using var form = new AddVehicleForm();
            if (form.ShowDialog(this) != DialogResult.OK || form.Vehicle == null) return;
            try
            {
                var added = await _vehicleService.AddVehicleAsync(form.Vehicle);
                if (added == null) { MessageBox.Show("Erreur lors de l'ajout.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                MessageBox.Show($"{added.Marque} {added.Modele} ajouté !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadVehiclesAsync();
            }
            catch (Exception ex) { MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (dgvVehicles.CurrentRow?.DataBoundItem is not Vehicle selectedVehicle)
            {
                MessageBox.Show("Veuillez sélectionner un véhicule.", "Aucun véhicule", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string vehicleLabel = $"{selectedVehicle.Marque} {selectedVehicle.Modele} ({selectedVehicle.Immatriculation})";
            using var form = new AddEntretienForm(selectedVehicle.ID, vehicleLabel);
            if (form.ShowDialog(this) != DialogResult.OK || form.Entretien == null) return;
            try
            {
                var added = await _vehicleService.AddEntretienAsync(form.Entretien);
                if (added == null) { MessageBox.Show("Erreur lors de l'ajout.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                MessageBox.Show($"Entretien '{added.type_entretien}' ajouté !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (dgvVehicles.CurrentRow?.DataBoundItem is not Vehicle selectedVehicle)
            {
                MessageBox.Show("Veuillez sélectionner un véhicule.", "Aucun véhicule", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (selectedVehicle.Immatriculation is null) return;
            try
            {
                var history = await _vehicleService.GetHistVehicleAsync(selectedVehicle.Immatriculation);
                if (history == null) { MessageBox.Show("Aucun historique trouvé.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                using var form = new VehicleHistoryForm(history);
                form.ShowDialog(this);
            }
            catch (Exception ex) { MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private async void dataGridView2_SelectionChanged(object? sender, EventArgs args)
        {
            if (dgvVehicles.CurrentRow?.DataBoundItem is not Vehicle selectedVehicle) return;
            if (selectedVehicle.Immatriculation is null) return;
            try
            {
                var history = await _vehicleService.GetHistVehicleAsync(selectedVehicle.Immatriculation);
                if (history == null || history.Historique == null || !history.Historique.Any())
                {
                    dgvEntretiens.DataSource = null;
                    return;
                }
                var rows = history.Historique.Select(ent => new
                {
                    Date = DateTime.TryParse(ent.date_etretien, out var d) ? d.ToString("dd/MM/yyyy") : ent.date_etretien,
                    Type = ent.type_entretien,
                    Kilométrage = ent.kilometrage?.ToString("N0") + " km",
                    Coût = ent.cout?.ToString("N2") + " €",
                }).ToList();
                dgvEntretiens.DataSource = rows;
            }
            catch (Exception ex) { MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private async void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var config = AppConfig.Load();
                var allVehicles = await _vehicleService.GetAllVehiclesAsync();
                using var selectionForm = new ExportSelectionForm(allVehicles);
                if (selectionForm.ShowDialog(this) != DialogResult.OK) return;
                using var folderDialog = new FolderBrowserDialog { Description = "Choisissez où enregistrer le rapport Excel" };
                if (folderDialog.ShowDialog(this) != DialogResult.OK) return;
                var histories = new List<VehicleHistory>();
                foreach (var vehicle in selectionForm.SelectedVehicles)
                {
                    if (vehicle.Immatriculation is null) continue;
                    var history = await _vehicleService.GetHistVehicleAsync(vehicle.Immatriculation);
                    if (history != null) histories.Add(history);
                }
                if (!histories.Any()) { MessageBox.Show("Aucun historique trouvé.", "Aucune donnée", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                var rapportService = new RapportService();
                string filePath = rapportService.GenererRapport(histories, folderDialog.SelectedPath);
                if (selectionForm.SendEmail)
                {
                    var notifService = new NotificationService();
                    await notifService.SendEmailAsync(config.Smtp.Host, config.Smtp.Port, config.Smtp.User, config.Smtp.Password, selectionForm.Email, "Rapport MyGarage", "Veuillez trouver en pièce jointe votre rapport.", filePath);
                }
                if (selectionForm.SendSms)
                {
                    var notifService = new NotificationService();
                    await notifService.SendSmsAsync(config.FreeMobile.UserId, config.FreeMobile.ApiKey, $"MyGarage : rapport généré pour {selectionForm.SelectedVehicles.Count} véhicule(s).");
                }
                MessageBox.Show($"Rapport généré !\n{filePath}", "Export terminé", MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = filePath, UseShellExecute = true });
            }
            catch (Exception ex) { MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                if (currentVersion == null) return;
                var updateService = new UpdateService();
                var update = await updateService.CheckForUpdateAsync(currentVersion);
                if (update == null) return;
                this.Invoke(() => { using var form = new UpdateForm(update); form.ShowDialog(this); });
            }
            catch { }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _maintenanceChecker?.Dispose();
            base.OnFormClosing(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (btnTheme != null && pnlHeader != null)
                btnTheme.Location = new Point(pnlHeader.Width - btnTheme.Width - 16, 13);
        }
    }
}