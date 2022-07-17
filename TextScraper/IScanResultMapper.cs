namespace TextScanner
{
    public interface IScanResultMapper<TOut>
    {
        TOut Map(ScanResult scrapeResult);
    }
}