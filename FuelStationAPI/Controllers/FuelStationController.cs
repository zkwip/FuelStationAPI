using FuelStationAPI.DataProviders;
using Microsoft.AspNetCore.Mvc;

namespace FuelStationAPI.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class FuelStationController : ControllerBase
    {
        private readonly ILogger<FuelStationController> _logger;
        private readonly IEnumerable<IFuelStationDataProvider> _scrapers;
        private Geolocation _location;

        public FuelStationController(ILogger<FuelStationController> logger, IEnumerable<IFuelStationDataProvider> scrapers)
        {
            _logger = logger;
            _scrapers = scrapers;
            _location = Geolocation.Ooij;
        }

        [HttpGet("GetFuelStations")]
        public async Task<FuelCostComparison> GetFuelStationsAsync()
        {
            bool fuelRemSucces = Double.TryParse(HttpContext.Request.Query["fuelRemaining"], out double fuelRemaining);

            bool geoSucces = Double.TryParse(HttpContext.Request.Query["lat"], out double latitude);
            geoSucces &= Double.TryParse(HttpContext.Request.Query["long"], out double longtitude);


            if (geoSucces)
                _location = new Geolocation(latitude, longtitude);

            IEnumerable<FuelStationIdentifier> stations = await ListFilteredStationsAsync();

            if (fuelRemSucces && geoSucces)
                return await CompareIncludingDistance(_location, fuelRemaining, stations);

            if (geoSucces)
                return await CompareIncludingDistance(_location, stations);

            return await ComparePricesOnly(stations);
        }

        [HttpGet("ListAllStations")]
        public async Task<IEnumerable<FuelStationIdentifier>> ListAllStationsAsync()
        {
            IEnumerable<FuelStationIdentifier>[] lists = await Task.WhenAll(_scrapers.Select(x => x.ScrapeStationListAsync()));
            return lists.SelectMany(x => x);
        }

        [HttpGet("ListFilteredStations")]
        public async Task<IEnumerable<FuelStationIdentifier>> ListFilteredStationsAsync()
        {
            IEnumerable<FuelStationIdentifier> list = await ListAllStationsAsync();
            list = list.Where(x => Geolocation.Distance(x.Location, _location) < 15);
            return list;
        }

        private async Task<FuelCostComparison> ComparePricesOnly(IEnumerable<FuelStationIdentifier> stations, FuelType type = FuelType.Euro95_E10)
        {
            IEnumerable<FuelStationScrapeResult> scrapes = await ScrapeStations(stations);
            return new FuelCostComparison(scrapes, r => GetFuelPrice(type,r));
        }

        private static double GetFuelPrice(FuelType type, FuelStationScrapeResult result)
        {
            foreach (FuelPriceResult price in result.FuelPrices)
                if (price.FuelType == type) 
                    return price.Price;

            return -1;
        }

        private async Task<IEnumerable<FuelStationScrapeResult>> ScrapeStations(IEnumerable<FuelStationIdentifier> stations)
        {
            List<Task<FuelStationScrapeResult?>> scrapeTasks = new();
            List<FuelStationScrapeResult> res = new();

            foreach (FuelStationIdentifier station in stations)
                scrapeTasks.Add(ScrapeStation(station));

            foreach (FuelStationScrapeResult? r in await Task.WhenAll(scrapeTasks))
                if (r is not null)
                    res.Add(r);

            return res;
        }

        private async Task<FuelStationScrapeResult?> ScrapeStation(FuelStationIdentifier station)
        {
            foreach (IFuelStationDataProvider scraper in _scrapers)
            {
                if (!scraper.StationDataSourceCheck(station))
                    continue;

                return await scraper.ScrapeStationPricesAsync(station);
            }

            return null;
        }

        private Task<FuelCostComparison> CompareIncludingDistance(Geolocation geolocation, IEnumerable<FuelStationIdentifier> stations)
        {
            return ComparePricesOnly(stations);
        }

        private Task<FuelCostComparison> CompareIncludingDistance(Geolocation geolocation, double fuelRemaining, IEnumerable<FuelStationIdentifier> stations)
        {
            return ComparePricesOnly(stations);
        }
    }
}
