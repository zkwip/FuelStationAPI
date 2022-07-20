﻿using Microsoft.Extensions.Caching.Memory;

namespace FuelStationAPI.DataProviders
{
    public class ArgosStationDataProvider : BaseFuelStationDataProvider
    {
        protected override string StationProviderName => "argos";

        public ArgosStationDataProvider(HttpClient client, ILogger<BaseFuelStationDataProvider> logger, IMemoryCache cache) : base(client, logger, cache)
        {
            _stationDetailUrlPrefix = "https://www.argos.nl/tankstation/";
            _stationListUrl = "https://www.argos.nl/tankstations/";
        }
        public override IEnumerable<FuelStationIdentifier> ExtractStations(string msg)
        {
            List<FuelStationIdentifier> list = new();
            StringScraper scraper = new(msg);

            scraper.ReadTo("<div id=\"tankstation-map\">");

            while (true)
            {
                try
                {
                    if (!scraper.TestReadTo("<div class=\"marker\" data-id=\"")) break;
                    scraper.ReadTo("<div class=\"marker\" data-id=\"");

                    scraper.ReadTo("data-lat=\"");
                    double lat = scraper.ReadDecimalTo("\"");

                    scraper.ReadTo("data-lng=\"");
                    double lng = scraper.ReadDecimalTo("\"");

                    scraper.ReadTo("<strong>");
                    string name = scraper.ReadTo("</strong>");

                    scraper.ReadTo("<a href=\"https://www.argos.nl/tankstation/");
                    string ident = scraper.ReadTo("\">Bekijk</a>");

                    list.Add(new FuelStationIdentifier("Argos", ident, name, new(lat, lng)));
                }
                catch (ScrapeException)
                {
                    break;
                }
            }
            return list;
        }

        protected override List<FuelPriceResult> ExtractPrices(string msg)
        {
            List<FuelPriceResult> list = new();

            ExtractPrice(msg, list, "Euro 95", FuelType.Euro95_E10);
            ExtractPrice(msg, list, "Superplus 98", FuelType.Euro98_E5);
            ExtractPrice(msg, list, "Diesel", FuelType.Diesel);

            return list;
        }

        private static void ExtractPrice(string msg, List<FuelPriceResult> prices, string handle, FuelType type)
        {
            try
            {
                StringScraper scraper = new(msg);

                scraper.ReadTo("<label class=\"" + handle + "\">Euro 95</label>");
                scraper.ReadTo("<span class=\"price\"> ");
                double price = scraper.ReadDecimalTo("</sup> </span>");

                prices.Add(new(type, price));
            }
            catch (ScrapeException) { }
        }
    }
}
