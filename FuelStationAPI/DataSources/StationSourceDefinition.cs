using TextScanner;

namespace FuelStationAPI.DataSources
{
    public class StationSourceDefinition
    {
        private readonly ScanPattern _stationPattern;
        private readonly IScanResultMapper<FuelStationScrapeResult> _stationMapper;

        private readonly Func<FuelStationData, string> _urlBuilder;
        private readonly string _name;

        public StationSourceDefinition(Func<FuelStationData, string> urlBuilder, string name, ScanPattern stationPattern, IScanResultMapper<FuelStationScrapeResult> stationMapper)
        {
            _urlBuilder = urlBuilder;
            _name = name;
            _stationMapper = stationMapper;
            _stationPattern = stationPattern;
        }

        public string GetUrl(FuelStationData station) => _urlBuilder.Invoke(station);

        public FuelStationScrapeResult Scrape(TextScanner.TextScanner scraper) => _stationMapper.Map(_stationPattern.RunOn(scraper));

    }
}
