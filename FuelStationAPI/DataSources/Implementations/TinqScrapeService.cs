using FuelStationAPI.Domain;
using Microsoft.Extensions.Caching.Memory;
using TextScanner.Pattern;

namespace FuelStationAPI.DataSources.Implementations
{
    public class TinqScrapeService : BaseScrapeService
    {
        public TinqScrapeService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, ILogger<BaseScrapeService> logger) : base(httpClientFactory, memoryCache, logger, "tinq", false) { }
        protected override string StationListUrl => "https://www.tinq.nl/tankstations";
        protected override string PriceUrlBuilder(FuelStationIdentifier station) => $"https://www.tinq.nl/tankstations/{station.Identifier}";

        protected override ScanPattern StationPattern => ScanPattern.Create()
            .AddEnclosedGetter("lat", "data-lat=\"", "\"")
            .AddEnclosedGetter("lng", "data-lng=\"", "\"")
            .AddEnclosedGetter("name", "<span class=\"field-content\"><h2>", "</h2>")
            .AddEnclosedGetter("identifier", "<span class=\"field-content\"><a href=\"/tankstations/", "#default");
        protected override ScanPattern PricePattern => ScanPattern.Create()
            .AddEnclosedGetter("type", "<div class=\"node node--type-price node--view-mode-default taxonomy-term-", " ds-1col clearfix\">")
            .AddEnclosedGetter("price", "<div content=\"", "\" class=\"field field--name-field-prices-price-pump field--type-float field--label-hidden field__item\">");
    }
}