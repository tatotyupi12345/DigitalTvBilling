using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.ListModels
{
    public class GPSLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double TowerLat { get; set; }
        public double TowerLon { get; set; }
    }
}