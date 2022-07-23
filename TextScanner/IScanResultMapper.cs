namespace TextScanner
{
    public interface IScanResultMapper<TOut>
    {
        MappedScanResult<TOut> Map(ScanResult scrapeResult);
    }
}