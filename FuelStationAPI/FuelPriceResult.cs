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

    public enum FuelType
    {
        E10 = 0,
        Euro95 = 0,

        E5 = 1,
        Euro98 = 1,

        Diesel = 2,
        D = 2,

        Electricity = 3
    }
}
