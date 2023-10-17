using FuelStationAPI.Domain;
using Microsoft.Extensions.Caching.Memory;
using TextScanner.Pattern;

namespace FuelStationAPI.DataSources.Implementations
{
    public class ArgosScrapeService : BaseScrapeService
    {
        public ArgosScrapeService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, ILogger<BaseScrapeService> logger) : base(httpClientFactory, memoryCache, logger, "argos", false) { }
        protected override string StationListUrl => "https://www.argos.nl/tankstations";
        protected override string PriceUrlBuilder(FuelStationIdentifier station) => $"https://www.argos.nl/tankstations/{station.Identifier}";

        protected override ScanPattern StationPattern => ScanPattern.Create()
            .AddHandle("<div class=\"marker\" data-id=\"")
            .AddEnclosedGetter("lat", "data-lat=\"", "\"")
            .AddEnclosedGetter("lng", "data-lng=\"", "\"")
            .AddEnclosedGetter("name", "<strong>", "</strong>")
            .AddEnclosedGetter("identifier", "<a href=\"https://www.argos.nl/tankstation/", "\">Bekijk</a>");
        protected override ScanPattern PricePattern => ScanPattern.Create()
            .AddHandle("<div class=\"col col4 price-item\">")
            .AddEnclosedGetter("type", "<label class=\"name\">", "</label>")
            .AddEnclosedGetter("price", "<span class=\"price\"> ", "</sup> </span>");
    }
}