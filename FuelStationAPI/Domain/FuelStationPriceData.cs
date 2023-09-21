namespace FuelStationAPI.Domain
{
    public class FuelStationPriceData
    {
        public FuelStationIdentifier Station { get; }

        public IEnumerable<FuelPriceResult> Prices { get; }

        public FuelStationPriceData(FuelStationIdentifier station, IEnumerable<FuelPriceResult> prices)
        {
            Prices = prices;
            Station = station;
        }
    }
}