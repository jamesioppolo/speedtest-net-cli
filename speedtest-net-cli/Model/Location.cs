using System;
using System.Xml.Linq;

namespace SpeedtestNetCli.Model
{
    public class Location
    {
        private const double earthRadiusKm = 6371;

        public double Latitude { get; }
        public double Longitude { get; }

        public Location(XElement node)
        {
            Latitude = Convert.ToDouble(node.Attribute("lat").Value);
            Longitude = Convert.ToDouble(node.Attribute("lon").Value);
        }
        
        public double DistanceTo(Location otherLocation)
        {
            var dlat = Radians(otherLocation.Latitude - Latitude);
            var dlon = Radians(otherLocation.Longitude - Longitude);
            var a = (Math.Sin(dlat / 2.0) * Math.Sin(dlat / 2.0) +
                    Math.Cos(Radians(Latitude)) *
                    Math.Cos(Radians(otherLocation.Latitude)) *
                    Math.Sin(dlon / 2.0) *
                    Math.Sin(dlon / 2.0));
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
            var d = earthRadiusKm * c;

            return d;
        }

        private static double Radians(double value)
        {
            return value * Math.PI / 180.0;
        }
    }
}
