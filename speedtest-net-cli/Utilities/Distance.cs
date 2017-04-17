using System;

namespace SpeedtestNetCli.Utilities
{
    public static class Distance
    {
        private const double earthRadiusKm = 6371;

        public static double Between(Location location1, Location location2)
        {
            var dlat = Radians(location2.Latitude - location1.Latitude);
            var dlon = Radians(location2.Longitude - location1.Longitude);
            var a = (Math.Sin(dlat / 2.0) * Math.Sin(dlat / 2.0) +
                    Math.Cos(Radians(location1.Latitude)) *
                    Math.Cos(Radians(location2.Latitude)) *
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
