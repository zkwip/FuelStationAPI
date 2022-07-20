namespace FuelStationAPI
{
    public class FuelStationScrapeResult
    {
        public bool Succes { get; }

        public IEnumerable<FuelPriceResult> FuelPrices { get; }

        public String FaultMessage { get; }

        public FuelStationIdentifier Station { get; }

        public FuelStationScrapeResult(FuelStationIdentifier station, Exception ex)
        {
            FaultMessage = ex.Message;
            Succes = false;
            FuelPrices = new List<FuelPriceResult>();
            Station = station;
        }

        public FuelStationScrapeResult(FuelStationIdentifier station, IEnumerable<FuelPriceResult> fuelPrices)
        {
            FuelPrices = fuelPrices;
            Succes = true;
            Station = station;
            FaultMessage = "";
        }
    }
}