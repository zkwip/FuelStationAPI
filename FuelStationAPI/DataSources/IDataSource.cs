using FuelStationAPI.Domain;

namespace FuelStationAPI.DataSources
{
    public interface IDataSource
    {
        string DataProvider { get; }
        Task<Option<List<FuelStationIdentifier>>> GetStationListAsync();
        Task<Option<List<FuelPriceResult>>> GetPricesAsync(FuelStationIdentifier station);
        //Task<Option<List<FuelStationIdentifier>>> GetStationDetailsAsync(FuelStationIdentifier station);
    }
}