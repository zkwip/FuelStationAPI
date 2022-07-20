
namespace FuelStationAPI
{
    public class FuelStationPricesPair
    {
        public FuelStationIdentifier Station { get; }

        public IEnumerable<FuelPriceResult> Prices { get; }

        public FuelStationPricesPair(FuelStationIdentifier station, IEnumerable<FuelPriceResult> prices)
        {
            Prices = prices;
            Station = station;
        }
    }
}