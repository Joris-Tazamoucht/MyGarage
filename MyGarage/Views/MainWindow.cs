using System.Reflection;
using BackgroundWorker;
using Manager.Manager;
using Repository.Repository;
using Service.Abstractions;
using Service.Services;

namespace MyGarage
{
    public partial class MyGarage : Form
    {
        private readonly IVehicleService _vehicleService;
        private readonly MaintenanceCheckerService _maintenanceChecker;

        public MyGarage()
        {
            InitializeComponent();
            this.Load += MainWindow_Load;

            string dbPath = "C:\\Users\\joris\\Desktop\\MyGarage.db"; // Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mygarage.db");
            string connectionString = $"Data Source={dbPath}";

            MessageBox.Show($"DB Path: {dbPath}\nConnection: {connectionString}");

            var repository = new MyGarageRepository(connectionString);
            var manager = new MyGarageManager(repository);

            _vehicleService = new VehicleService(manager);

            // ✅ On crée le checker mais on le démarre dans Load, pas dans le constructeur
            _maintenanceChecker = new MaintenanceCheckerService(manager, intervalMinutes: 60);
            _maintenanceChecker.MaintenanceAlert += msg =>
            {
                if (this.IsHandleCreated)
                    this.Invoke(() => MessageBox.Show(msg, "Entretien à prévoir",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning));
            };

            LoadVehiclesAsync();
        }

        private void MainWindow_Load(object? sender, EventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version is not null)
                label3.Text = $"Version : {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";

            _maintenanceChecker.Start();
        }

        private async void LoadVehiclesAsync()
        {
            try
            {
                var vehicles = await _vehicleService.GetVehiclesAsync("AB-123-CD");
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

        private async void button1_Click(object sender, EventArgs e)
        {
            // À compléter selon besoin (ajout, suppression...)
        }

        private async void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Gérer le clic sur une cellule, ex: afficher l'historique
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _maintenanceChecker?.Dispose();
            base.OnFormClosing(e);
        }
    }
}