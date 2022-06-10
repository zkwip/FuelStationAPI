namespace FuelStationAPI.DataProviders
{
    public class ArgosStationDataProvider : BaseFuelStationDataProvider
    {
        public ArgosStationDataProvider(HttpClient client, ILogger<BaseFuelStationDataProvider> logger) : base(client, logger)
        {
            _stationDetailUrlPrefix = "https://www.argos.nl/tankstation/";
            _stationListUrl = "https://www.argos.nl/tankstations/";
        }
        public override IEnumerable<FuelStationData> ExtractStations(string msg)
        {
            List<FuelStationData> list = new();
            StringScraper scraper = new StringScraper(msg);

            scraper.ReadTo("<div id=\"tankstation-map\">");

            while (true)
            {
                try
                {
                    scraper.ReadTo("<div class=\"marker\" data-id=\"");

                    scraper.ReadTo("data-lat=\"");
                    double lat = scraper.ReadDecimalTo("\"");

                    scraper.ReadTo("data-lng=\"");
                    double lng = scraper.ReadDecimalTo("\"");

                    scraper.ReadTo("<strong>");
                    string name = scraper.ReadTo("</strong>");

                    scraper.ReadTo("<a href=\"https://www.argos.nl/tankstation/");
                    string ident = scraper.ReadTo("\">Bekijk</a>");

                    list.Add(new FuelStationData("Argos", ident, name, new(lat, lng)));
                }
                catch (ScrapeException)
                {
                    break;
                }
            }
            return list;
        }

        public override bool StationDataSourceCheck(FuelStationData station) => (station.DataPrivider.ToLower() == "argos");

        protected override List<FuelPriceResult> ExtractPrices(string msg)
        {
            List<FuelPriceResult> list = new();

            ExtractPrice(msg, list, "Euro 95", FuelType.Euro95);
            ExtractPrice(msg, list, "Superplus 98", FuelType.Euro98);
            ExtractPrice(msg, list, "Diesel", FuelType.Diesel);

            return list;
        }

        private static void ExtractPrice(string msg, List<FuelPriceResult> prices, string handle, FuelType type)
        {
            try
            {
                StringScraper scraper = new(msg);

                scraper.ReadTo("<label class=\"" + handle + "\">Euro 95</label>");
                scraper.ReadTo("<span class=\"price\"> ");
                double price = scraper.ReadDecimalTo("</sup> </span>");

                prices.Add(new(type, price));
            }
            catch (ScrapeException) { }
        }
    }
}
