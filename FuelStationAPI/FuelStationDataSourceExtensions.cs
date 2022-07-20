using FuelStationAPI.DataSources;
using Microsoft.Extensions.Caching.Memory;
using TextScanner;

namespace FuelStationAPI
{
    public static class FuelStationDataSourceExtensions
    {
        public static void AddFuelStationDataSources(this IServiceCollection services)
        {
            // TINQ
            services.AddSingleton(p =>
            {
                return StationListSourceFactory(p, "https://www.tinq.nl/tankstations", "tinq", new PatternMapper<FuelStationIdentifier>(
                    ScanPattern.Create()
                        .AddEnclosedGetter("lat", "data-lat=\"", "\"")
                        .AddEnclosedGetter("lng", "data-lng=\"", "\"")
                        .AddEnclosedGetter("name", "<span class=\"field-content\"><h2>", "</h2>")
                        .AddEnclosedGetter("identifier", "<span class=\"field-content\"><a href=\"/tankstations/", "#default"),
                    new FuelStationIdentifierMapper("tinq","TINQ ")
                ).Repeat);
            });

            services.AddSingleton(p =>
            {
                return StationDataSourceFactory(p, s => $"https://www.tinq.nl/tankstations/{s.Identifier}", "tinq", new PatternMapper<FuelPriceResult>(
                    ScanPattern.Create()
                        .AddEnclosedGetter("type", "<div class=\"node node--type-price node--view-mode-default taxonomy-term-", " ds-1col clearfix\">")
                        .AddEnclosedGetter("price", "<div content=\"", "\" class=\"field field--name-field-prices-price-pump field--type-float field--label-hidden field__item\">"),
                    new FuelPriceResultMapper()
                ).Repeat);
            });
        }

        public static IFuelPriceDataSource StationDataSourceFactory(IServiceProvider services, Func<FuelStationIdentifier, string> urlBuilder, string providerName, ITextSpanMapper<List<FuelPriceResult>> mapper)
        {
            return new StationDetailSource(
                services.GetRequiredService<HttpClient>(),
                services.GetRequiredService<IMemoryCache>(),
                mapper,
                urlBuilder,
                providerName
            );
        }

        public static IFuelStationListSource StationListSourceFactory(IServiceProvider services, string url, string providerName, ITextSpanMapper<List<FuelStationIdentifier>> mapper)
        {
            return new StationListSource(
                services.GetRequiredService<HttpClient>(),
                services.GetRequiredService<IMemoryCache>(),
                mapper,
                url,
                providerName
            );
        }


    }
}
