using System.Globalization;
using System.Text.RegularExpressions;

namespace TextScanner
{
    public struct ManagedTextSpan
    {
        private static readonly Regex _numberRegex = new("<[^>]*>|[^0-9.,-]");
        private readonly string _text;
        private readonly int _start;
        private readonly int _length;

        public ManagedTextSpan(string text)
        {
            _text = text;
            _start = 0;
            _length = text.Length;
        }

        public MappedScanResult<T> MapWith<T>(ITextSpanMapper<T> mapper) => mapper.Map(this);

        private ManagedTextSpan(string text, int start, int length)
        {
            _text = text;
            _start = start;
            _length = length;
        }

        public ReadOnlySpan<char> AsSpan() => _text.AsSpan(_start, _length);

        public ManagedTextSpan Substring(int start) => Substring(start, _length - start);

        public ManagedTextSpan Substring(int start, int length)
        {
            if (start < 0 || start > _length)
                throw new ArgumentOutOfRangeException(nameof(start));

            if (length < 0 || length + start > _length)
                throw new ArgumentOutOfRangeException(nameof(length));

            return new(_text, _start + start, length);
        }

        public override string ToString() => AsSpan().ToString();

        public int ToInt()
        {
            string str = _numberRegex.Replace(ToString(), "");
            return int.Parse(str);
        }

        public double ToDouble(string locale = "en-US")
        {
            string str = _numberRegex.Replace(ToString(), "");
            return double.Parse(str, CultureInfo.CreateSpecificCulture(locale));
        }
    }
}
