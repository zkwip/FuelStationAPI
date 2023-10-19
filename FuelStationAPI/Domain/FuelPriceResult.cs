namespace FuelStationAPI.Domain
{
    public class FuelPriceResult
    {
        public FuelPriceResult(FuelType fuelType, double price, DateTime retrievalTime)
        {
            FuelType = fuelType;
            Price = price;
            RetrievalTime = retrievalTime;
        }

        public DateTime RetrievalTime { get; }

        public FuelType FuelType { get; }

        public double Price { get; }
    }
}
