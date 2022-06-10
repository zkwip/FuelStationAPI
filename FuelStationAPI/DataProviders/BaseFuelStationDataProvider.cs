namespace FuelStationAPI.DataProviders
{
    public abstract class BaseFuelStationDataProvider : IFuelStationDataProvider
    {
        protected string _stationDetailUrlPrefix;
        protected string _stationListUrl;

        protected readonly HttpClient _client;
        protected readonly ILogger<BaseFuelStationDataProvider> _logger;

        public BaseFuelStationDataProvider(HttpClient client, ILogger<BaseFuelStationDataProvider> logger)
        {
            _client = client;
            _logger = logger;

            _stationDetailUrlPrefix = "";
            _stationListUrl = "";
        }

        public virtual async Task<IEnumerable<FuelStationData>> ScrapeStationListAsync()
        {
            using HttpResponseMessage message = await _client.GetAsync(_stationListUrl);
            string msg = await message.Content.ReadAsStringAsync();
            return ExtractStations(msg);
        }

        public abstract bool StationDataSourceCheck(FuelStationData station);

        public abstract IEnumerable<FuelStationData> ExtractStations(string msg);

        public virtual async Task<FuelStationScrapeResult> ScrapeStationPricesAsync(FuelStationData station)
        {
            string url = _stationDetailUrlPrefix + station.Identifier;
            using HttpResponseMessage message = await _client.GetAsync(url);

            if (!message.IsSuccessStatusCode)
                return new FuelStationScrapeResult(station, new Exception(message.ReasonPhrase));

            string msg = await message.Content.ReadAsStringAsync();

            List<FuelPriceResult> prices = ExtractPrices(msg);

            if (prices.Count == 0)
            {
                _logger.Log(LogLevel.Information, "The station {station} ({identifier}) did not provide any fuel prices", station.Name, station.Identifier);
                return new FuelStationScrapeResult(station, new Exception("No prices found"));
            }

            return new FuelStationScrapeResult(station, prices);
        }

        protected abstract List<FuelPriceResult> ExtractPrices(string msg);

    }
}

