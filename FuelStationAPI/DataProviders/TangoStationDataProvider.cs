﻿using FuelStationAPI.DataProviders;
using Microsoft.Extensions.Caching.Memory;

namespace FuelStationAPI.DataProviders
{
    public class TangoStationDataProvider : BaseFuelStationDataProvider
    {
        public TangoStationDataProvider(HttpClient client, ILogger<BaseFuelStationDataProvider> logger, IMemoryCache cache) : base(client, logger, cache)
        {
            _stationDetailUrlPrefix = @"https://www.tango.nl/stations/";
        }

        public async override Task<IEnumerable<FuelStationIdentifier>> ScrapeStationListAsync()
        {
            FuelStationIdentifier[] stations = { new FuelStationIdentifier("tango", "tango-eindhoven-kennedylaan", "Tango Eindhoven Kennedylaan", Geolocation.Strijp) };
            return stations.AsEnumerable();
        }

        public async override Task<FuelStationScrapeResult> ScrapeStationPricesAsync(FuelStationIdentifier station)
        {
            string url = _stationDetailUrlPrefix + station.Identifier;
            HttpResponseMessage message = await _httpClient.GetAsync(url);

            if (!message.IsSuccessStatusCode)
                return new FuelStationScrapeResult(station, new Exception(message.ReasonPhrase));

            string msg = await message.Content.ReadAsStringAsync();
            List<FuelPriceResult> list = ExtractPrices(msg);

            if (list.Count == 0)
            {
                _logger.Log(LogLevel.Information, "The station {station} did not result any fuel prices", station.Name);
                return new FuelStationScrapeResult(station, new Exception("No prices found"));
            }

            return new FuelStationScrapeResult(station, list);
        }

        protected override List<FuelPriceResult> ExtractPrices(string msg)
        {
            List<FuelPriceResult> list = new();

            ExtractPrice(msg, list, "euro95", FuelType.Euro95_E10);
            ExtractPrice(msg, list, "euro98", FuelType.Euro98_E5);
            ExtractPrice(msg, list, "diesel", FuelType.Diesel);
            return list;
        }

        private void ExtractPrice(string msg, List<FuelPriceResult> list, string handle, FuelType type)
        {
            StringScraper scraper = new(msg);
            try
            {
                if (!scraper.TestReadTo("\" id=\"" + handle + "\">")) return;

                scraper.ReadTo("\" id=\"" + handle + "\">");
                scraper.ReadTo("<div class=\"pump_price\">");
                scraper.ReadTo("<span class=\"price\">");
                list.Add(new(type, scraper.ReadDecimalTo("</span>")));
            }
            catch (ScrapeException) {}
        }

        protected override string StationProviderName => "tango";

        public override IEnumerable<FuelStationIdentifier> ExtractStations(string msg)
        {
            throw new NotImplementedException();
        }
    }
}
