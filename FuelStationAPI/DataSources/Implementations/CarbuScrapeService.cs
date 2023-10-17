using FuelStationAPI.Domain;
using Microsoft.Extensions.Caching.Memory;
using TextScanner.Pattern;

namespace FuelStationAPI.DataSources.Implementations
{
    public class CarbuScrapeService : BaseScrapeService
    {
        public CarbuScrapeService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, ILogger<BaseScrapeService> logger) : base(httpClientFactory, memoryCache, logger, "carbu", true) { }

        protected override string StationListUrl => "https://carbu.com/belgie//liste-stations-service/E10/Bilzen/3740/BE_li_701";
        protected override string PriceUrlBuilder(FuelStationIdentifier station) => $"https://carbu.com/belgie/index.php/station/{station.Identifier}";

        protected override ScanPattern StationPattern => ScanPattern.Create()
            .AddHandle("<div class=\"station-content col-xs-12\"")
            .AddEnclosedGetter("lat", "data-lat=\"", "\"")
            .AddEnclosedGetter("lng", "data-lng=\"", "\"")
            .AddEnclosedGetter("name", "data-name=\"", "\"")
            .AddEnclosedGetter("identifier", "data-link=\"https://carbu.com/belgie/index.php/station/", "\"");
        protected override ScanPattern PricePattern => ScanPattern.Create()
            .AddHandle("<div class=\"col-xs-12 col-sm-6\">")
            .AddHandle("<div class=\"panel-heading\" style=\"text-align:center\">")
            .AddEnclosedGetter("type", "<h2 class=\"title\">", "</h2>")
            .AddEnclosedGetter("price", "<h1 class=\"price\">", " &euro;");
    }
}