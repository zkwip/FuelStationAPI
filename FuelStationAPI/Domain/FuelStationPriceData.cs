namespace FuelStationAPI.Domain
{
    public class FuelStationPriceData
    {
        public Station Station { get; }

        public IEnumerable<FuelPriceResult> Prices { get; }

        public FuelStationPriceData(Station station, IEnumerable<FuelPriceResult> prices)
        {
            Prices = prices;
            Station = station;
        }
    }
}