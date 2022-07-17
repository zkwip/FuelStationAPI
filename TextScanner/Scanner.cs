namespace TextScanner
{
    public class Scanner
    {
        private ManagedTextSpan _text;

        public Scanner(string text)
        {
            _text = new ManagedTextSpan(text);
        }

        public Scanner(ManagedTextSpan text)
        {
            _text = text;
        }

        private ReadOnlySpan<char> TextSpan => _text.AsSpan();

        public bool Contains(string handle) => TextSpan.IndexOf(handle) >= 0;

        public ManagedTextSpan ReadTo(string handle)
        {
            int start = FindStartOfSubstring(handle);
            int end = start + handle.Length;

            ManagedTextSpan res = _text.Substring(0, start);
            _text = _text.Substring(end);

            return res;
        }

        public ManagedTextSpan GetRemainingText() => _text;

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
