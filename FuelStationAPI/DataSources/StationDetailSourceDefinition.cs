namespace FuelStationAPI.DataSources
{
    public class StationDetailSourceDefinition
    {
        public readonly Func<string,string> UrlBuilder;

        public readonly string Name;

        public StationDetailSourceDefinition(Func<string, string> urlBuilder, string name)
        {
            UrlBuilder = urlBuilder;
            Name = name;
        }
    }
}
