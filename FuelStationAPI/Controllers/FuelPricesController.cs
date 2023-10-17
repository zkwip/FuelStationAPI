using FuelStationAPI.Aggregator;
using FuelStationAPI.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FuelStationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FuelPricesController : ControllerBase
    {
        private readonly FuelPricesAggregator _fuelServiceAggregator;

        private readonly Geolocation _location;
        private readonly double _searchDistance = 15.0;
        private readonly double _litersPerKm = 0.07;
        private readonly double _tankSize = 45.0;

        public FuelPricesController(FuelPricesAggregator fuelServiceAggregator)
        {
            _location = Geolocation.Maastricht;
            _fuelServiceAggregator = fuelServiceAggregator;
        }

        [HttpGet("Version")]
        public string Version() => "v0.1.1";

        [HttpGet("GetStations")]
        public IAsyncEnumerable<FuelStationIdentifier> GetStationsAsync() => 
            _fuelServiceAggregator.GetAllStationsAsync();

        [HttpGet("GetCloseStations")]
        public IAsyncEnumerable<FuelStationIdentifier> GetCloseStationsAsync() =>
            _fuelServiceAggregator.GetAllStationsAsync()
                .Where(station => station.IsCloseTo(_searchDistance, _location));

        [HttpGet("GetClosePrices")]
        public IAsyncEnumerable<FuelStationPriceData> GetClosePricesAsync() =>
            _fuelServiceAggregator.GetAllStationsAsync()
                .Where(station => station.IsCloseTo(_searchDistance, _location))
                .AggregatePrices(_fuelServiceAggregator)
                .OrderBy(data => data.Prices.Min(price => price.Price));

        [HttpGet("GetCloseE95Prices")]
        public IAsyncEnumerable<FuelStationPriceData> GetCloseE95PricesAsync() =>
            _fuelServiceAggregator.GetAllStationsAsync()
                .Where(station => station.IsCloseTo(_searchDistance, _location))
                .AggregatePrices(_fuelServiceAggregator)
                .OnlyFuel(FuelType.Euro95_E10)
                .OrderBy(data => data.Prices.Min(price => price.Price));


        [HttpGet("GetBestPrices")]
        public IAsyncEnumerable<CostAnalysis<FuelStationPriceData>> GetBestPricesAsync() =>
            _fuelServiceAggregator.GetAllStationsAsync()
                .Where(station => station.IsCloseTo(_searchDistance, _location))
                .AggregatePrices(_fuelServiceAggregator)
                .OnlyFuel(FuelTypeExtensions.Gasoline)
                .ToCostAnalysis(data => data.Prices.Min(price => price.Price) * _tankSize)
                .AddCost(data => 2 * Geolocation.Distance(_location, data.Station.Location) * _litersPerKm * data.Prices.Min(price => price.Price))
                .OrderBy(anal => anal.Cost)
                .Take(5);
    }
}
