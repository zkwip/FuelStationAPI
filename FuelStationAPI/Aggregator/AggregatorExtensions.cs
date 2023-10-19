using FuelStationAPI.Domain;

namespace FuelStationAPI.Aggregator
{
    static class AggregatorExtensions
    {
        public static IAsyncEnumerable<FuelStationPriceData> NonEmpty(this IAsyncEnumerable<FuelStationPriceData> source) =>
            source.Where(x => x.Prices.Any(price => price.Price > 0.0));

        public static IAsyncEnumerable<FuelStationPriceData> OnlyFuel(this IAsyncEnumerable<FuelStationPriceData> source, FuelType type) =>
            source.Select(oldData => FilterPrice(oldData, x => x == type)).NonEmpty();

        public static IAsyncEnumerable<FuelStationPriceData> OnlyFuel(this IAsyncEnumerable<FuelStationPriceData> source, Predicate<FuelType> fuelSpec) =>
            source.Select(oldData => FilterPrice(oldData, fuelSpec)).NonEmpty();

        private static FuelStationPriceData FilterPrice(FuelStationPriceData oldData, Predicate<FuelType> fuelSpec)
        {
            if (!oldData.Prices.Any())
                return oldData;

            var newPrice = oldData.Prices.Where(x => fuelSpec(x.FuelType)).Where(x => x.Price > 0);
            return new FuelStationPriceData(oldData.Station, newPrice);
        }

        public static IAsyncEnumerable<FuelStationPriceData> AggregatePrices(this IAsyncEnumerable<Station> stations, FuelPricesAggregator aggregator) =>
            aggregator.GetMultiStationPriceDataAsync(stations).NonEmpty();

        public static IAsyncEnumerable<CostAnalysis<FuelStationPriceData>> ToCostAnalysis(this IAsyncEnumerable<FuelStationPriceData> data, Func<FuelStationPriceData, double> initialCost) =>
            data.Select(item => new CostAnalysis<FuelStationPriceData>(item, initialCost(item)));

        public static IAsyncEnumerable<CostAnalysis<FuelStationPriceData>> AddCost(this IAsyncEnumerable<CostAnalysis<FuelStationPriceData>> original, Func<FuelStationPriceData, double> costMap) =>
            original.Select(item => item.AddCost(costMap(item.Value)));


        public static async IAsyncEnumerable<TOut> SelectAsync<TIn, TOut>(this IAsyncEnumerable<TIn> source, Func<TIn, Task<TOut>> map) { await foreach (var item in source) yield return await map(item); }

    }
}
