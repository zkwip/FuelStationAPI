namespace TextScanner
{
    public class ScanPattern : IExtendableScanPattern
    {
        private readonly List<string> _handles;
        private readonly Dictionary<int, string> _names;

        private bool BreakOnMissedHandle;

        private ScanPattern(bool breakOnStepFail = true)
        {
            _names = new Dictionary<int, string>();
            _handles = new List<string>();
            BreakOnMissedHandle = breakOnStepFail;
        }

        public static IExtendableScanPattern Create()
        {
            return new ScanPattern();
        }

        public ScanPattern AddEnclosedGetter(string name, string prefix, string suffix)
        {
            _handles.Add(prefix);
            _names.Add(_handles.Count, name);
            _handles.Add(suffix);
            return this;
        }

        public ScanPattern AddHandle(string handle)
        {
            _handles.Add(handle);
            return this;
        }

        public IOpenScanPattern AddGetter(string name)
        {
            _names.Add(_handles.Count, name);
            return this;
        }

        public ScanResult RunOn(Scanner scraper)
        {
            Dictionary<string, ManagedTextSpan> result = new();

            for (int i = 0; i < _handles.Count; i++)
            {
                bool stepSuccess = StepOverHandles(scraper, result, i);

                if (!stepSuccess && BreakOnMissedHandle) 
                    return new("Could not find handle " + _handles[i]);
            }

            return new(result);
        }

        private bool StepOverHandles(Scanner scraper, Dictionary<string, ManagedTextSpan> result, int index)
        {
            string handle = _handles[index];

            if (!scraper.Contains(handle)) 
                return false;

            if (_names.ContainsKey(index))
            {
                result.Add(_names[index], scraper.ReadTo(handle));
                return true;
            }

            scraper.SkipTo(handle);
            return true;
        }
    }

    public interface IOpenScanPattern
    {
        ScanPattern AddHandle(string handle);
        ScanPattern AddEnclosedGetter(string name, string prefix, string suffix);
    }

    public interface IExtendableScanPattern : IOpenScanPattern
    {
        IOpenScanPattern AddGetter(string name);
    }
}
