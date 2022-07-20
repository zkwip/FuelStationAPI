using TextScanner;

namespace FuelStationAPI.DataSources
{
    public class StationSourceDefinition
    {
        private readonly ScanPattern _stationPattern;
        private readonly IScanResultMapper<FuelStationScrapeResult> _stationMapper;

        private readonly Func<FuelStationData, string> _urlBuilder;

        public StationSourceDefinition(Func<FuelStationData, string> urlBuilder, ScanPattern stationPattern, IScanResultMapper<FuelStationScrapeResult> stationMapper)
        {
            _urlBuilder = urlBuilder;
            _stationMapper = stationMapper;
            _stationPattern = stationPattern;
        }

        public string GetUrl(FuelStationData station) => _urlBuilder.Invoke(station);

        public FuelStationScrapeResult Scrape(Scanner scraper) => _stationMapper.Map(_stationPattern.RunOn(scraper));

    }
}
