using TextScanner;

namespace FuelStationAPI.DataSources
{
    public interface IFuelStationListSource : IDataSource
    {
        Task<MappedScanResult<List<FuelStationIdentifier>>> GetStationListAsync();
    }

    public interface IFuelStationDetailSource : IDataSource
    {
        Task<MappedScanResult<List<FuelStationIdentifier>>> GetStationDetailsAsync(FuelStationIdentifier station); // needs a better output type
    }

    public interface IFuelPriceDataSource : IDataSource
    {

        Task<MappedScanResult<List<FuelPriceResult>>> GetPricesAsync(FuelStationIdentifier station);
    }

    public interface IDataSource
    {
        string DataProvider { get; }
    }
}