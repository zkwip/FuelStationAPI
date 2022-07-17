using TextScraper;

namespace FuelStationAPI.DataSources
{
    public class StationSourceDefinition
    {
        private readonly ScrapePattern _stationPattern;
        private readonly IScrapeResultMapper<FuelStationScrapeResult> _stationMapper;

        private readonly Func<FuelStationData, string> _urlBuilder;
        private readonly string _name;

        public StationSourceDefinition(Func<FuelStationData, string> urlBuilder, string name, ScrapePattern stationPattern, IScrapeResultMapper<FuelStationScrapeResult> stationMapper)
        {
            _urlBuilder = urlBuilder;
            _name = name;
            _stationMapper = stationMapper;
            _stationPattern = stationPattern;
        }

        public string GetUrl(FuelStationData station) => _urlBuilder.Invoke(station);

        public FuelStationScrapeResult Scrape(Scraper scraper) => _stationMapper.Map(_stationPattern.RunOn(scraper));

    }
}
