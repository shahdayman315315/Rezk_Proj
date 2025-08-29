namespace Rezk_Proj.Helpers
{
    public class GeoHelper
    {
        public static double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            double R = 6371e3; // نصف قطر الأرض بالمتر

            // نحول الدرجات لراديان
            double phi1 = (double)lat1 * (Math.PI / 180.0);
            double phi2 = (double)lat2 * (Math.PI / 180.0);
            double dPhi = ((double)lat2 - (double)lat1) * (Math.PI / 180.0);
            double dLambda = ((double)lon2 - (double)lon1) * (Math.PI / 180.0);

            double a = Math.Sin(dPhi / 2) * Math.Sin(dPhi / 2) +
                       Math.Cos(phi1) * Math.Cos(phi2) *
                       Math.Sin(dLambda / 2) * Math.Sin(dLambda / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = R * c; // المسافة بالمتر
            return distance / 1000.0; // تحويل إلى كيلومتر
        }
    }
}