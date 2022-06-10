using System.Globalization;
using System.Text.RegularExpressions;

namespace FuelStationAPI.DataProviders
{
    public class StringScraper
    {
        private string _text;
        private readonly Regex _regex;

        public StringScraper(string text)
        {
            _text = text;
            _regex = new Regex("<[^>]*>|[^0-9.,]");
        }

        public string ReadTo(string handle)
        {
            int start = _text.IndexOf(handle);
            if (start == -1)
                throw new ScrapeException("Could not find the handle in the string");

            int end = start + handle.Length;

            string res = _text[..start];
            _text = _text[end..];

            return res;
        }

        public double ReadDecimalTo(string handle)
        {
            string textToParse = ReadTo(handle);
            textToParse = _regex.Replace(textToParse, "");

            if (double.TryParse(textToParse, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-US"), out double value))
                return value;

            throw new ScrapeException("The string \"" + textToParse + "\" does not parse as a number");
        }

        public double ReadCommaDecimalTo(string handle)
        {
            string textToParse = ReadTo(handle);
            textToParse = _regex.Replace(textToParse, "");

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
