using FuelStationAPI.Domain;

namespace FuelStationAPI.DataSources.Implementations
{
    public class TestScrapeService : IDataSource
    {
        public string DataProvider => "test";

        public Task<Option<List<FuelPriceResult>>> GetPricesAsync(FuelStationIdentifier station)
        {
            var list = new List<FuelPriceResult>
            {
                new FuelPriceResult(FuelType.Euro95_E10, 2.0)
            };

            return Task.FromResult<Option<List<FuelPriceResult>>>(list);
        }

        public Task<Option<List<FuelStationIdentifier>>> GetStationListAsync()
        {
            var list = new List<FuelStationIdentifier>
            {
                new FuelStationIdentifier(DataProvider,"henk","Henk", Geolocation.Veghel)
            };

            return Task.FromResult<Option<List<FuelStationIdentifier>>>(list);
        }
    }
}