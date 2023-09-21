using FuelStationAPI.Domain;

namespace FuelStationAPI
{
    public static class FuelTypeExtensions
    {
        public static Predicate<FuelType> Gasoline => (FuelType t) => t switch
        {
            FuelType.Euro95_E5 => true,
            FuelType.Euro95_E10 => true,
            FuelType.Euro98_E5 => true,
            _ => false
        };

        public static Predicate<T> All<T>() => (T t) => true;

        public static Predicate<FuelType> Only(FuelType a) => (FuelType b) => (a == b);
    }
}
