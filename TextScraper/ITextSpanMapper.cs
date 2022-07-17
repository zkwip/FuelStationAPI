namespace TextScanner
{
    public interface ITextSpanMapper<TOut>
    {
        TOut Map(ManagedTextSpan text);
    }

    public class DoubleMapper : ITextSpanMapper<double>
    {
        private readonly string _locale;

        public DoubleMapper(string locale = "en-US")
        {
            _locale = locale;
        }

        public double Map(ManagedTextSpan text) => text.ToDouble(_locale);
    }

    public class IntMapper : ITextSpanMapper<int>
    {
        public int Map(ManagedTextSpan text) => text.ToInt();
    }

    public class StringMapper : ITextSpanMapper<string>
    {
        public string Map(ManagedTextSpan text) => text.ToString();
    }

    public class PatternMapper<TOut> : ITextSpanMapper<TOut>
    {
        private readonly ScanPattern _pattern;
        private readonly IScanResultMapper<TOut> _mapper;

        public PatternMapper(ScanPattern pattern, IScanResultMapper<TOut> mapper)
        {
            _pattern = pattern;
            _mapper = mapper;
        }

        public TOut Map(ManagedTextSpan text)
        {
            var scraper = new TextScanner(text);
            var result = _pattern.RunOn(scraper);
            return _mapper.Map(result);
        }
    }
}
