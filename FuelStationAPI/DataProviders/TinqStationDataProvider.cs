using Microsoft.Extensions.Caching.Memory;

namespace FuelStationAPI.DataProviders
{
    public class TinqStationDataProvider : BaseFuelStationDataProvider
    {
        public TinqStationDataProvider(HttpClient client, ILogger<BaseFuelStationDataProvider> logger, IMemoryCache cache) : base(client, logger, cache)
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

            StringScraper scraper = new(msg);
            List<FuelStationData> res = new();

            while (true)
            {
                try
                {
                    scraper.ReadTo(handle_lat);
                    double lat = scraper.ReadDecimalTo("\"");

                    scraper.ReadTo(handle_lng);
                    double lng = scraper.ReadDecimalTo("\"");

                    scraper.ReadTo(handle_name);
                    string name = scraper.ReadTo(handle_name_end);

                    scraper.ReadTo(handle_identifier);
                    string identifier = scraper.ReadTo(handle_identifier_end);

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
                StringScraper scraper = new(msg);
                if (!scraper.TestReadTo("<div class=\"node node--type-price node--view-mode-default taxonomy-term-" + handle + " ds-1col clearfix\">")) return;
                scraper.ReadTo("<div class=\"node node--type-price node--view-mode-default taxonomy-term-" + handle + " ds-1col clearfix\">");
                scraper.ReadTo("<div content=\"");
                double price = scraper.ReadDecimalTo("\" class=\"field field--name-field-prices-price-pump field--type-float field--label-hidden field__item\">");

                prices.Add(new(type, price));
            }
            catch (ScrapeException) { }
        }
    }
}