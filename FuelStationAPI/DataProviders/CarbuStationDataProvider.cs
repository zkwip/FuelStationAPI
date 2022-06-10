namespace FuelStationAPI.DataProviders
{
    public class CarbuStationDataProvider : BaseFuelStationDataProvider
    {
        public CarbuStationDataProvider(HttpClient client, ILogger<BaseFuelStationDataProvider> logger) : base(client, logger)
        {
            _stationDetailUrlPrefix = "https://carbu.com/belgie/index.php/station/";
            _stationListUrl = "https://carbu.com/belgie//liste-stations-service/E10/Bilzen/3740/BE_li_701";
        }

        public override IEnumerable<FuelStationData> ExtractStations(string msg)
        {
            List<FuelStationData> list = new();
            StringScraper scraper = new StringScraper(msg);

            while (true)
            {
                try
                {
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

        public override bool StationDataSourceCheck(FuelStationData station) => (station.DataPrivider.ToLower() == "carbu");

        protected override List<FuelPriceResult> ExtractPrices(string msg)
        {
            List<FuelPriceResult> list = new();
            ExtractPrice(msg, list, "Super 95 (E10)", FuelType.Euro95);
            ExtractPrice(msg, list, "Super 98 (E5)", FuelType.Euro98);
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
