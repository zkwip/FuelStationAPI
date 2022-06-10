namespace FuelStationAPI.Scraper
{
    public interface IFuelSiteScraper
    {
        Task<IEnumerable<FuelStationData>> ScrapeStationListAsync();
        Task<FuelStationScrapeResult> ScrapeStationPricesAsync(FuelStationData station);
        bool StationBrandCheck(FuelStationData station);
    }
}
