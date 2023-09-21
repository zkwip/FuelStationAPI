using FuelStationAPI.Domain;

namespace FuelStationAPI
{
    static class FuelServiceAggeregatorExtensions
    {
        public static IAsyncEnumerable<FuelStationPriceData> NonEmpty(this IAsyncEnumerable<FuelStationPriceData> source) => 
            source.Where(x => x.Prices.Any(price => price.Price > 0.0));

        public static IAsyncEnumerable<FuelStationPriceData> OnlyFuel(this IAsyncEnumerable<FuelStationPriceData> source, FuelType type) =>
            source.Select(oldData => FilterPrice(oldData, type)).NonEmpty();

        private static FuelStationPriceData FilterPrice(FuelStationPriceData oldData, FuelType type)
        {
            if (!oldData.Prices.Any())
                return oldData;

            var newPrice = oldData.Prices.Where<FuelPriceResult>((FuelPriceResult price) => price.FuelType == type);
            return new FuelStationPriceData(oldData.Station, newPrice);
        }

        public static IAsyncEnumerable<FuelStationPriceData> AggregatePrices(this IAsyncEnumerable<FuelStationIdentifier> stations, FuelServiceAggregator aggregator) =>
            aggregator.GetMultiStationPriceDataAsync(stations).NonEmpty();

        public static IAsyncEnumerable<CostAnalysis<FuelStationPriceData>> ToCostAnalysis(this IAsyncEnumerable<FuelStationPriceData> data, Func<FuelStationPriceData, double> initialCost) =>
            data.Select(item => new CostAnalysis<FuelStationPriceData>(item, initialCost(item)));

        public static IAsyncEnumerable<CostAnalysis<FuelStationPriceData>> AddCost(this IAsyncEnumerable<CostAnalysis<FuelStationPriceData>> original, Func<FuelStationPriceData, double> costMap) =>
            original.Select(item => item.AddCost(costMap(item.Value)));


    }
}
