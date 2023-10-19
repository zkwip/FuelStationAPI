using FuelStationAPI.Aggregator;
using FuelStationAPI.DataSources.Implementations;

namespace FuelStationAPI.DataSources
{
    
    public static class DataSourceExtensions
    {
        public static void AddFuelStationServices(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddHttpClient("scraper", client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "GibGas");
            });

            services.AddSingleton<IDataSource, ArgosScraper>();
            services.AddSingleton<IDataSource, CarbuScraper>();
            services.AddSingleton<IDataSource, TangoScraper>();
            services.AddSingleton<IDataSource, TinqScraper>();

            services.AddSingleton<FuelPricesAggregator>();

        }
    }
}
