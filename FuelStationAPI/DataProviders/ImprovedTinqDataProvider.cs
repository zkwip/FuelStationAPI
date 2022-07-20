using Microsoft.Extensions.Caching.Memory;
using TextScanner;

namespace FuelStationAPI.DataProviders
{
    public class ImprovedTinqStationDataProvider : BaseFuelStationDataProvider
    {
        public ImprovedTinqStationDataProvider(HttpClient client, ILogger<BaseFuelStationDataProvider> logger, IMemoryCache cache) : base(client, logger, cache)
        {
            _stationDetailUrlPrefix = "https://www.tinq.nl/tankstations/";
            _stationListUrl = "https://www.tinq.nl/tankstations";
        }

        protected override string StationProviderName => "tinq";

        public override IEnumerable<FuelStationIdentifier> ExtractStations(string msg)
        {
            ScanPattern pattern = ScanPattern.Create()
                .AddEnclosedGetter("lat", "data-lat=\"", "\"")
                .AddEnclosedGetter("lng", "data-lng=\"", "\"")
                .AddEnclosedGetter("name", "<span class=\"field-content\"><h2>", "</h2>")
                .AddEnclosedGetter("identifier", "<span class=\"field-content\"><a href=\"/tankstations/", "#default");

            Scanner scraper = new(msg);
            List<FuelStationIdentifier> res = new();

            while (true)
            {
                var result = pattern.RunOn(scraper);
                    
                if (!result.Succes) 
                    break;

                var lat = result["lat"].ToDouble();
                var lng = result["lng"].ToDouble();
                var name = result["name"].ToString();
                var identifier = result["identifier"].ToString();

                FuelStationIdentifier station = new("TINQ", identifier, "TINQ " + name, new Geolocation(lat, lng));

                res.Add(station);
            }
            return res;
        }

        protected override List<FuelPriceResult> ExtractPrices(string msg)
        {
            List<FuelPriceResult> list = new();

            ExtractPrice(msg, list, "Euro95 E10", FuelType.Euro95_E10);
            ExtractPrice(msg, list, "Superplus 98 E5", FuelType.Euro98_E5);
            ExtractPrice(msg, list, "Diesel B7", FuelType.Diesel);

            return list;
        }

        private static void ExtractPrice(string msg, List<FuelPriceResult> prices, string handle, FuelType type)
        {
            Scanner scraper = new(msg);

            ScanResult result = ScanPattern.Create()
                .AddHandle("<div class=\"node node--type-price node--view-mode-default taxonomy-term-" + handle + " ds-1col clearfix\">")
                .AddEnclosedGetter("price", "<div content=\"", "\" class=\"field field--name-field-prices-price-pump field--type-float field--label-hidden field__item\">")
                .RunOn(scraper);

            if (result.Succes)
            {
                var price = result["price"].ToDouble();
                prices.Add(new(type, price));
            }
        }
    }
}