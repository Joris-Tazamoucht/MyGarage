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

        public async Task<Vehicle> GetHistVehicleAsync(string immatriculation)
        {
            var res = await _myGarageRepository.GetHistVehicleAsync(immatriculation);

            if (res is not null)
                return res;

            return null;
        }
    }
}
