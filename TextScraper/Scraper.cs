namespace TextScraper
{
    public class Scraper
    {
        private SmartSubstring _text;

        public Scraper(string text)
        {
            _text = new SmartSubstring(text);
        }

        public Scraper(SmartSubstring text)
        {
            _text = text;
        }

        private ReadOnlySpan<char> TextSpan => _text.AsSpan();

        public bool Contains(string handle) => TextSpan.IndexOf(handle) >= 0;

        public SmartSubstring ReadTo(string handle)
        {
            int start = FindStartOfSubstring(handle);
            int end = start + handle.Length;

            SmartSubstring res = _text.Substring(0, start);
            _text = _text.Substring(end);

            return res;
        }

        public SmartSubstring GetRemainingText() => _text;

        public void SkipTo(string handle)
        {
            int start = FindStartOfSubstring(handle);
            _text = _text.Substring(start + handle.Length);
        }

        private int FindStartOfSubstring(string handle)
        {
            int start = TextSpan.IndexOf(handle);

            if (start == -1)
                throw new ScrapeException("Could not find the handle in the string: " + handle);

            return start;
        }
    }

    public class ScrapeException : Exception
    {
        public ScrapeException(string? message) : base(message) { }
    }
}
