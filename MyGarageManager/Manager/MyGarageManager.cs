using Manager.Abstractions;
using Models.Models;
using Repository.Abstractions;

namespace Manager.Manager
{
    public class MyGarageManager : IMyGarageManager
    {
        private readonly IMyGarageRepository _myGarageRepository;

        public MyGarageManager(IMyGarageRepository myGarageRepository)
        {
            _myGarageRepository = myGarageRepository;
        }

        public async Task<List<Vehicle>> GetVehicleAsync(string immatriculation)
        {
            var res = await _myGarageRepository.GetVehicleAsync(immatriculation);

            if (res is not null)
                return res;

            return null;
        }

        public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
        {
            var res = await _myGarageRepository.AddVehicleAsync(vehicle);
            if (res is not null)
                return res;
            return null;
        }

        public async Task<Vehicle> DeleteVehicleAsync(string immatriculation)
        {
            var res = await _myGarageRepository.DeleteVehicleAsync(immatriculation);
            if (res is not null)
                return res;
            return null;
        }

        public async Task<VehicleHistory> GetHistVehicleAsync (string immatriculation)
        {
            var res = await _myGarageRepository.GetHistVehicleAsync(immatriculation);
            if (res is not null)
                return res;
            return null;
        }
    }
}
