using Microsoft.Extensions.Caching.Memory;
using TextScraper;

namespace FuelStationAPI.DataProviders
{
    public class ImprovedTinqStationDataProvider : BaseFuelStationDataProvider
    {
        public ImprovedTinqStationDataProvider(HttpClient client, ILogger<BaseFuelStationDataProvider> logger, IMemoryCache cache) : base(client, logger, cache)
        {
            _stationDetailUrlPrefix = "https://www.tinq.nl/tankstations/";
            _stationListUrl = "https://www.tinq.nl/tankstations";
        }

        protected override string StationProviderName => "tinq";

        public override IEnumerable<FuelStationData> ExtractStations(string msg)
        {
            const string latPrefix = "data-lat=\"";
            const string lngPrefix = "data-lng=\"";
            const string namePrefix = "<span class=\"field-content\"><h2>";
            const string nameSuffix = "</h2>";
            const string identifierPrefix = "<span class=\"field-content\"><a href=\"/tankstations/";
            const string identifierSuffix = "#default";

            ScrapePattern pattern = ScrapePattern.Create()
                .AddEnclosedGetter("lat", latPrefix, "\"")
                .AddEnclosedGetter("lng", lngPrefix, "\"")
                .AddEnclosedGetter("name", namePrefix, nameSuffix)
                .AddEnclosedGetter("identifier", identifierPrefix, identifierSuffix);

            Scraper scraper = new(msg);
            List<FuelStationData> res = new();

            while (true)
            {
                try
                {
                    var result = pattern.Run(scraper);
                    
                    if (!result.Succes) 
                        break;

                    var lat = result["lat"].ToDouble();
                    var lng = result["lng"].ToDouble();
                    var name = result["name"].ToString();
                    var identifier = result["identifier"].ToString();

                    FuelStationData station = new("TINQ", identifier, "TINQ " + name, new Geolocation(lat, lng));
                    res.Add(station);
                }
                catch (ScrapeException) { break; }
            }
            return res;
        }

        protected override List<FuelPriceResult> ExtractPrices(string msg)
        {
            List<FuelPriceResult> list = new();

            ExtractPrice(msg, list, "Euro95 E10", FuelType.Euro95_E10);
            ExtractPrice(msg, list, "Superplus 98 E5", FuelType.Euro98_E5);
            ExtractPrice(msg, list, "Diesel B7", FuelType.Diesel);

            return list;
        }

        private static void ExtractPrice(string msg, List<FuelPriceResult> prices, string handle, FuelType type)
        {
            Scraper scraper = new(msg);

            const string pricePrefix = "<div content=\"";
            const string priceSuffix = "\" class=\"field field--name-field-prices-price-pump field--type-float field--label-hidden field__item\">";

            ScrapePattern pattern = ScrapePattern.Create()
                .AddHandle("<div class=\"node node--type-price node--view-mode-default taxonomy-term-" + handle + " ds-1col clearfix\">")
                .AddEnclosedGetter("price", pricePrefix, priceSuffix);

            try
            {
                var price = pattern.Run(scraper)["price"].ToDouble();

                prices.Add(new(type, price));
            }
            catch (ScrapeException) { }
        }
    }
}