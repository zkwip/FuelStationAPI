using System.Globalization;
using System.Text.RegularExpressions;

namespace TextScraper
{
    public class Scraper
    {
        private static readonly Regex _numberRegex = new("<[^>]*>|[^0-9.,]");
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

        public bool TestReadTo(string handle) => TextSpan.IndexOf(handle) >= 0;

        public SmartSubstring ReadTo(string handle)
        {
            int start = FindStartOfSubstring(handle);
            int end = start + handle.Length;

            SmartSubstring res = _text.Substring(0, start);
            _text = _text.Substring(end);

            return res;
        }

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

        public double ReadDecimalTo(string handle)
        {
            string textToParse = ReadTo(handle).ToString();
            textToParse = _numberRegex.Replace(textToParse, "");

            if (double.TryParse(textToParse, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-US"), out double value))
                return value;

            throw new ScrapeException("The string \"" + textToParse + "\" does not parse as a number");
        }

        public double ReadCommaDecimalTo(string handle)
        {
            string textToParse = ReadTo(handle).ToString();
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
