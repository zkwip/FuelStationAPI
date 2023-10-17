using FuelStationAPI.Domain;
using Microsoft.Extensions.Caching.Memory;
using TextScanner.Pattern;

namespace FuelStationAPI.DataSources.Implementations
{
    public class TankBilligScrapeService : BaseScrapeService
    {
        public TankBilligScrapeService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, ILogger<BaseScrapeService> logger) : base(httpClientFactory, memoryCache, logger, "tankbillig", false) { }
        protected override string StationListUrl => "https://tankbillig.info/index.php?lat=51.7833&long=6.0167";
        protected override string PriceUrlBuilder(FuelStationIdentifier station) => $"https://ich-tanke.de/tankstelle/{station.Identifier}";

        protected override ScanPattern StationPattern => ScanPattern.Create()
            .AddEnclosedGetter("identifier", "\"stationID\":\"", "\",")
            .AddEnclosedGetter("name", "\"gasStationName\":\"", "\", \"brand\":\"")
            .AddEnclosedGetter("lng", "\", \"longitude\":\"", "\",")
            .AddEnclosedGetter("lat", "\", \"latitude\":\"", "\",");
        protected override ScanPattern PricePattern => ScanPattern.Create()
            .AddHandle("<div class=\"pricing ")
            .AddEnclosedGetter("type", "\" id=\"", "\">")
            .AddHandle("<div class=\"pump_price\">")
            .AddEnclosedGetter("price", "<span class=\"price\">", "</span> EUR");
    }
}