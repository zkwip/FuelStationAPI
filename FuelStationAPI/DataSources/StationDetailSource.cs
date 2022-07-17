using Microsoft.Extensions.Caching.Memory;
using TextScraper;

namespace FuelStationAPI.DataSources
{
    public class StationDetailSource : IFuelStationDataSource
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly StationSourceDefinition _definition;

        public StationDetailSource(HttpClient httpClient, IMemoryCache memoryCache, StationSourceDefinition definition)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _definition = definition;
        }

        public async Task<FuelStationScrapeResult> GetPricesAsync(FuelStationData station)
        {
            return await _memoryCache.GetOrCreateAsync(station, async entry => {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                return await QueryStationPricesAsync(station);
            });
        }

        private async Task<FuelStationScrapeResult> QueryStationPricesAsync(FuelStationData station)
        {
            var body = await GetHttpBodyAsync(_definition.GetUrl(station));

            if (body is null) 
                return new(station, new Exception("Could not read the station page"));

            var scraper = new Scraper(body);
            return _definition.Scrape(scraper);

        }
        
        private async Task<string?> GetHttpBodyAsync(string url){
            using var message = await _httpClient.GetAsync(url);

            if (!message.IsSuccessStatusCode)
                return null;

            return await message.Content.ReadAsStringAsync();
        }
    }
}
