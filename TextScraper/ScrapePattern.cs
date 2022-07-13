namespace TextScraper
{
    public class ScrapePattern : IExtendableScrapePattern
    {
        private readonly List<string> _handles;
        private readonly Dictionary<int, string> _names;

        private ScrapePattern()
        {
            _names = new Dictionary<int, string>();
            _handles = new List<string>();
        }

        public static IExtendableScrapePattern Create()
        {
            return new ScrapePattern();
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

        public Dictionary<string, SmartSubstring> Run(Scraper scraper)
        {
            Dictionary<string, SmartSubstring> result = new();

            for (int i = 0; i < _handles.Count; i++)
                StepOverHandles(scraper, result, i);

            return result;
        }

        private void StepOverHandles(Scraper scraper, Dictionary<string, SmartSubstring> result, int i)
        {
            if (_names.ContainsKey(i))
            {
                result.Add(_names[i], scraper.ReadTo(_handles[i]));
                return;
            }

            scraper.SkipTo(_handles[i]);
        }
    }

    public interface IOpenScrapePattern
    {
        ScrapePattern AddHandle(string handle);
    }

    public interface IExtendableScrapePattern : IOpenScrapePattern
    {
        IOpenScrapePattern AddGetter(string name);
    }
}
