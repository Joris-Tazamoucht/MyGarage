using System;
using System.Collections.Generic;
using System.Text;
using Models.Models;

namespace Repository.Abstractions
{
    public interface IMyGarageRepository
    {
        public Task<Vehicle> GetHistVehicleAsync(string immatriculation);
        public Task<Vehicle> AddVehicleAsync (Vehicle vehicle);
        public Task<Vehicle> DeleteVehicleAsync(string immatriculation);
    }
}
