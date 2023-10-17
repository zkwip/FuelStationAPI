using FuelStationAPI.Domain;
using FuelStationAPI.Mappers;
using Microsoft.Extensions.Caching.Memory;
using TextScanner;
using TextScanner.Pattern;

namespace FuelStationAPI.DataSources
{
    public abstract class BaseScrapeService : IDataSource
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<BaseScrapeService> _logger;
        private readonly string _dataProvider;
        private readonly bool _decimalComma;

        protected BaseScrapeService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, ILogger<BaseScrapeService> logger, string dataProvider, bool decimalComma)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _logger = logger;
            _dataProvider = dataProvider;
            _decimalComma = decimalComma;
        }

        protected abstract string PriceUrlBuilder(FuelStationIdentifier station);
        protected abstract string StationListUrl { get; }
        protected abstract ScanPattern StationPattern { get; }
        protected abstract ScanPattern PricePattern { get; }
        public string DataProvider => _dataProvider;


        public async Task<Option<List<FuelStationIdentifier>>> GetStationListAsync()
        {
            return await _memoryCache.GetOrCreateAsync(_dataProvider, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                return await QueryStationListAsync();
            });
        }

        private async Task<Option<List<FuelStationIdentifier>>> QueryStationListAsync()
        {
            var body = await GetHttpBodyAsync(StationListUrl);

            if (body is null)
            {
                _logger.LogWarning("Scraper of type {dataProvider} found null body on URL {url}", _dataProvider, StationListUrl);
                return Option<List<FuelStationIdentifier>>.None();
            }

            var stationMapper = new PatternMapper<FuelStationIdentifier>(
                StationPattern,
                new FuelStationIdentifierMapper(_dataProvider, "")
            ).Repeat;

            Option<List<FuelStationIdentifier>> data = stationMapper.Map(new(body));

            _logger.LogWarning("Number of results from provider {provider}: {number}",_dataProvider, data.Reduce(new()).Count());
            return data;
        }

        private async Task<string?> GetHttpBodyAsync(string url)
        {
            using var httpClient = _httpClientFactory.CreateClient("scraper");
            using var message = await httpClient.GetAsync(url);

            if (!message.IsSuccessStatusCode)
                return null;

            var msg = await message.Content.ReadAsStringAsync();
            return msg;
        }

        public async Task<Option<List<FuelPriceResult>>> GetPricesAsync(FuelStationIdentifier station)
        {
            return await _memoryCache.GetOrCreateAsync(station, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await QueryStationPricesAsync(station);
            });
        }

        private async Task<Option<List<FuelPriceResult>>> QueryStationPricesAsync(FuelStationIdentifier station)
        {
            var body = await GetHttpBodyAsync(PriceUrlBuilder(station));

            if (body is null)
                return Option<List<FuelPriceResult>>.None();

            var priceMapper = new PatternMapper<FuelPriceResult>(
                PricePattern,
                new FuelPriceResultMapper(_decimalComma)
            ).Repeat;

            return priceMapper.Map(new(body));
        }
    }
}