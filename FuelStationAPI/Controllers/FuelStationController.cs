using ASPWebAPITest.Scraper;
using Microsoft.AspNetCore.Mvc;

namespace ASPWebAPITest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FuelStationController : ControllerBase
    {
        private readonly ILogger<FuelStationController> _logger;
        private readonly IEnumerable<IFuelSiteScraper> _scrapers;
        private Geolocation _location;

        public FuelStationController(ILogger<FuelStationController> logger, IEnumerable<IFuelSiteScraper> scrapers)
        {
            _logger = logger;
            _scrapers = scrapers;
            _location = Geolocation.Strijp;
        }

        [HttpGet("GetFuelStations")]
        public async Task<FuelCostComparison> GetFuelStationsAsync()
        {
            bool fuelRemSucces = Double.TryParse(HttpContext.Request.Query["fuelRemaining"], out double fuelRemaining);

            bool geoSucces = Double.TryParse(HttpContext.Request.Query["lat"], out double latitude);
            geoSucces &= Double.TryParse(HttpContext.Request.Query["long"], out double longtitude);


            if (geoSucces)
                _location = new Geolocation(latitude, longtitude);

            IEnumerable<FuelStationData> stations = await ListFilteredStationsAsync();

            if (fuelRemSucces && geoSucces)
                return await CompareIncludingDistance(_location, fuelRemaining, stations);

            if (geoSucces)
                return await CompareIncludingDistance(_location, stations);

            return await ComparePricesOnly(stations);
        }

        [HttpGet("ListAllStations")]
        public async Task<IEnumerable<FuelStationData>> ListAllStationsAsync()
        {
            IEnumerable<FuelStationData>[] lists = await Task.WhenAll(_scrapers.Select(x => x.ScrapeStationListAsync()));
            return lists.SelectMany(x => x);
        }

        [HttpGet("ListFilteredStations")]
        public async Task<IEnumerable<FuelStationData>> ListFilteredStationsAsync()
        {
            IEnumerable<FuelStationData> list = await ListAllStationsAsync();
            list = list.Where(x => Geolocation.Distance(x.Location, Geolocation.Strijp) < 5);
            return list;
        }

        private async Task<FuelCostComparison> ComparePricesOnly(IEnumerable<FuelStationData> stations, FuelType type = FuelType.Euro95)
        {
            IEnumerable<FuelStationScrapeResult> scrapes = await ScrapeStations(stations);
            return new FuelCostComparison(scrapes, r => r.FuelPrices.Where(x => x.FuelType == type).Select(x => x.Price).Min()); ;
        }

        private async Task<IEnumerable<FuelStationScrapeResult>> ScrapeStations(IEnumerable<FuelStationData> stations)
        {
            List<Task<FuelStationScrapeResult?>> scrapeTasks = new();
            List<FuelStationScrapeResult> res = new();

            foreach (FuelStationData station in stations)
                scrapeTasks.Add(ScrapeStation(station));

            foreach (FuelStationScrapeResult? r in await Task.WhenAll(scrapeTasks))
                if (r is not null)
                    res.Add(r);

            return res;
        }

        private async Task<FuelStationScrapeResult?> ScrapeStation(FuelStationData station)
        {
            foreach (IFuelSiteScraper scraper in _scrapers)
            {
                if (!scraper.StationBrandCheck(station))
                    continue;

                return await scraper.ScrapeStationPricesAsync(station);
            }

            return null;
        }

        private Task<FuelCostComparison> CompareIncludingDistance(Geolocation geolocation, IEnumerable<FuelStationData> stations)
        {
            return ComparePricesOnly(stations);
        }

        private Task<FuelCostComparison> CompareIncludingDistance(Geolocation geolocation, double fuelRemaining, IEnumerable<FuelStationData> stations)
        {
            return ComparePricesOnly(stations);
        }
    }
}
