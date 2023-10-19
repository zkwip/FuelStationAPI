using FuelStationAPI.Domain;

namespace FuelStationAPI.DataSources.Implementations
{
    public class DebuggingDataSource : IDataSource
    {
        public string DataProvider => "debugger";

        public Task<Option<List<FuelPriceResult>>> GetPricesAsync(Station station)
        {
            var list = new List<FuelPriceResult>
            {
                new FuelPriceResult(FuelType.Euro95_E10, 2.0, DateTime.Now)
            };

            return Task.FromResult<Option<List<FuelPriceResult>>>(list);
        }

        public Task<Option<List<Station>>> GetStationListAsync()
        {
            var list = new List<Station>
            {
                new Station(DataProvider,"debug", "Debug Station", Geolocation.Veghel)
            };

            return Task.FromResult<Option<List<Station>>>(list);
        }
    }
}