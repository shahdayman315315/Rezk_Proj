namespace Rezk_Proj.Helpers
{
    public class GeoHelper
    {
        public static double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            // نحول من decimal لـ double
            double dLat1 = (double)lat1;
            double dLon1 = (double)lon1;
            double dLat2 = (double)lat2;
            double dLon2 = (double)lon2;

            var R = 6371; // نصف قطر الأرض بالكيلومتر
            var dLat = (dLat2 - dLat1) * (Math.PI / 180);
            var dLon = (dLon2 - dLon1) * (Math.PI / 180);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(dLat1 * (Math.PI / 180)) * Math.Cos(dLat2 * (Math.PI / 180)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // المسافة بالكيلومتر
        }
    }
}
