using System.Globalization;

namespace FuelStationAPI.Scraper
{
    public class StringScraper
    {
        private string _text;

        public StringScraper(string text)
        {
            _text = text;
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
            string last = ReadTo(handle);
            if (double.TryParse(last, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-US"), out double value))
                return value;

            throw new ScrapeException("The string \"" + last + "\" does not parse as a number");

        }
    }

    public class ScrapeException : Exception
    {
        public ScrapeException(string? message) : base(message) { }
    }
}
