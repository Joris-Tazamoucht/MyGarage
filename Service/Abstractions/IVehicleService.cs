using Models.Models;

namespace Service.Abstractions
{
    public interface IVehicleService
    {
        Task<List<Vehicle>> GetVehiclesAsync(string immatriculation);
        Task<Vehicle> AddVehicleAsync(Vehicle vehicle);
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle> DeleteVehicleAsync(string immatriculation);
        Task<VehicleHistory> GetHistVehicleAsync(string immatriculation);

        Task<Entretien?> AddEntretienAsync(Entretien entretien);
    }
}