using FuelStationAPI.DataSources;
using FuelStationAPI.Domain;

namespace FuelStationAPI.Aggregator
{
    public class FuelPricesAggregator
    {
        private readonly ILogger<FuelPricesAggregator> _logger;
        private readonly IEnumerable<IDataSource> _dataSources;

        public FuelPricesAggregator(ILogger<FuelPricesAggregator> logger, IEnumerable<IDataSource> dataSources)
        {
            _logger = logger;
            _dataSources = dataSources;
        }

        public async IAsyncEnumerable<FuelStationIdentifier> GetAllStationsAsync()
        {
            _logger.Log(LogLevel.Information, "Number of sources: {sources}", _dataSources.Count());

            var tasks = _dataSources.Select(x => x.GetStationListAsync());
            var results = GreedyAwaitAll(tasks);

            await foreach (var item in results)
            {
                foreach (var station in item.Reduce(new List<FuelStationIdentifier>()))
                    yield return station;
            }
        }

        private static async IAsyncEnumerable<T> GreedyAwaitAll<T>(IEnumerable<Task<T>> tasks)
        {
            foreach (var task in tasks)
            {
                yield return await task;
            }
        }

        public async Task<FuelStationPriceData> GetStationPriceDataAsync(FuelStationIdentifier station)
        {
            var prices = await Collect(GetStationPricesAsync(station));
            return new FuelStationPriceData(station, prices);
        }

        public async IAsyncEnumerable<FuelStationPriceData> GetMultiStationPriceDataAsync(IAsyncEnumerable<FuelStationIdentifier> stations)
        {
            await foreach (var station in stations)
            {
                yield return await GetStationPriceDataAsync(station);
            }
        }

        private async Task<IEnumerable<T>> Collect<T>(IAsyncEnumerable<T> enumerable)
        {
            var values = new List<T>();
            await foreach (var item in enumerable)
            {
                if (item is null)
                    _logger.Log(LogLevel.Warning, "null price detected!");
                else
                    values.Add(item);
            }

            return values;
        }

        private IAsyncEnumerable<FuelPriceResult> GetStationPricesAsync(FuelStationIdentifier station) =>
            GetPriceSource(station).Map(x => GetPricesFromSource(x, station))
                .Reduce(AsyncEnumerable.Empty<FuelPriceResult>());

        private static async IAsyncEnumerable<FuelPriceResult> GetPricesFromSource(IDataSource source, FuelStationIdentifier station)
        {
            var opt = await source.GetPricesAsync(station);
            foreach (var item in opt.Reduce(new()))
            {
                yield return item;
            }
        }

        private Option<IDataSource> GetPriceSource(FuelStationIdentifier station)
        {
            _logger.Log(LogLevel.Warning, "Number of sources: {sources}", _dataSources.Count());

            foreach (IDataSource source in _dataSources)
            {
                if (station.DataPrivider == source.DataProvider)
                    return Option<IDataSource>.Some(source);
            }

            return Option<IDataSource>.None();
        }
    }
}
