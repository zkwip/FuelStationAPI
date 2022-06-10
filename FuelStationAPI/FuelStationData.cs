namespace FuelStationAPI
{
    public class FuelStationData
    {
        public string DataPrivider { get; set; }

        public string Identifier { get; set; }

        public string Name { get; set; }

        public Geolocation Location { get; set; }

        public FuelStationData(string provider, string identifier, string name, Geolocation location)
        {
            Location = location;
            Identifier = identifier;
            DataPrivider = provider;
            Name = name;
        }
    }
}
