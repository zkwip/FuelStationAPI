namespace FuelStationAPI
{
    public class Geolocation
    {
        public Geolocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public static readonly Geolocation Strijp = new(51.451438, 5.439417);
        public static readonly Geolocation Ooij = new(51.845234, 5.889089);
        public static readonly Geolocation Veghel = new(51.622560, 5.531636);
        public double Latitude { get; }

        public double Longitude { get; }

        public static double Distance(Geolocation a, Geolocation b)
        {
            //theoretical distance based on a sphere earth mode :p
            (double ax, double ay, double az) = LlTo3dPoint(a);
            (double bx, double by, double bz) = LlTo3dPoint(b);

            double cx = ax - bx;
            double cy = ay - by;
            double cz = az - bz;

            return Math.Sqrt(cx * cx + cy * cy + cz * cz);
        }

        private static (double x, double y, double z) LlTo3dPoint(Geolocation p)
        {
            const double er = 6378.137;
            const double pr = 6356.752;

            double x = er * Math.Cos(p.Latitude * Math.PI / 180.0) * Math.Sin(p.Longitude * Math.PI / 180.0);
            double y = er * Math.Cos(p.Latitude * Math.PI / 180.0) * Math.Cos(p.Longitude * Math.PI / 180.0);
            double z = pr * Math.Sin(p.Latitude * Math.PI / 180.0);

            return (x, y, z);
        }

    }
}