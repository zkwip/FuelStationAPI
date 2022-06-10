
namespace FuelStationAPI
{
    public class FuelStationQueryResult
    {
        public IEnumerable<FuelPriceResult> Prices { get; }

        public string Name { get; }

        public FuelStationQueryResult(string name, IEnumerable<FuelPriceResult> prices)
        {
            Name = name;
            Prices = prices;
        }
    }
}