using FuelStationAPI.Domain;

namespace FuelStationAPI.DataSources
{
    public interface IDataSource
    {
        string DataProvider { get; }
        Task<Option<List<Station>>> GetStationListAsync();
        Task<Option<List<FuelPriceResult>>> GetPricesAsync(Station station);
        //Task<Option<List<FuelStationIdentifier>>> GetStationDetailsAsync(FuelStationIdentifier station);
    }
}