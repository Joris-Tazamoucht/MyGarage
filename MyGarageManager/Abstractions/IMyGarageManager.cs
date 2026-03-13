using Models.Models;

namespace Manager.Abstractions
{
    public interface IMyGarageManager
    {
        Task<List<Vehicle>> GetVehicleAsync(string immatriculation);
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle> AddVehicleAsync(Vehicle vehicle);
        Task<Vehicle> DeleteVehicleAsync(string immatriculation);
        Task<VehicleHistory> GetHistVehicleAsync(string immatriculation);

        Task<Entretien?> AddEntretienAsync(Entretien entretien);
    }
}