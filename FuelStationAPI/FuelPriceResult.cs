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

    public static class FuelTypeExtensions
    {
        public static Predicate<FuelType> Gasoline = (FuelType t) => t switch
        {
            FuelType.Euro95_E5 => true,
            FuelType.Euro95_E10 => true,
            FuelType.Euro98_E5 => true,
            _ => false
        };

        public static Predicate<FuelType> Only(FuelType a) => (FuelType b) => (a == b);
    }
}
