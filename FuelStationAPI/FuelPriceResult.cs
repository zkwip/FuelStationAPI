namespace FuelStationAPI
{
    public class FuelPriceResult
    {
        public FuelPriceResult(FuelType fuelType, double price)
        {
            FuelType = fuelType;
            Price = price;
        }
        public FuelType FuelType { get; }

        public double Price { get; }
    }
}
