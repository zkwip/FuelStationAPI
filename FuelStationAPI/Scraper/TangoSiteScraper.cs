namespace ASPWebAPITest.Scraper
{
    public class TangoSiteScraper : BaseSiteScraper
    {
        public TangoSiteScraper(HttpClient client, ILogger logger) : base(client, logger)
        {
            _stationDetailUrlPrefix = @"https://www.tango.nl/stations/";
        }

        public async override Task<IEnumerable<FuelStationData>> ScrapeStationListAsync()
        {
            FuelStationData[] stations = { new FuelStationData("tango", "tango-eindhoven-kennedylaan", "Tango Eindhoven Kennedylaan", Geolocation.Strijp) };
            return stations.AsEnumerable();
        }

        public async override Task<FuelStationScrapeResult> ScrapeStationPricesAsync(FuelStationData station)
        {
            string url = _stationDetailUrlPrefix + station.Identifier;
            HttpResponseMessage message = await _client.GetAsync(url);

            if (!message.IsSuccessStatusCode)
                return new FuelStationScrapeResult(station, new Exception(message.ReasonPhrase));

            string msg = await message.Content.ReadAsStringAsync();
            List<FuelPriceResult> list = ExtractPrices(msg);

            if (list.Count == 0)
            {
                _logger.Log(LogLevel.Information, "The station {station} did not result any fuel prices", station.Name);
                return new FuelStationScrapeResult(station, new Exception("No prices found"));
            }

            return new FuelStationScrapeResult(station, list);
        }

        protected override List<FuelPriceResult> ExtractPrices(string msg)
        {
            List<FuelPriceResult> list = new();

            ExtractPrice(msg, list, "euro95", FuelType.Euro95);
            ExtractPrice(msg, list, "euro98", FuelType.Euro98);
            ExtractPrice(msg, list, "diesel", FuelType.Diesel);
            return list;
        }

        private static void ExtractPrice(string msg, List<FuelPriceResult> list, string handle, FuelType type)
        {
            StringScraper scraper = new(msg);
            try
            {
                scraper.ReadTo("\" id=\"" + handle + "\">");
                scraper.ReadTo("<div class=\"pump_price\">");
                scraper.ReadTo("<span class=\"price\">");
                list.Add(new(type, scraper.ReadDecimalTo("</span>")));
            }
            catch (ScrapeException) { }
        }

        public override bool StationBrandCheck(FuelStationData station) => (station.Brand.ToLower() == "tango");

        public override IEnumerable<FuelStationData> ExtractStations(string msg)
        {
            throw new NotImplementedException();
        }
    }
}
