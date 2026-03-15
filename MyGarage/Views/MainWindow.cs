using BackgroundWorker;
using Manager.Manager;
using Models.Models;
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

        public MainWindow()
        {
            InitializeComponent();
            this.Load += MainWindow_Load;

            string dbPath = "C:\\Users\\Joris\\Desktop\\MyGarage.db"; // Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mygarage.db");
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

            dataGridView2.SelectionChanged += dataGridView2_SelectionChanged;
            btnExport.Click += btnExport_Click;

            _ = LoadVehiclesAsync();
        }

        private void MainWindow_Load(object? sender, EventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version is not null)
                label3.Text = $"Version : {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";

            _maintenanceChecker.Start();

            // ✅ Vérification des mises à jour en arrière-plan
            _ = CheckForUpdatesAsync();
        }

        private async Task LoadVehiclesAsync()
        {
            try
            {
                var vehicles = await _vehicleService.GetAllVehiclesAsync();
                dataGridView2.DataSource = vehicles;

                if (dataGridView2.Columns["ID"] != null)
                    dataGridView2.Columns["ID"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Ajouter un véhicule (ex: via un formulaire de saisie) et rafraîchir la liste après l'ajout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button1_Click(object sender, EventArgs e)
        {
            using var form = new AddVehicleForm();

            if (form.ShowDialog(this) != DialogResult.OK || form.Vehicle == null)
                return;

            try
            {
                var added = await _vehicleService.AddVehicleAsync(form.Vehicle);

                if (added == null)
                {
                    MessageBox.Show("Erreur lors de l'ajout du véhicule.",
                        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show($"{added.Marque} {added.Modele} ({added.Immatriculation}) ajouté avec succès.",
                    "Véhicule ajouté", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Rafraîchir la liste
                await LoadVehiclesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async void dataGridView2_SelectionChanged(object? sender, EventArgs args)
        {
            if (dataGridView2.CurrentRow?.DataBoundItem is not Vehicle selectedVehicle) return;
            if (selectedVehicle.Immatriculation is null) return;

            try
            {
                var history = await _vehicleService.GetHistVehicleAsync(selectedVehicle.Immatriculation);

                if (history == null || history.Historique == null || !history.Historique.Any())
                {
                    dataGridView1.DataSource = null;
                    return;
                }

                var rows = history.Historique.Select(ent => new
                {
                    Date = DateTime.TryParse(ent.date_etretien, out var d) ? d.ToString("dd/MM/yyyy") : ent.date_etretien,
                    Type = ent.type_entretien,
                    Kilométrage = ent.kilometrage?.ToString("N0") + " km",
                    Coût = ent.cout?.ToString("N2") + " €",
                }).ToList();

                dataGridView1.DataSource = rows;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _maintenanceChecker?.Dispose();
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Ajouter un entretien pour le véhicule sélectionné (ex: via un formulaire de saisie) et rafraîchir la liste après l'ajout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button2_Click(object sender, EventArgs e)
        {
            // Vérifier qu'un véhicule est sélectionné dans le DataGridView
            if (dataGridView2.CurrentRow?.DataBoundItem is not Vehicle selectedVehicle)
            {
                MessageBox.Show("Veuillez sélectionner un véhicule dans la liste.",
                    "Aucun véhicule sélectionné", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string vehicleLabel = $"{selectedVehicle.Marque} {selectedVehicle.Modele} ({selectedVehicle.Immatriculation})";

            using var form = new AddEntretienForm(selectedVehicle.ID, vehicleLabel);

            if (form.ShowDialog(this) != DialogResult.OK || form.Entretien == null)
                return;

            try
            {
                var added = await _vehicleService.AddEntretienAsync(form.Entretien);

                if (added == null)
                {
                    MessageBox.Show("Erreur lors de l'ajout de l'entretien.",
                        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show(
                    $"Entretien '{added.type_entretien}' du {DateTime.Parse(added.date_etretien!):dd/MM/yyyy} ajouté avec succès.",
                    "Entretien ajouté", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Voir l'historique d'entretien du véhicule sélectionné (ex: via une nouvelle fenêtre ou un panneau latéral)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow?.DataBoundItem is not Vehicle selectedVehicle)
            {
                MessageBox.Show("Veuillez sélectionner un véhicule dans la liste.",
                    "Aucun véhicule sélectionné", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectedVehicle.Immatriculation is null)
            {
                MessageBox.Show("Ce véhicule n'a pas d'immatriculation renseignée.",
                    "Données manquantes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var history = await _vehicleService.GetHistVehicleAsync(selectedVehicle.Immatriculation);

                if (history == null)
                {
                    MessageBox.Show("Aucun historique trouvé pour ce véhicule.",
                        "Historique introuvable", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using var form = new VehicleHistoryForm(history);
                form.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Générer un rapport Excel pour les véhicules sélectionnés et l'envoyer par email/SMS
        /// </summary>
        private async void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var config = AppConfig.Load();
                var allVehicles = await _vehicleService.GetAllVehiclesAsync();

                using var selectionForm = new ExportSelectionForm(allVehicles);
                if (selectionForm.ShowDialog(this) != DialogResult.OK) return;

                using var folderDialog = new FolderBrowserDialog
                {
                    Description = "Choisissez où enregistrer le rapport Excel"
                };
                if (folderDialog.ShowDialog(this) != DialogResult.OK) return;

                var histories = new List<VehicleHistory>();
                foreach (var vehicle in selectionForm.SelectedVehicles)
                {
                    if (vehicle.Immatriculation is null) continue;
                    var history = await _vehicleService.GetHistVehicleAsync(vehicle.Immatriculation);
                    if (history != null) histories.Add(history);
                }

                if (!histories.Any())
                {
                    MessageBox.Show("Aucun historique trouvé pour les véhicules sélectionnés.",
                        "Aucune donnée", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var rapportService = new RapportService();
                string filePath = rapportService.GenererRapport(histories, folderDialog.SelectedPath);

                if (selectionForm.SendEmail)
                {
                    var notifService = new NotificationService();
                    await notifService.SendEmailAsync(
                        smtpHost: config.Smtp.Host,
                        smtpPort: config.Smtp.Port,
                        smtpUser: config.Smtp.User,
                        smtpPassword: config.Smtp.Password,
                        to: selectionForm.Email,
                        subject: "Rapport MyGarage",
                        body: "Veuillez trouver en pièce jointe votre rapport d'entretien MyGarage.",
                        attachmentPath: filePath
                    );
                }

                if (selectionForm.SendSms)
                {
                    var notifService = new NotificationService();
                    await notifService.SendSmsAsync(
                        userId: config.FreeMobile.UserId,
                        apiKey: config.FreeMobile.ApiKey,
                        message: $"MyGarage : rapport généré pour {selectionForm.SelectedVehicles.Count} véhicule(s)."
                    );
                }

                MessageBox.Show($"Rapport généré avec succès !\n{filePath}",
                    "Export terminé", MessageBoxButtons.OK, MessageBoxIcon.Information);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

                // Retour sur le thread UI
                this.Invoke(() =>
                {
                    using var form = new UpdateForm(update);
                    form.ShowDialog(this);
                });
            }
            catch
            {
                // Silencieux — pas de connexion internet ou GitHub indisponible
            }
        }
    }
}