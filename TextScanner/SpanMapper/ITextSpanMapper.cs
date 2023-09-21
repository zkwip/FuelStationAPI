namespace TextScanner.SpanMapper
{
    public interface ITextSpanMapper<TOut>
    {
        MappedScanResult<TOut> Map(ManagedTextSpan text);
    }
}
