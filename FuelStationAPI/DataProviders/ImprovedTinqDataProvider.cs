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
            const string handle_lat = "data-lat=\"";
            const string handle_lng = "data-lng=\"";
            const string handle_name = "<span class=\"field-content\"><h2>";
            const string handle_name_end = "</h2>";
            const string handle_identifier = "<span class=\"field-content\"><a href=\"/tankstations/";
            const string handle_identifier_end = "#default";

            ScrapePattern pattern = ScrapePattern.Create()
                .AddHandle(handle_lat)
                .AddGetter("lat")
                .AddHandle("\"")
                .AddHandle(handle_lng)
                .AddGetter("lng")
                .AddHandle("\"")
                .AddHandle(handle_name)
                .AddGetter("name")
                .AddHandle(handle_name_end)
                .AddHandle(handle_identifier)
                .AddGetter("identifier")
                .AddHandle(handle_identifier_end);

            Scraper scraper = new(msg);
            List<FuelStationData> res = new();

            while (true)
            {
                try
                {
                    var result = pattern.Run(scraper);

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
            try
            {
                Scraper scraper = new(msg);

                ScrapePattern pattern = ScrapePattern.Create()
                    .AddHandle("<div class=\"node node--type-price node--view-mode-default taxonomy-term-" + handle + " ds-1col clearfix\">")
                    .AddHandle("<div content=\"")
                    .AddGetter("price")
                    .AddHandle("\" class=\"field field--name-field-prices-price-pump field--type-float field--label-hidden field__item\">");

                var price = pattern.Run(scraper)["price"].ToDouble();

                prices.Add(new(type, price));
            }
            catch (ScrapeException) { }
        }
    }
}