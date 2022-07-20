namespace FuelStationAPI
{
    public class FuelStationIdentifier
    {
        public string DataPrivider { get; set; }

        public string Identifier { get; set; }

        public string Name { get; set; }

        public Geolocation Location { get; set; }

        public FuelStationIdentifier(string provider, string identifier, string name, Geolocation location)
        {
            Location = location;
            Identifier = identifier;
            DataPrivider = provider;
            Name = name;
        }

        public override string ToString() => $"{DataPrivider}:{Identifier}";
        
    }
}
