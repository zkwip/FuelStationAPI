namespace TextScanner
{
    public class ScanResult
    {
        private readonly Dictionary<string, ManagedTextSpan>? _resultData;

        public readonly Boolean Succes;
        public readonly string Message;

        public ScanResult(Dictionary<string, ManagedTextSpan> resultData)
        {
            _resultData = resultData;
            Succes = true;
            Message = "Succes";
        }

        public ScanResult(string msg)
        {
            Succes = false;
            Message = msg;
        }

        public ManagedTextSpan this[string name] => TryGet(name);

        private ManagedTextSpan TryGet(string name)
        {
            if (!Succes)
                throw new InvalidOperationException("Tried to read data from failed scrape result");

            return _resultData![name];
        }
    }
}