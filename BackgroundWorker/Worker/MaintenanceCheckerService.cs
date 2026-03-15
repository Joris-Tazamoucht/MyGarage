using Manager.Abstractions;

namespace BackgroundWorker
{

    public class MaintenanceCheckerService : IDisposable
    {
        private readonly IMyGarageManager _manager;
        private readonly int _intervalMinutes;
        private System.Threading.Timer? _timer;

        public event Action<string>? MaintenanceAlert;

        public MaintenanceCheckerService(IMyGarageManager manager, int intervalMinutes = 60)
        {
            _manager = manager;
            _intervalMinutes = intervalMinutes;
            // ✅ Plus de démarrage immédiat dans le constructeur
        }

        // ✅ Appelé depuis MainWindow_Load quand le handle est prêt
        public void Start()
        {
            _timer = new System.Threading.Timer(
                callback: _ => CheckMaintenanceAsync().GetAwaiter().GetResult(),
                state: null,
                dueTime: TimeSpan.Zero,
                period: TimeSpan.FromMinutes(_intervalMinutes)
            );
        }

        private async Task CheckMaintenanceAsync()
        {
            try
            {
                var vehicles = await _manager.GetAllVehiclesAsync();

                foreach (var vehicle in vehicles)
                {
                    if (vehicle.Immatriculation is null) continue;

                    var history = await _manager.GetHistVehicleAsync(vehicle.Immatriculation);
                    if (history?.Historique == null || !history.Historique.Any()) continue;

                    var dernierEntretien = history.Historique.First();

                    if (DateTime.TryParse(dernierEntretien.date_etretien, out var date))
                    {
                        if ((DateTime.Now - date).TotalDays > 335)
                        {
                            MaintenanceAlert?.Invoke(
                                $"⚠️ {vehicle.Marque} {vehicle.Modele} ({vehicle.Immatriculation}) : " +
                                $"dernier entretien le {date:dd/MM/yyyy} — révision bientôt nécessaire !");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MaintenanceAlert?.Invoke($"Erreur vérification entretiens : {ex.Message}");
            }
        }

        public void Dispose() => _timer?.Dispose();
    }
}