namespace TextScanner
{
    public interface ITextSpanMapper<TOut>
    {
        MappedScanResult<TOut> Map(ManagedTextSpan text);
    }

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
