namespace ASPWebAPITest.Scraper
{
    public class CarbuSiteScraper : BaseSiteScraper
    {
        public CarbuSiteScraper(HttpClient client, ILogger logger) : base(client, logger)
        {
            _stationDetailUrlPrefix = "https://carbu.com/belgie/index.php/station/";
        }

        public override IEnumerable<FuelStationData> ExtractStations(string msg)
        {
            throw new NotImplementedException();
        }

        public override bool StationBrandCheck(FuelStationData station)
        {
            throw new NotImplementedException();
        }

        protected override List<FuelPriceResult> ExtractPrices(string msg)
        {
            throw new NotImplementedException();
        }
    }
}
