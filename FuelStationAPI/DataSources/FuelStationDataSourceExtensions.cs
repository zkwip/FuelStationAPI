using FuelStationAPI.Aggregator;
using FuelStationAPI.DataSources.Implementations;

namespace FuelStationAPI.DataSources
{
    
    public static class FuelStationDataSourceExtensions
    {
        public static void AddFuelStationServices(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddHttpClient("scraper", client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "GibGas");
            });

            services.AddSingleton<IDataSource, ArgosScrapeService>();
            services.AddSingleton<IDataSource, CarbuScrapeService>();
            services.AddSingleton<IDataSource, TangoScrapeService>();
            services.AddSingleton<IDataSource, TinqScrapeService>();

            services.AddSingleton<FuelPricesAggregator>();

        }
    }
}
