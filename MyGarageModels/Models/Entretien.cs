using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Models
{
    public class Entretien
    {
        public int id { get; set; }
        public int vehicle_id { get; set; }
        public string? date_etretien { get; set; }
        public string? type_etretien { get; set; }
        public int? kilometrage { get; set; }
        public float? cout { get; set; }
        public string? notes { get; set; }


    }
}
