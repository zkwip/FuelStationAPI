namespace FuelStationAPI.DataProviders
{
    public interface IFuelStationDataProvider
    {
        Task<IEnumerable<FuelStationData>> ScrapeStationListAsync();
        Task<FuelStationScrapeResult> ScrapeStationPricesAsync(FuelStationData station);
        bool StationDataSourceCheck(FuelStationData station);
    }
}
