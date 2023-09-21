using FuelStationAPI.Domain;
using FuelStationAPI.Mappers;
using Microsoft.Extensions.Caching.Memory;
using TextScanner;

namespace FuelStationAPI.DataSources
{
    public static class FuelStationDataSourceExtensions
    {
        public static void AddFuelStationDataSources(this IServiceCollection services)
        {
            services.AddSingleton<FuelServiceAggregator>();
            services.AddArgosServices();
            services.AddCarbuServices();
            services.AddTangoServices();
            //services.AddTankBilligServices();
            services.AddTinqServices();
        }

        private static void AddArgosServices(this IServiceCollection services)
        {
            services.AddDataSourceServices(
                "argos",
                "",
                "https://www.argos.nl/tankstations",
                s => $"https://www.argos.nl/tankstation/{s.Identifier}",
                ScanPattern.Create()
                    .AddHandle("<div class=\"marker\" data-id=\"")
                    .AddEnclosedGetter("lat", "data-lat=\"", "\"")
                    .AddEnclosedGetter("lng", "data-lng=\"", "\"")
                    .AddEnclosedGetter("name", "<strong>", "</strong>")
                    .AddEnclosedGetter("identifier", "<a href=\"https://www.argos.nl/tankstation/", "\">Bekijk</a>"),
                ScanPattern.Create()
                    .AddHandle("<div class=\"col col4 price-item\">")
                    .AddEnclosedGetter("type", "<label class=\"name\">", "</label>")
                    .AddEnclosedGetter("price", "<span class=\"price\"> ", "</sup> </span>"),
                false);
        }

        private static void AddCarbuServices(this IServiceCollection services)
        {
            services.AddDataSourceServices(
                "carbu",
                "",
                "https://carbu.com/belgie//liste-stations-service/E10/Bilzen/3740/BE_li_701",
                s => $"https://carbu.com/belgie/index.php/station/{s.Identifier}",
                ScanPattern.Create()
                    .AddHandle("<div class=\"station-content col-xs-12\">")
                    .AddEnclosedGetter("lat", "data-lat=\"", "\"")
                    .AddEnclosedGetter("lng", "data-lng=\"", "\"")
                    .AddEnclosedGetter("name", "data-name=\"", "\"")
                    .AddEnclosedGetter("identifier", "data-link=\"https://carbu.com/belgie/index.php/station/", "\""),
                ScanPattern.Create()
                    .AddHandle("<div class=\"col-xs-12 col-sm-6\">")
                    .AddHandle("<div class=\"panel-heading\" style=\"text-align:center\">")
                    .AddEnclosedGetter("type", "<h2 class=\"title\">", "</h2>")
                    .AddEnclosedGetter("price", "<h1 class=\"price\">", " &euro;"),
                true);
        }

        private static void AddTangoServices(this IServiceCollection services)
        {
            services.AddDataSourceServices(
                "tango",
                "",
                "https://www.tango.nl/get/stations.json",
                s => $"https://www.tango.nl/stations/{s.Identifier}",
                ScanPattern.Create()
                    .AddHandle("StationId")
                    .AddEnclosedGetter("identifier", "NodeURL\":\"\\/stations\\/", "\",")
                    .AddEnclosedGetter("name", "Name\":\"", "\",")
                    .AddEnclosedGetter("lat", "XCoordinate\":\"", "\",")
                    .AddEnclosedGetter("lng", "YCoordinate\":\"", "\","),
                ScanPattern.Create()
                    .AddHandle("<div class=\"pricing ")
                    .AddEnclosedGetter("type", "\" id=\"", "\">")
                    .AddHandle("<div class=\"pump_price\">")
                    .AddEnclosedGetter("price", "<span class=\"price\">", "</span> EUR"),
                false);
        }

        // pricing does not yet work with this version of TextScanner, waiting for TextMagic instead.

        /* private static void AddTankBilligServices(this IServiceCollection services)
        {
            services.AddDataSourceServices(
                "tankbillig",
                "",
                "https://tankbillig.info/index.php?lat=51.7833&long=6.0167",
                s => $"https://ich-tanke.de/tankstelle/{s.Identifier}",
                ScanPattern.Create()
                    .AddEnclosedGetter("identifier", "\"stationID\":\"", "\",")
                    .AddEnclosedGetter("name", "\"gasStationName\":\"", "\", \"brand\":\"")
                    .AddEnclosedGetter("lng", "\", \"longitude\":\"", "\",")
                    .AddEnclosedGetter("lat", "\", \"latitude\":\"", "\","),
                ScanPattern.Create()
                    .AddHandle("<div class=\"pricing ")
                    .AddEnclosedGetter("type", "\" id=\"", "\">")
                    .AddHandle("<div class=\"pump_price\">")
                    .AddEnclosedGetter("price", "<span class=\"price\">", "</span> EUR"),
                false);
        } */

        private static void AddTinqServices(this IServiceCollection services)
        {
            services.AddDataSourceServices(
                "tinq",
                "TINQ ",
                "https://www.tinq.nl/tankstations",
                s => $"https://www.tinq.nl/tankstations/{s.Identifier}",
                ScanPattern.Create()
                    .AddEnclosedGetter("lat", "data-lat=\"", "\"")
                    .AddEnclosedGetter("lng", "data-lng=\"", "\"")
                    .AddEnclosedGetter("name", "<span class=\"field-content\"><h2>", "</h2>")
                    .AddEnclosedGetter("identifier", "<span class=\"field-content\"><a href=\"/tankstations/", "#default"),
                ScanPattern.Create()
                    .AddEnclosedGetter("type", "<div class=\"node node--type-price node--view-mode-default taxonomy-term-", " ds-1col clearfix\">")
                    .AddEnclosedGetter("price", "<div content=\"", "\" class=\"field field--name-field-prices-price-pump field--type-float field--label-hidden field__item\">"),
                false);
        }

        private static void AddDataSourceServices(this IServiceCollection services, string providerName, string namePrefix, string listUrl, Func<FuelStationIdentifier, string> priceUrlBuilder, ScanPattern listPattern, ScanPattern pricePattern, bool useDecimalCommaPrice)
        {
            services.AddHttpClient(providerName, client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "GibGas");
            });

            services.AddSingleton<IFuelStationListSource>(p =>
            {
                return new FuelStationListSource(
                    p.GetRequiredService<IHttpClientFactory>(),
                    p.GetRequiredService<IMemoryCache>(),
                    new PatternMapper<FuelStationIdentifier>(
                        listPattern,
                        new FuelStationIdentifierMapper(providerName, namePrefix)
                        ).Repeat,
                    listUrl,
                    providerName
                );
            });

            services.AddSingleton<IFuelPriceDataSource>(p =>
            {
                return new FuelPriceDataSource(
                    p.GetRequiredService<IHttpClientFactory>(),
                    p.GetRequiredService<IMemoryCache>(),
                    new PatternMapper<FuelPriceResult>(
                        pricePattern,
                        new FuelPriceResultMapper(useDecimalCommaPrice)
                    ).Repeat,
                    priceUrlBuilder,
                    providerName);
            });
        }
    }
}
