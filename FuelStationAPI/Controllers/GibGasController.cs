using FuelStationAPI.DataSources;
using Microsoft.AspNetCore.Mvc;
using TextScanner;

namespace FuelStationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GibGasController : ControllerBase
    {
        private readonly ILogger<FuelStationController> _logger;
        private readonly IEnumerable<IFuelPriceDataSource> _fuelPriceDataSources;
        private readonly IEnumerable<IFuelStationDetailSource> _stationDetailSources;
        private readonly IEnumerable<IFuelStationListSource> _stationListSources;
        private Geolocation _location;

        public GibGasController(ILogger<FuelStationController> logger, IEnumerable<IFuelPriceDataSource> fuelPriceDataSources, IEnumerable<IFuelStationDetailSource> stationDetailSources, IEnumerable<IFuelStationListSource> stationListSources)
        {
            _logger = logger;
            _location = Geolocation.Ooij;

            _fuelPriceDataSources = fuelPriceDataSources;
            _stationDetailSources = stationDetailSources;
            _stationListSources = stationListSources;
        }

        private readonly Predicate<FuelStationIdentifier> AllStations = s => true;

        [HttpGet("GetStations")]
        public async Task<List<FuelStationIdentifier>> GetStationsAsync(Predicate<FuelStationIdentifier>? which)
        {
            var results = await Task.WhenAll(_stationListSources.Select(x => x.GetStationListAsync()));
            var list = new List<FuelStationIdentifier>();


            if (which is null)
                which = AllStations;

            foreach (var item in results)
            {
                if (item.Succes)
                {
                    list.AddRange(item.Result.FindAll(which));
                }
            }

            return list;
        }

        [HttpGet("GetPrices")]
        public async Task<List<FuelPriceResult>> GetPricesAsync(FuelStationIdentifier station) => await GetPricesAsync(station, null);
        private async Task<List<FuelPriceResult>> GetPricesAsync(FuelStationIdentifier station, Predicate<FuelType>? fuels)
        { 
            IFuelPriceDataSource? priceSource = GetPriceSource(station);
            if (priceSource is null)
                throw new Exception();

            if (fuels is null)
                fuels = FuelTypeExtensions.Gasoline;

            var prices = await priceSource.GetPricesAsync(station);
            
            if (!prices.Succes)
                _logger.LogWarning(prices.Message);
            
            var list = prices.Result;

            if(list.Count == 0)
                _logger.LogWarning("no prices found for station {0}", station);


            return list.FindAll(x => fuels(x.FuelType));
        }

        [HttpGet("ListFilteredStations")]
        public async Task<IEnumerable<FuelStationIdentifier>> ListFilteredStationsAsync(Predicate<FuelStationIdentifier>? which = null)
        {
            IEnumerable<FuelStationIdentifier> list = await GetStationsAsync(which);
            list = list.Where(x => Geolocation.Distance(x.Location, _location) < 15);
            return list;
        }

        private async Task<FuelStationPricesPair> GetStationPricePairAsync(FuelStationIdentifier station, Predicate<FuelType>? fuels = null)
        {
            var prices = await GetPricesAsync(station, fuels);
            return new FuelStationPricesPair(station, prices);
        }

        [HttpGet("GetAllPrices")]
        public async Task<List<FuelStationPricesPair>> GetAllPricesAsync() => await GetAllPricesAsync(null, null);

        [HttpGet("GetClosePrices")]
        public async Task<List<FuelStationPricesPair>> GetClosePricesAsync() => await GetAllPricesAsync(x => Geolocation.Distance(x.Location,_location) < 20.0, null);


        private async Task<List<FuelStationPricesPair>> GetAllPricesAsync(Predicate<FuelStationIdentifier>? which = null, Predicate<FuelType>? fuels = null)
        {
            var stations = await GetStationsAsync(which);

            var pairs = await Task.WhenAll(stations.Select(x => GetStationPricePairAsync(x, fuels)));
            return pairs.Where(x => x.Prices.Any()).ToList();
        }

        private IFuelPriceDataSource? GetPriceSource(FuelStationIdentifier station)
        {
            foreach(IFuelPriceDataSource source in _fuelPriceDataSources)
            {
                if (station.DataPrivider == source.DataProvider) 
                    return source;
            }

            _logger.LogWarning("Could not find a suitable PriceDataSource for the provider handle {0}", station.DataPrivider);

            return null; 
        }
    }
}
