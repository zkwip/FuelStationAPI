using FuelStationAPI.DataSources;
using FuelStationAPI.Domain;

namespace FuelStationAPI
{
    public class FuelServiceAggregator
    {
        private readonly ILogger<FuelServiceAggregator> _logger;
        private readonly IEnumerable<IFuelPriceDataSource> _fuelPriceDataSources;
        private readonly IEnumerable<IFuelStationListSource> _stationListSources;

        public FuelServiceAggregator(ILogger<FuelServiceAggregator> logger, IEnumerable<IFuelPriceDataSource> fuelPriceDataSources, IEnumerable<IFuelStationListSource> stationListSources)
        {
            _logger = logger;

            _fuelPriceDataSources = fuelPriceDataSources;
            _stationListSources = stationListSources;
        }

        public async IAsyncEnumerable<FuelStationIdentifier> GetAllStationsAsync()
        {
            var tasks = _stationListSources.Select(x => x.GetStationListAsync());
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

        private static async IAsyncEnumerable<FuelPriceResult> GetPricesFromSource(IFuelPriceDataSource source, FuelStationIdentifier station)
        {
            var opt = await source.GetPricesAsync(station);
            foreach (var item in opt.Reduce(new()))
            {
                yield return item;
            }
        }

        private Option<IFuelPriceDataSource> GetPriceSource(FuelStationIdentifier station)
        {
            foreach (IFuelPriceDataSource source in _fuelPriceDataSources)
            {
                if (station.DataPrivider == source.DataProvider)
                    return Option<IFuelPriceDataSource>.Some(source);
            }

            return Option<IFuelPriceDataSource>.None();
        }
    }
}
