using FuelStationAPI.Domain;

namespace FuelStationAPI.DataSources
{
    public interface IFuelStationListSource : IDataSource
    {
        Task<Option<List<FuelStationIdentifier>>> GetStationListAsync();
    }

    public interface IFuelStationDetailSource : IDataSource
    {
        Task<Option<List<FuelStationIdentifier>>> GetStationDetailsAsync(FuelStationIdentifier station);
    }

    public interface IFuelPriceDataSource : IDataSource
    {
        Task<Option<List<FuelPriceResult>>> GetPricesAsync(FuelStationIdentifier station);
    }

    public interface IDataSource
    {
        string DataProvider { get; }
    }
}