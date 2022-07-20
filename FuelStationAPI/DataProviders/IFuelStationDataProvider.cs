namespace FuelStationAPI.DataProviders
{
    public interface IFuelStationDataProvider
    {
        Task<IEnumerable<FuelStationIdentifier>> ScrapeStationListAsync();
        Task<FuelStationScrapeResult> ScrapeStationPricesAsync(FuelStationIdentifier station);
        bool StationDataSourceCheck(FuelStationIdentifier station);
    }
}
