namespace TextScanner
{
    public class PatternMapper<TOut> : ITextSpanMapper<TOut>
    {
        private readonly ScanPattern _pattern;
        private readonly IScanResultMapper<TOut> _mapper;

        public PatternMapper(ScanPattern pattern, IScanResultMapper<TOut> mapper)
        {
            _pattern = pattern;
            _mapper = mapper;
        }

        public MappedScanResult<TOut> Map(ManagedTextSpan text)
        {
            var scraper = new Scanner(text);
            var result = _pattern.RunOn(scraper);
            return _mapper.Map(result);
        }

        public ITextSpanMapper<List<TOut>> Repeat => new MultiPatternMapper<TOut>(this);

        public List<TOut> RepeatToList(ManagedTextSpan text, int limit = -1)
        {
            List<TOut> result = new();
            var scraper = new Scanner(text);

            while (true)
            {
                if (limit > 0 && result.Count > limit)
                    break;

                var itemResult = _pattern.RunOn(scraper);

                if (!itemResult.Succes)
                    break;

                result.Add(_mapper.Map(itemResult).Result);
            }

            return result;
        }
    }

    public class MultiPatternMapper<TOut> : ITextSpanMapper<List<TOut>>
    {
        private readonly PatternMapper<TOut> _itemMapper;

        public MultiPatternMapper(PatternMapper<TOut> itemMapper)
        {
            _itemMapper = itemMapper;
        }

        public MappedScanResult<List<TOut>> Map(ManagedTextSpan text) => new(_itemMapper.RepeatToList(text));
    }
}
