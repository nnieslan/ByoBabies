using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ByoBaby.Rest.Helpers
{
    public class LocationCalculationHelper
    {
        public static double CalcHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            int R = 6371; // km

            var dLat = DegreeToRadian(lat2 - lat1);
            var dLon = DegreeToRadian(lon2 - lon1);
            var rlat1 = DegreeToRadian(lat1);
            var rlat2 = DegreeToRadian(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c;

            return d;
        }

        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}