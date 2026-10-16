using BackgroundWorker;
using Manager.Manager;
using Models.Models;
using MyGarage.Controls;
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
        private int _selectionVersion;

        private static int S(int value) => AppTheme.S(value);

        // ── Structure ─────────────────────────────────────────────────────
        private readonly ThemedPanel pnlSideBar = new() { Role = SurfaceRole.SideBar, Edges = AnchorStyles.Right };
        private readonly ThemedPanel pnlContent = new() { Role = SurfaceRole.Background };

        // ── Barre latérale ────────────────────────────────────────────────
        private readonly ThemedLabel lblVersion = new() { Role = TextRole.Muted, Font = AppTheme.FontSmall };
        private readonly ThemedButton btnDashboard = new() { Variant = ButtonVariant.Nav, Glyph = Icons.Home, Text = "Tableau de bord", Active = true };
        private readonly ThemedButton btnExport = new() { Variant = ButtonVariant.Nav, Glyph = Icons.Document, Text = "Exporter un rapport" };
        private readonly ThemedButton btnTheme = new() { Variant = ButtonVariant.Nav };

        // ── Contenu ───────────────────────────────────────────────────────
        private readonly ThemedButton btnAddVehicle = new() { Variant = ButtonVariant.Primary, Glyph = Icons.Add, Text = "Nouveau véhicule" };
        private readonly ThemedButton btnAddEntretien = new() { Variant = ButtonVariant.Secondary, Glyph = Icons.Add, Text = "Ajouter" };
        private readonly ThemedButton btnHistory = new() { Variant = ButtonVariant.Ghost, Glyph = Icons.History, Text = "Historique complet" };

        private readonly StatCard statVehicles = new(Icons.Car, AppTheme.AccentBlue, "Véhicules");
        private readonly StatCard statEntretiens = new(Icons.Repair, AppTheme.WarningOrange, "Entretiens");
        private readonly StatCard statDepenses = new(Icons.Money, AppTheme.SuccessGreen, "Dépenses totales");
        private readonly StatCard statDernier = new(Icons.Calendar, AppTheme.Purple, "Dernier entretien");

        private readonly ThemedLabel lblVehiclesSub = new() { Role = TextRole.Muted, AutoSize = false, AutoEllipsis = true };
        private readonly ThemedLabel lblEntretiensSub = new() { Role = TextRole.Muted, AutoSize = false, AutoEllipsis = true };
        private readonly ThemedGrid dgvVehicles = new()
        {
            Dock = DockStyle.Fill,
            EmptyText = "Aucun véhicule pour l'instant.\nCliquez sur « Nouveau véhicule » pour commencer."
        };
        private readonly ThemedGrid dgvEntretiens = new() { Dock = DockStyle.Fill };

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
                if (IsHandleCreated && !IsDisposed)
                    BeginInvoke(new Action(() => MessageBox.Show(this, msg, "Entretien à prévoir",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)));
            };
        }

        // ══════════════════════════════════════════════════════════════════
        //  Construction de l'interface
        // ══════════════════════════════════════════════════════════════════
        private void BuildUI()
        {
            Text = "MyGarage";
            Size = new Size(S(1320), S(800));
            MinimumSize = new Size(S(1060), S(660));
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable;

            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "mygarage.ico");
            if (File.Exists(iconPath)) Icon = new Icon(iconPath);

            BuildSideBar();
            BuildContent();

            Controls.Add(pnlContent);
            Controls.Add(pnlSideBar);

            dgvVehicles.AddPillColumn("Immatriculation", colored: false);
            dgvEntretiens.AddPillColumn("Type", colored: true);
            ShowNoSelection();
            ApplyTheme();
        }

        private void BuildSideBar()
        {
            pnlSideBar.Dock = DockStyle.Left;
            pnlSideBar.Width = S(236);
            pnlSideBar.Padding = new Padding(S(16), S(20), S(16), S(16));

            // Logo + nom + version
            var brand = new Panel { Dock = DockStyle.Top, Height = S(52) };
            var logo = new IconBadge { Glyph = Icons.Car, GlyphSize = 15f, Size = new Size(S(40), S(40)), Location = new Point(0, S(4)) };
            var title = new ThemedLabel { Text = "MyGarage", Font = AppTheme.FontSubtitle, Location = new Point(S(52), S(4)) };
            lblVersion.Location = new Point(S(53), S(27));
            brand.Controls.AddRange(new Control[] { logo, title, lblVersion });

            var menuLabel = new ThemedLabel
            {
                Text = "MENU",
                Role = TextRole.Muted,
                Font = AppTheme.FontSmallBold,
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = S(46),
                TextAlign = ContentAlignment.BottomLeft,
                Padding = new Padding(S(4), 0, 0, S(8))
            };

            var nav = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            foreach (var b in new[] { btnDashboard, btnExport })
            {
                b.Size = new Size(S(204), S(40));
                b.Margin = new Padding(0, 0, 0, S(4));
                nav.Controls.Add(b);
            }

            btnTheme.Dock = DockStyle.Bottom;
            btnTheme.Height = S(40);

            // Docking : le dernier ajouté se place en premier
            pnlSideBar.Controls.Add(btnTheme);
            pnlSideBar.Controls.Add(nav);
            pnlSideBar.Controls.Add(menuLabel);
            pnlSideBar.Controls.Add(brand);
        }

        private void BuildContent()
        {
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Padding = new Padding(S(28), S(24), S(28), S(24));

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Margin = Padding.Empty };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, S(78)));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, S(96 + 20)));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            root.Controls.Add(BuildPageHeader(), 0, 0);
            root.Controls.Add(BuildStatsRow(), 0, 1);
            root.Controls.Add(BuildGridsRow(), 0, 2);
            pnlContent.Controls.Add(root);
        }

        private Control BuildPageHeader()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty };
            var title = new ThemedLabel { Text = "Tableau de bord", Font = AppTheme.FontTitle, Location = new Point(0, 0) };
            var subtitle = new ThemedLabel
            {
                Text = "Suivez l'entretien et les dépenses de vos véhicules",
                Role = TextRole.Secondary,
                Location = new Point(S(1), S(36))
            };
            btnAddVehicle.Height = S(40);
            btnAddVehicle.FitWidth();
            panel.Controls.AddRange(new Control[] { title, subtitle, btnAddVehicle });
            panel.Layout += (s, e) => btnAddVehicle.Location = new Point(panel.Width - btnAddVehicle.Width, S(4));
            return panel;
        }

        private Control BuildStatsRow()
        {
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Margin = new Padding(0, 0, 0, S(20))
            };
            for (int i = 0; i < 4; i++) table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var cards = new[] { statVehicles, statEntretiens, statDepenses, statDernier };
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].Dock = DockStyle.Fill;
                cards[i].Margin = new Padding(i == 0 ? 0 : S(8), 0, i == cards.Length - 1 ? 0 : S(8), 0);
                table.Controls.Add(cards[i], i, 0);
            }
            return table;
        }

        private Control BuildGridsRow()
        {
            var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = Padding.Empty };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var vehiclesCard = BuildGridCard("Véhicules", lblVehiclesSub, dgvVehicles, Array.Empty<Control>());
            vehiclesCard.Margin = new Padding(0, 0, S(10), 0);

            btnAddEntretien.Height = S(34);
            btnAddEntretien.FitWidth();
            btnHistory.Height = S(34);
            btnHistory.FitWidth();
            var entretiensCard = BuildGridCard("Entretiens", lblEntretiensSub, dgvEntretiens, new Control[] { btnAddEntretien, btnHistory });
            entretiensCard.Margin = new Padding(S(10), 0, 0, 0);

            table.Controls.Add(vehiclesCard, 0, 0);
            table.Controls.Add(entretiensCard, 1, 0);
            return table;
        }

        /// <summary>Carte avec titre, sous-titre, boutons d'action à droite et une grille.</summary>
        private static CardPanel BuildGridCard(string title, ThemedLabel subtitle, ThemedGrid grid, Control[] actions)
        {
            var card = new CardPanel { Dock = DockStyle.Fill, Padding = new Padding(S(18), S(16), S(18), S(12)) };
            var header = new Panel { Dock = DockStyle.Top, Height = S(60) };
            var lblTitle = new ThemedLabel { Text = title, Font = AppTheme.FontSubtitle, Location = new Point(0, 0) };
            header.Controls.Add(lblTitle);
            header.Controls.Add(subtitle);
            foreach (var action in actions) header.Controls.Add(action);

            header.Layout += (s, e) =>
            {
                int x = header.Width;
                foreach (var action in actions)   // le premier est le plus à droite
                {
                    x -= action.Width;
                    action.Location = new Point(x, S(2));
                    x -= S(8);
                }
                subtitle.SetBounds(S(1), S(25), Math.Max(10, x - S(8)), S(20));
            };

            card.Controls.Add(grid);
            card.Controls.Add(header);
            return card;
        }

        private void WireEvents()
        {
            Load += MainWindow_Load;
            btnTheme.Click += (s, e) => AppTheme.Toggle();
            btnDashboard.Click += async (s, e) => await LoadVehiclesAsync();
            btnAddVehicle.Click += BtnAddVehicle_Click;
            btnAddEntretien.Click += BtnAddEntretien_Click;
            btnHistory.Click += BtnHistory_Click;
            btnExport.Click += BtnExport_Click;
            dgvVehicles.SelectionChanged += DgvVehicles_SelectionChanged;
            dgvVehicles.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) BtnHistory_Click(s, e); };
            dgvVehicles.DataBindingComplete += (s, e) => ConfigureVehicleColumns();
            dgvEntretiens.DataBindingComplete += (s, e) => ConfigureEntretienColumns();
            AppTheme.ThemeChanged += ApplyTheme;
        }

        private void ConfigureVehicleColumns()
        {
            dgvVehicles.SetColumn("ID", visible: false);
            dgvVehicles.SetColumn("Marque", "Marque");
            dgvVehicles.SetColumn("Modele", "Modèle");
            dgvVehicles.SetColumn("Immatriculation", "Immatriculation", fillWeight: 125);
            dgvVehicles.SetColumn("Kilometrage", "Kilométrage", format: "#,##0' km'", alignRight: true);
            dgvVehicles.SetColumn("Annee", "Année", alignRight: true, fillWeight: 60);
        }

        private void ConfigureEntretienColumns()
        {
            dgvEntretiens.SetColumn("Date", fillWeight: 80);
            dgvEntretiens.SetColumn("Type", fillWeight: 100);
            dgvEntretiens.SetColumn("Kilométrage", alignRight: true, fillWeight: 90);
            dgvEntretiens.SetColumn("Coût", alignRight: true, fillWeight: 80);
        }

        // ══════════════════════════════════════════════════════════════════
        //  Thème
        // ══════════════════════════════════════════════════════════════════
        private void ApplyTheme()
        {
            BackColor = AppTheme.Background;
            btnTheme.Text = AppTheme.IsDark ? "Mode clair" : "Mode sombre";
            btnTheme.Glyph = AppTheme.IsDark ? Icons.Sun : Icons.Moon;
            NativeTheme.ApplyTitleBar(this);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            NativeTheme.ApplyTitleBar(this);
        }

        // ══════════════════════════════════════════════════════════════════
        //  Données
        // ══════════════════════════════════════════════════════════════════
        private void MainWindow_Load(object? sender, EventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version is not null)
                lblVersion.Text = $"Version {version.Major}.{version.Minor}.{version.Build}";

            _ = LoadVehiclesAsync();
            _maintenanceChecker.Start();
            _ = CheckForUpdatesAsync();
        }

        private async Task LoadVehiclesAsync()
        {
            try
            {
                var vehicles = await _vehicleService.GetAllVehiclesAsync();
                int n = vehicles?.Count ?? 0;
                dgvVehicles.DataSource = vehicles;

                statVehicles.Value = n.ToString();
                statVehicles.Detail = "dans votre garage";
                lblVehiclesSub.Text = n switch { 0 => "Aucun véhicule", 1 => "1 véhicule", _ => $"{n} véhicules" };
                if (n == 0) ShowNoSelection();
            }
            catch (Exception ex) { ShowErrorBox(ex); }
        }

        private async void DgvVehicles_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvVehicles.CurrentRow?.DataBoundItem is not Vehicle vehicle || vehicle.Immatriculation is null)
            {
                ShowNoSelection();
                return;
            }
            await LoadEntretiensAsync(vehicle);
        }

        private async Task LoadEntretiensAsync(Vehicle vehicle)
        {
            int version = ++_selectionVersion;
            try
            {
                var history = await _vehicleService.GetHistVehicleAsync(vehicle.Immatriculation!);
                if (version != _selectionVersion) return;   // une sélection plus récente a pris le relais

                var list = (history?.Historique ?? Enumerable.Empty<Entretien>())
                    .OrderByDescending(ent => ParseDate(ent.date_etretien) ?? DateTime.MinValue)
                    .ToList();

                string vehicleName = $"{vehicle.Marque} {vehicle.Modele}".Trim();
                lblEntretiensSub.Text = $"{vehicleName}  ·  {vehicle.Immatriculation}";
                lblEntretiensSub.Role = TextRole.Accent;
                dgvEntretiens.EmptyText = "Aucun entretien enregistré pour ce véhicule.";
                dgvEntretiens.DataSource = list.Count == 0 ? null : list.Select(ent => new
                {
                    Date = ParseDate(ent.date_etretien)?.ToString("dd/MM/yyyy") ?? ent.date_etretien,
                    Type = ent.type_entretien,
                    Kilométrage = ent.kilometrage?.ToString("N0") + " km",
                    Coût = ent.cout?.ToString("N2") + " €",
                }).ToList();

                float total = list.Sum(ent => ent.cout ?? 0);
                var last = list.Select(ent => ParseDate(ent.date_etretien)).FirstOrDefault(d => d.HasValue);

                statEntretiens.Value = list.Count.ToString();
                statDepenses.Value = $"{total:N2} €";
                statDernier.Value = last?.ToString("dd/MM/yyyy") ?? "—";
                statEntretiens.Detail = statDepenses.Detail = statDernier.Detail = vehicleName;
            }
            catch (Exception ex) { ShowErrorBox(ex); }
        }

        private void ShowNoSelection()
        {
            _selectionVersion++;
            dgvEntretiens.DataSource = null;
            dgvEntretiens.EmptyText = "Sélectionnez un véhicule pour voir ses entretiens.";
            lblEntretiensSub.Text = "Aucun véhicule sélectionné";
            lblEntretiensSub.Role = TextRole.Muted;
            foreach (var card in new[] { statEntretiens, statDepenses, statDernier })
            {
                card.Value = "—";
                card.Detail = "";
            }
        }

        private static DateTime? ParseDate(string? text) => DateTime.TryParse(text, out var d) ? d : null;

        private Vehicle? SelectedVehicleOrWarn()
        {
            if (dgvVehicles.CurrentRow?.DataBoundItem is Vehicle v) return v;
            MessageBox.Show(this, "Veuillez sélectionner un véhicule.", "Aucun véhicule",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return null;
        }

        private void ShowErrorBox(Exception ex)
            => MessageBox.Show(this, $"Erreur : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);

        // ══════════════════════════════════════════════════════════════════
        //  Actions
        // ══════════════════════════════════════════════════════════════════
        private async void BtnAddVehicle_Click(object? sender, EventArgs e)
        {
            using var form = new AddVehicleForm();
            if (form.ShowDialog(this) != DialogResult.OK || form.Vehicle == null) return;
            try
            {
                var added = await _vehicleService.AddVehicleAsync(form.Vehicle);
                if (added == null)
                {
                    MessageBox.Show(this, "Erreur lors de l'ajout.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                await LoadVehiclesAsync();
            }
            catch (Exception ex) { ShowErrorBox(ex); }
        }

        private async void BtnAddEntretien_Click(object? sender, EventArgs e)
        {
            if (SelectedVehicleOrWarn() is not Vehicle vehicle) return;

            string label = $"{vehicle.Marque} {vehicle.Modele}  ·  {vehicle.Immatriculation}";
            using var form = new AddEntretienForm(vehicle.ID, label);
            if (form.ShowDialog(this) != DialogResult.OK || form.Entretien == null) return;
            try
            {
                var added = await _vehicleService.AddEntretienAsync(form.Entretien);
                if (added == null)
                {
                    MessageBox.Show(this, "Erreur lors de l'ajout.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                await LoadEntretiensAsync(vehicle);   // la liste se met à jour tout de suite
            }
            catch (Exception ex) { ShowErrorBox(ex); }
        }

        private async void BtnHistory_Click(object? sender, EventArgs e)
        {
            if (SelectedVehicleOrWarn() is not Vehicle vehicle || vehicle.Immatriculation is null) return;
            try
            {
                var history = await _vehicleService.GetHistVehicleAsync(vehicle.Immatriculation);
                if (history == null)
                {
                    MessageBox.Show(this, "Aucun historique trouvé.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                using var form = new VehicleHistoryForm(history);
                form.ShowDialog(this);
            }
            catch (Exception ex) { ShowErrorBox(ex); }
        }

        private async void BtnExport_Click(object? sender, EventArgs e)
        {
            try
            {
                var config = AppConfig.Load();
                var allVehicles = await _vehicleService.GetAllVehiclesAsync();
                using var selectionForm = new ExportSelectionForm(allVehicles);
                if (selectionForm.ShowDialog(this) != DialogResult.OK) return;

                using var folderDialog = new FolderBrowserDialog { Description = "Choisissez où enregistrer le rapport Excel" };
                if (folderDialog.ShowDialog(this) != DialogResult.OK) return;

                UseWaitCursor = true;
                var histories = new List<VehicleHistory>();
                foreach (var vehicle in selectionForm.SelectedVehicles)
                {
                    if (vehicle.Immatriculation is null) continue;
                    var history = await _vehicleService.GetHistVehicleAsync(vehicle.Immatriculation);
                    if (history != null) histories.Add(history);
                }
                if (!histories.Any())
                {
                    UseWaitCursor = false;
                    MessageBox.Show(this, "Aucun historique trouvé.", "Aucune donnée", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var rapportService = new RapportService();
                string filePath = rapportService.GenererRapport(histories, folderDialog.SelectedPath);

                if (selectionForm.SendEmail)
                {
                    var notifService = new NotificationService();
                    await notifService.SendEmailAsync(config.Smtp.Host, config.Smtp.Port, config.Smtp.User, config.Smtp.Password,
                        selectionForm.Email, "Rapport MyGarage", "Veuillez trouver en pièce jointe votre rapport.", filePath);
                }
                if (selectionForm.SendSms)
                {
                    var notifService = new NotificationService();
                    await notifService.SendSmsAsync(config.FreeMobile.UserId, config.FreeMobile.ApiKey,
                        $"MyGarage : rapport généré pour {selectionForm.SelectedVehicles.Count} véhicule(s).");
                }

                UseWaitCursor = false;
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = filePath, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                UseWaitCursor = false;
                ShowErrorBox(ex);
            }
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                if (currentVersion == null) return;

                var update = await new UpdateService().CheckForUpdateAsync(currentVersion);
                if (update == null || IsDisposed) return;

                using var form = new UpdateForm(update);
                form.ShowDialog(this);
            }
            catch (Exception ex)
            {
                // Pas de fenêtre d'erreur : un échec réseau au démarrage ne doit pas gêner l'utilisateur
                System.Diagnostics.Debug.WriteLine($"Vérification des mises à jour impossible : {ex.Message}");
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            AppTheme.ThemeChanged -= ApplyTheme;
            _maintenanceChecker?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
