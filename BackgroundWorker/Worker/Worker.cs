using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Service.Services;

namespace BackgroundWorker.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly VehicleService _vehicleService; // ton service qui appelle l'API

        public Worker(ILogger<Worker> logger, VehicleService vehicleService)
        {
            _logger = logger;
            _vehicleService = vehicleService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) { }
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        try
        //        {
        //            var upcoming = await _vehicleService.GetUpcomingMaintenanceAsync();

        //            foreach (var v in upcoming)
        //            {
        //                // Ici tu peux utiliser ToastNotification ou console
        //                _logger.LogInformation("Entretien à venir pour {immatriculation}", v.immatriculation);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Erreur lors de la récupération des entretiens");
        //        }

        //        // Attendre 30 minutes avant la prochaine vérification
        //        await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        //    }
        //}
    }
}
