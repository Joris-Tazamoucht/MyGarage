using System;
using System.Collections.Generic;
using System.Text;
using Models.Models;

namespace Manager.Abstractions
{
    public interface IMyGarageManager
    {
        public Task<Vehicle> GetHistVehicleAsync(string immatriculation);
    }
}
