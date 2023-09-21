using FuelStationAPI.Domain;
using Microsoft.Extensions.Caching.Memory;
using TextScanner.SpanMapper;

namespace FuelStationAPI.DataSources
{
    public class FuelStationListSource : IFuelStationListSource
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;

        private readonly ITextSpanMapper<List<FuelStationIdentifier>> _mapper;
        private readonly string _url;
        private readonly string _providerName;

        public string DataProvider => _providerName;

        public FuelStationListSource(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, ITextSpanMapper<List<FuelStationIdentifier>> mapper, string url, string providerName)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _mapper = mapper;
            _url = url;
            _providerName = providerName;
        }

        public async Task<Option<List<FuelStationIdentifier>>> GetStationListAsync()
        {
            return await _memoryCache.GetOrCreateAsync(_providerName, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                return await QueryStationListAsync();
            });
        }

        private async Task<Option<List<FuelStationIdentifier>>> QueryStationListAsync()
        {
            var body = await GetHttpBodyAsync(_url);

            if (body is null)
                return Option<List<FuelStationIdentifier>>.None();

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
    }
}
