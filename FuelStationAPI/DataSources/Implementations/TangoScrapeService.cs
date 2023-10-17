using FuelStationAPI.Domain;
using Microsoft.Extensions.Caching.Memory;
using TextScanner.Pattern;

namespace FuelStationAPI.DataSources.Implementations
{
    public class TangoScrapeService : BaseScrapeService
    {
        public TangoScrapeService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, ILogger<BaseScrapeService> logger) : base(httpClientFactory, memoryCache, logger, "tango", false) { }
        protected override string StationListUrl => "https://www.tango.nl/get/stations.json";
        protected override string PriceUrlBuilder(FuelStationIdentifier station) => $"https://www.tango.nl/stations/{station.Identifier}";

        protected override ScanPattern StationPattern => ScanPattern.Create()
            .AddHandle("StationId")
            .AddEnclosedGetter("identifier", "NodeURL\":\"\\/stations\\/", "\",")
            .AddEnclosedGetter("name", "Name\":\"", "\",")
            .AddEnclosedGetter("lat", "XCoordinate\":\"", "\",")
            .AddEnclosedGetter("lng", "YCoordinate\":\"", "\",");
        protected override ScanPattern PricePattern => ScanPattern.Create()
            .AddHandle("<div class=\"pricing ")
            .AddEnclosedGetter("type", "\" id=\"", "\">")
            .AddHandle("<div class=\"pump_price\">")
            .AddEnclosedGetter("price", "<span class=\"price\">", "</span> EUR");
    }
}