namespace TextScanner.SpanMapper
{
    public class StringMapper : ITextSpanMapper<string>
    {
        public MappedScanResult<string> Map(ManagedTextSpan text)
        {
            try
            {
                return new(text.ToString());
            }
            catch (Exception ex)
            {
                return MappedScanResult<string>.Fail(ex.Message);
            }
        }
    }
}
