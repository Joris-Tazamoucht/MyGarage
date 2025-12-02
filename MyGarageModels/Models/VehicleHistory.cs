using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Models
{
    public class VehicleHistory
    {
        public Vehicle Vehicle { get; set; }
        public List<Entretien> Historique { get; set; } = new List<Entretien>();
    }

}
