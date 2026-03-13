using System;
using System.Collections.Generic;
using System.Text;
using Models.Models;

namespace Repository.Abstractions
{
    public interface IMyGarageRepository
    {
        public Task<List<Vehicle>> GetVehicleAsync(string immatriculation);
        public Task<Vehicle> AddVehicleAsync (Vehicle vehicle);
        public Task<Vehicle> DeleteVehicleAsync(string immatriculation);
        public Task<VehicleHistory> GetHistVehicleAsync(string immatriculation);
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<Entretien?> AddEntretienAsync(Entretien entretien);

    }
}
