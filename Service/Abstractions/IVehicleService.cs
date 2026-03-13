using Models.Models;

namespace Service.Abstractions
{
    public interface IVehicleService
    {
        Task<List<Vehicle>> GetVehiclesAsync(string immatriculation);
        Task<Vehicle> AddVehicleAsync(Vehicle vehicle);
        Task<Vehicle> DeleteVehicleAsync(string immatriculation);
        Task<VehicleHistory> GetHistVehicleAsync(string immatriculation);
    }
}