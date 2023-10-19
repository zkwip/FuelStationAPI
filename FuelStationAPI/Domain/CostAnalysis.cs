namespace FuelStationAPI.Domain
{
    public class CostAnalysis<T>
    {
        public double Cost { get; set; }
        public T Value { get; set; }

        public CostAnalysis(T value, double cost = 0.0)
        {
            Cost = cost;
            Value = value;
        }

        public CostAnalysis<T> AddCost(double cost) 
        {
            Cost += cost; 
            return this; 
        }
    }
}
