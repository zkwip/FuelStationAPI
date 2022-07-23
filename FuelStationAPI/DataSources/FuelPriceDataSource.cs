using Microsoft.Extensions.Caching.Memory;
using TextScanner;

namespace FuelStationAPI.DataSources
{
    public class FuelPriceDataSource : IFuelPriceDataSource
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;

        private readonly string _providerName;
        private readonly ITextSpanMapper<List<FuelPriceResult>> _mapper;
        private readonly Func<FuelStationIdentifier, string> _urlBuilder;

        public string DataProvider => _providerName;

        public FuelPriceDataSource(IHttpClientFactory httpClient, IMemoryCache memoryCache, ITextSpanMapper<List<FuelPriceResult>> mapper, Func<FuelStationIdentifier, string> urlBuilder, string providerName)
        {
            _httpClientFactory = httpClient;
            _memoryCache = memoryCache;
            _mapper = mapper;
            _urlBuilder = urlBuilder;
            _providerName = providerName;
        }

        public async Task<MappedScanResult<List<FuelPriceResult>>> GetPricesAsync(FuelStationIdentifier station)
        {
            return await _memoryCache.GetOrCreateAsync(station, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                return await QueryStationPricesAsync(station);
            });
        }

        private async Task<MappedScanResult<List<FuelPriceResult>>> QueryStationPricesAsync(FuelStationIdentifier station)
        {
            var body = await GetHttpBodyAsync(GetUrl(station));

            if (body is null)
                return MappedScanResult<List<FuelPriceResult>>.Fail("The HTTP request failed");

            return _mapper.Map(new(body));

        }

        private async Task<string?> GetHttpBodyAsync(string url)
        {

            using var httpClient = _httpClientFactory.CreateClient(_providerName);
            using var message = await httpClient.GetAsync(url);

            if (!message.IsSuccessStatusCode)
                return null;

            return await message.Content.ReadAsStringAsync();
        }
        private string GetUrl(FuelStationIdentifier station) => _urlBuilder.Invoke(station);
    }
}
