using Microsoft.Extensions.Caching.Memory;

namespace FuelStationAPI.DataProviders
{
    public class CarbuStationDataProvider : BaseFuelStationDataProvider
    {
        public CarbuStationDataProvider(HttpClient client, ILogger<BaseFuelStationDataProvider> logger, IMemoryCache cache) : base(client, logger, cache)
        {
            _stationDetailUrlPrefix = "https://carbu.com/belgie/index.php/station/";
            _stationListUrl = "https://carbu.com/belgie//liste-stations-service/E10/Bilzen/3740/BE_li_701";
        }

        protected override string StationProviderName => "carbu";

        public override IEnumerable<FuelStationData> ExtractStations(string msg)
        {
            List<FuelStationData> list = new();
            StringScraper scraper = new(msg);

            while (true)
            {
                try
                {
                    if (!scraper.TestReadTo("<div class=\"station-content col-xs-12\">")) break;
                    scraper.ReadTo("<div class=\"station-content col-xs-12\">");

                    scraper.ReadTo("data-lat=\"");
                    double lat = scraper.ReadDecimalTo("\"");

                    scraper.ReadTo("data-lng=\"");
                    double lng = scraper.ReadDecimalTo("\"");

                    scraper.ReadTo("data-name=\"");
                    string name = scraper.ReadTo("\"");

                    scraper.ReadTo("data-link=\"https://carbu.com/belgie/index.php/station/");
                    string ident = scraper.ReadTo("\"");

                    list.Add(new FuelStationData("carbu", ident, name, new(lat, lng)));
                }
                catch (ScrapeException)
                {
                    break;
                }
            }
            return list;
        }

        protected override List<FuelPriceResult> ExtractPrices(string msg)
        {
            List<FuelPriceResult> list = new();
            ExtractPrice(msg, list, "Super 95 (E10)", FuelType.Euro95_E10);
            ExtractPrice(msg, list, "Super 98 (E5)", FuelType.Euro98_E5);
            ExtractPrice(msg, list, "Diesel (B7)", FuelType.Diesel);
            return list;
        }

        private void ExtractPrice(string msg, List<FuelPriceResult> list, string handle, FuelType type)
        {
            try
            {
                StringScraper scraper = new(msg);
                scraper.ReadTo("<div class=\"panel-heading\" style=\"text-align:center\">");
                scraper.ReadTo("<h2 class=\"title\">" + handle + "</h2>");
                scraper.ReadTo("<h1 class=\"price\">");

                double price = scraper.ReadCommaDecimalTo(" &euro;/L</h1>");

                list.Add(new(type, price));
            }
            catch (ScrapeException) { }
        }
    }
}
