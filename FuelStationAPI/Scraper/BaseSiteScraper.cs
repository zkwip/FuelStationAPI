namespace FuelStationAPI.Scraper
{
    public abstract class BaseSiteScraper : IFuelSiteScraper
    {
        protected string _stationDetailUrlPrefix;
        protected string _stationListUrl;

        protected readonly HttpClient _client;
        protected readonly ILogger _logger;

        public BaseSiteScraper(HttpClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;

            _stationDetailUrlPrefix = "";
            _stationListUrl = "";
        }

        public virtual async Task<IEnumerable<FuelStationData>> ScrapeStationListAsync()
        {
            HttpResponseMessage message = await _client.GetAsync(_stationListUrl);
            string msg = await message.Content.ReadAsStringAsync();
            return ExtractStations(msg);
        }

        public abstract bool StationBrandCheck(FuelStationData station);

        public abstract IEnumerable<FuelStationData> ExtractStations(string msg);

        public virtual async Task<FuelStationScrapeResult> ScrapeStationPricesAsync(FuelStationData station)
        {
            string url = _stationDetailUrlPrefix + station.Identifier;
            HttpResponseMessage message = await _client.GetAsync(url);

            if (!message.IsSuccessStatusCode)
                return new FuelStationScrapeResult(station, new Exception(message.ReasonPhrase));

            string msg = await message.Content.ReadAsStringAsync();

            List<FuelPriceResult> prices = ExtractPrices(msg);

            if (prices.Count == 0)
            {
                _logger.Log(LogLevel.Information, "The station {station} did not result any fuel prices", station.Name);
                return new FuelStationScrapeResult(station, new Exception("No prices found"));
            }

            return new FuelStationScrapeResult(station, prices);
        }

        protected abstract List<FuelPriceResult> ExtractPrices(string msg);

    }
}

