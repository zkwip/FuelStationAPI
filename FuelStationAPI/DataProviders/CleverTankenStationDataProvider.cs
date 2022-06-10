namespace FuelStationAPI.DataProviders
{
    public class CleverTankenStationDataProvider : BaseFuelStationDataProvider
    {
        public CleverTankenStationDataProvider(HttpClient client, ILogger<BaseFuelStationDataProvider> logger) : base(client, logger)
        {
            _stationDetailUrlPrefix = "https://www.clever-tanken.de/tankstelle_details/";
            _stationListUrl = "https://www.clever-tanken.de/tankstelle_liste?lat=51.808520305084&lon=5.96523930478924&ort=47559+Kranenburg%2FWyler&spritsorte=5&r=15";
        }

        public override IEnumerable<FuelStationData> ExtractStations(string msg)
        {
            List<FuelStationData> list = new();
            List<Geolocation> locations = new();
            StringScraper scraper = new StringScraper(msg);

            scraper.ReadTo("var firstTimeToggle = true;");

            // Locations are seperate and incomplete - scrape the locations first
            while (true)
            {
                try
                {
                    scraper.ReadTo("addPoi('");
                    double lat = scraper.ReadCommaDecimalTo("', '");
                    double lng = scraper.ReadCommaDecimalTo("','','");
                    scraper.ReadTo("',null,2209,2210,60");
                    locations.Add(new(lat, lng));
                }
                catch (ScrapeException)
                {
                    break;
                }
            }

            scraper = new(msg);
            scraper.ReadTo("<div id=\"main-column-container\" class=\"col-12\">");
            int id = 0;

            while (true)
            {
                try
                {
                    scraper.ReadTo("<a href=\"/tankstelle_details/");
                    string ident = scraper.ReadTo("\"");
                    scraper.ReadTo("', 'hover', '");
                    string name = scraper.ReadTo("', '");
                    string address = scraper.ReadTo("' )\">");

                    if (id >= locations.Count) 
                        throw new ScrapeException("The number of scraped locations does not match the number of stations");

                    list.Add(new FuelStationData("CleverTanken", ident, name + " " + address, locations[id]));

                    id++;
                }
                catch (ScrapeException)
                {
                    break;
                }
            }
            return list;
        }

        public override bool StationDataSourceCheck(FuelStationData station) => (station.DataPrivider.ToLower() == "clevertanken");

        protected override List<FuelPriceResult> ExtractPrices(string msg)
        {
            List<FuelPriceResult> list = new();

            ExtractPrice(msg, list, "Super E10", FuelType.Euro95);
            ExtractPrice(msg, list, "SuperPlus", FuelType.Euro98);
            ExtractPrice(msg, list, "Diesel", FuelType.Diesel);

            return list;
        }

        private static void ExtractPrice(string msg, List<FuelPriceResult> prices, string handle, FuelType type)
        {
            try
            {
                StringScraper scraper = new(msg);

                scraper.ReadTo("<div class=\"price-type-name\">" + handle + "</div>");
                scraper.ReadTo("<div class=\"price-field\">");
                double price = scraper.ReadDecimalTo("</div>");

                prices.Add(new(type, price));
            }
            catch (ScrapeException) { }
        }
    }
}
