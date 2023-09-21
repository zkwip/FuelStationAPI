namespace TextScanner.Pattern
{
    public interface IOpenScanPattern
    {
        ScanPattern AddHandle(string handle);
        ScanPattern AddEnclosedGetter(string name, string prefix, string suffix);
    }
}
