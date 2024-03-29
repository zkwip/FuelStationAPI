﻿using FuelStationAPI.Domain;

namespace FuelStationAPI.Controllers
{
    public class SourceStatusReport
    {
        private SourceStatusReport(string dataProvider, int count)
        {
            DataProvider = dataProvider;
            StationCount = count;
        }

        public string DataProvider { get; }
        public int StationCount { get; }

        public static async Task<SourceStatusReport> CreateAsync(IAsyncGrouping<string, FuelStationIdentifier> x)
        {
            int count = await x.CountAsync();
            return new(x.Key, count);
        }
    }
}