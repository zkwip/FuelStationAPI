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
        AdBlue,
        Euro95_E5,
        Euro95_E10,
        Euro98_E5,
        LPG,
        CNG,
        Hydrogen,
        Diesel,
        Electricity
    }
}
