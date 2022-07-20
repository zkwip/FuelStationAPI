namespace FuelStationAPI
{
    public class FuelCostComparison
    {
        public FuelCostComparison(IEnumerable<FuelStationScrapeResult> scrapes, Func<FuelStationScrapeResult, double> costCalculator)
        {
            List<FuelCostComparisonItem> items = new();

            foreach (FuelStationScrapeResult scrape in scrapes)
            {
                double cost = costCalculator(scrape);
                if (cost > 0)
                    items.Add(new FuelCostComparisonItem(scrape.Station, cost));
            }

            Options = items.OrderBy(a => a.Cost).ToList();
            double best = items[0].Cost;
        }

        public List<FuelCostComparisonItem> Options { get; }
    }

    public class FuelCostComparisonItem
    {
        public FuelStationIdentifier Station { get; }

        public double Cost { get; }

        public FuelCostComparisonItem(FuelStationIdentifier station, double cost)
        {
            Station = station;
            Cost = cost;
        }

    }
}
