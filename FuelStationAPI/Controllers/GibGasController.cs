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

        [HttpGet("GetStations")]
        public async Task<List<FuelStationIdentifier>> GetStationsAsync() 
        {
            var results = await Task.WhenAll(_stationListSources.Select(x => x.GetStationListAsync()));
            var list = new List<FuelStationIdentifier>();

            foreach (var item in results)
            {
                if (item.Succes)
                {
                    list.AddRange(item.Result);
                }
            }

            return list;
        }

        [HttpGet("GetPrices")]
        public async Task<MappedScanResult<List<FuelPriceResult>>> GetPricesAsync(FuelStationIdentifier station)
        {
            IFuelPriceDataSource? priceSource = GetPriceSource(station);
            if (priceSource is null) 
                return MappedScanResult<List<FuelPriceResult>>.Fail("no data provider");

            return await priceSource.GetPricesAsync(station);
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
