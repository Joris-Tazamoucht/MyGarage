using Manager.Abstractions;
using Models.Models;
using Service.Abstractions;

namespace Service.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IMyGarageManager _manager;

        // Plus de URL HTTP — on injecte le manager directement
        public VehicleService(IMyGarageManager manager)
        {
            _manager = manager;
        }

        public async Task<List<Vehicle>> GetVehiclesAsync(string immatriculation)
            => await _manager.GetVehicleAsync(immatriculation);

        public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
            => await _manager.AddVehicleAsync(vehicle);

        public async Task<Vehicle> DeleteVehicleAsync(string immatriculation)
            => await _manager.DeleteVehicleAsync(immatriculation);

        public async Task<VehicleHistory> GetHistVehicleAsync(string immatriculation)
            => await _manager.GetHistVehicleAsync(immatriculation);
    }
}