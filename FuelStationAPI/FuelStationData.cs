namespace FuelStationAPI
{
    public class FuelStationData
    {
        public string Brand { get; set; }

        public string Identifier { get; set; }

        public string Name { get; set; }

        public Geolocation Location { get; set; }

        public FuelStationData(string brand, string identifier, string name, Geolocation location)
        {
            Location = location;
            Identifier = identifier;
            Brand = brand;
            Name = name;
        }
    }
}
