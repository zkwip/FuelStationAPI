namespace TextScanner.SpanMapper
{
    public class IntMapper : ITextSpanMapper<int>
    {

        public MappedScanResult<int> Map(ManagedTextSpan text)
        {
            try
            {
                return new(text.ToInt());
            }
            catch (Exception ex)
            {
                return MappedScanResult<int>.Fail(ex.Message);
            }
        }
    }
}
