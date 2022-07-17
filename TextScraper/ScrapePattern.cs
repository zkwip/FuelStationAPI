namespace TextScraper
{
    public class ScrapePattern : IExtendableScrapePattern
    {
        private readonly List<string> _handles;
        private readonly Dictionary<int, string> _names;

        private bool BreakOnMissedHandle;

        private ScrapePattern(bool breakOnStepFail = true)
        {
            _names = new Dictionary<int, string>();
            _handles = new List<string>();
            BreakOnMissedHandle = breakOnStepFail;
        }

        public static IExtendableScrapePattern Create()
        {
            return new ScrapePattern();
        }

        public ScrapePattern AddEnclosedGetter(string name, string prefix, string suffix)
        {
            _handles.Add(prefix);
            _names.Add(_handles.Count, name);
            _handles.Add(suffix);
            return this;
        }

        public ScrapePattern AddHandle(string handle)
        {
            _handles.Add(handle);
            return this;
        }

        public IOpenScrapePattern AddGetter(string name)
        {
            _names.Add(_handles.Count, name);
            return this;
        }

        public ScrapeResult RunOn(Scraper scraper)
        {
            Dictionary<string, SmartSubstring> result = new();

            for (int i = 0; i < _handles.Count; i++)
            {
                bool stepSuccess = StepOverHandles(scraper, result, i);

                if (!stepSuccess && BreakOnMissedHandle) 
                    return new("Could not find handle " + _handles[i]);
            }

            return new(result);
        }

        private bool StepOverHandles(Scraper scraper, Dictionary<string, SmartSubstring> result, int index)
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

    public interface IOpenScrapePattern
    {
        ScrapePattern AddHandle(string handle);
        ScrapePattern AddEnclosedGetter(string name, string prefix, string suffix);
    }

    public interface IExtendableScrapePattern : IOpenScrapePattern
    {
        IOpenScrapePattern AddGetter(string name);
    }
}
