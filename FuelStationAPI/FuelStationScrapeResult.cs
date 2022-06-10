namespace FuelStationAPI
{
    public class FuelStationScrapeResult
    {
        public bool Succes { get; }

        public IEnumerable<FuelPriceResult> FuelPrices { get; }

        public Exception? Exception { get; }

        public FuelStationData Station { get; }

        public FuelStationScrapeResult(FuelStationData station, Exception ex)
        {
            Exception = ex;
            Succes = false;
            FuelPrices = new List<FuelPriceResult>();
            Station = station;
        }

        public FuelStationScrapeResult(FuelStationData station, IEnumerable<FuelPriceResult> fuelPrices)
        {
            FuelPrices = fuelPrices;
            Succes = true;
            Station = station;
        }
    }
}