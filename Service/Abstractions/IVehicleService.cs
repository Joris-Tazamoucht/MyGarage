using System;
using System.Collections.Generic;
using System.Text;
using Models.Models;

namespace Service.Abstractions
{
    public interface IVehicleService
    {
        public Task<List<Vehicle>> GetVehiclesAsync (string immatriculation);
        public Task<List<Maintenance>> GetUpcomingMaintenanceAsync();
    }
}
