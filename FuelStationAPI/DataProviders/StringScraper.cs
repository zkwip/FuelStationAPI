using System.Globalization;
using System.Text.RegularExpressions;

namespace FuelStationAPI.DataProviders
{
    public class StringScraper
    {
        private readonly string _text;
        private static readonly Regex _numberRegex = new("<[^>]*>|[^0-9.,]");
        private int _start;
        private int _length;

        public StringScraper(string text)
        {
            _text = text;
            _start = 0;
            _length = text.Length;
        }

        public void Reset()
        {
            _start = 0;
            _length = _text.Length;
        }

        private ReadOnlySpan<char> TextSpan => _text.AsSpan(_start, _length);

        private void SetTextStart(int offset) 
        {
            _start += offset; 
            _length -= offset;
        }

        public bool TestReadTo(string handle) => TextSpan.IndexOf(handle) >= 0;

        public string ReadTo(string handle)
        {
            int start = TextSpan.IndexOf(handle);
            if (start == -1)
                throw new ScrapeException("Could not find the handle in the string: " + handle);

            int end = start + handle.Length;

            string res = TextSpan[..start].ToString();
            SetTextStart(end);

            return res;
        }

        public void SkipTo(string handle)
        {
            int start = TextSpan.IndexOf(handle);
            if (start == -1)
                throw new ScrapeException("Could not find the handle in the string: " + handle);

            SetTextStart(start + handle.Length);
        }

        public double ReadDecimalTo(string handle)
        {
            string textToParse = ReadTo(handle);
            textToParse = _numberRegex.Replace(textToParse, "");

            if (double.TryParse(textToParse, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-US"), out double value))
                return value;

            throw new ScrapeException("The string \"" + textToParse + "\" does not parse as a number");
        }

        public double ReadCommaDecimalTo(string handle)
        {
            string textToParse = ReadTo(handle);
            textToParse = _numberRegex.Replace(textToParse, "");

            if (double.TryParse(textToParse, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("fr-FR"), out double value))
                return value;

            throw new ScrapeException("The string \"" + textToParse + "\" does not parse as a number");
        }
    }

    public class ScrapeException : Exception
    {
        public ScrapeException(string? message) : base(message) { }
    }
}
