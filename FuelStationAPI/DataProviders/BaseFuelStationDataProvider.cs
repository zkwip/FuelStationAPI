using Microsoft.Extensions.Caching.Memory;

namespace FuelStationAPI.DataProviders
{
    public abstract class BaseFuelStationDataProvider : IFuelStationDataProvider
    {
        protected string _stationDetailUrlPrefix;
        protected string _stationListUrl;

        protected readonly HttpClient _httpClient;
        protected readonly ILogger<BaseFuelStationDataProvider> _logger;
        private readonly IMemoryCache _memoryCache;

        public BaseFuelStationDataProvider(HttpClient client, ILogger<BaseFuelStationDataProvider> logger, IMemoryCache memoryCache)
        {
            _httpClient = client;
            _logger = logger;

            _stationDetailUrlPrefix = "";
            _stationListUrl = "";
            _memoryCache = memoryCache;
        }

        public virtual async Task<IEnumerable<FuelStationIdentifier>> ScrapeStationListAsync()
        {
            return await _memoryCache.GetOrCreateAsync(StationProviderName, async entry => {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                return await QueryStationsAsync();
            });
        }

        private async Task<IEnumerable<FuelStationIdentifier>> QueryStationsAsync()
        {
            using HttpResponseMessage message = await _httpClient.GetAsync(_stationListUrl);
            string msg = await message.Content.ReadAsStringAsync();
            return ExtractStations(msg);
        }

        protected abstract string StationProviderName { get; }

        public bool StationDataSourceCheck(FuelStationIdentifier station) => (station.DataPrivider.ToLower() == StationProviderName);

        public abstract IEnumerable<FuelStationIdentifier> ExtractStations(string msg);

        public virtual async Task<FuelStationScrapeResult> ScrapeStationPricesAsync(FuelStationIdentifier station)
        {
            return await _memoryCache.GetOrCreateAsync(station, async entry => {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await QueryStationPricesAsync((FuelStationIdentifier)entry.Key);
            });
        }

        public virtual async Task<FuelStationScrapeResult> QueryStationPricesAsync(FuelStationIdentifier station)
        {
            string url = _stationDetailUrlPrefix + station.Identifier;
            using HttpResponseMessage message = await _httpClient.GetAsync(url);

            if (!message.IsSuccessStatusCode)
                return new FuelStationScrapeResult(station, new Exception(message.ReasonPhrase));

            string msg = await message.Content.ReadAsStringAsync();

            List<FuelPriceResult> prices = ExtractPrices(msg);

            if (prices.Count == 0)
            {
                _logger.Log(LogLevel.Information, "The station {station} ({identifier}) did not provide any fuel prices", station.Name, station.Identifier);
                return new FuelStationScrapeResult(station, new Exception("No prices found"));
            }

            return new FuelStationScrapeResult(station, prices);
        }

        protected abstract List<FuelPriceResult> ExtractPrices(string msg);

    }
}

