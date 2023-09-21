namespace TextScanner.SpanMapper
{
    public class DoubleMapper : ITextSpanMapper<double>
    {
        private readonly string _locale;

        public DoubleMapper(string locale = "en-US")
        {
            _locale = locale;
        }

        public MappedScanResult<double> Map(ManagedTextSpan text)
        {
            try
            {
                return new(text.ToDouble(_locale));
            }
            catch (Exception ex)
            {
                return MappedScanResult<double>.Fail(ex.Message);
            }
        }
    }
}
