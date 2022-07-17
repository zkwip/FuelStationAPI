using FuelStationAPI.DataProviders;

namespace FuelStationAPI.DataSources
{
    public interface IFuelStationDataSource
    {
        Task<FuelStationScrapeResult> GetPricesAsync(FuelStationData station);
    }
}