namespace TextScanner.Pattern
{
    public interface IExtendableScanPattern : IOpenScanPattern
    {
        IOpenScanPattern AddGetter(string name);
    }
}
