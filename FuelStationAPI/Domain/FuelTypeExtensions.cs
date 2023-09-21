namespace FuelStationAPI.Domain
{
    public static class FuelTypeExtensions
    {
        public static Predicate<FuelType> Gasoline => (t) => t switch
        {
            FuelType.Euro95_E5 => true,
            FuelType.Euro95_E10 => true,
            FuelType.Euro98_E5 => true,
            _ => false
        };

        public static Predicate<T> All<T>() => (t) => true;

        public static Predicate<FuelType> Only(FuelType a) => (b) => a == b;
    }
}
