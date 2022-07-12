using System;

namespace TextScraper
{
    public struct SmartSubstring
    {
        private readonly string _text;
        private readonly int _start;
        private readonly int _length;

        public SmartSubstring(string text) : this()
        {
            _text = text;
            _start = 0;
            _length = text.Length;
        }

        private SmartSubstring(string text, int start, int length)
        {
            _text = text;
            _start = start;
            _length = length;
        }

        public ReadOnlySpan<char> AsSpan() => _text.AsSpan(_start, _length);

        public SmartSubstring Substring(int start) => Substring(start, _length - start);

        public SmartSubstring Substring(int start, int length)
        {
            if (start < 0 || start > _length)
                throw new ArgumentOutOfRangeException(nameof(start));

            if (length < 0 || length > _length)
                throw new ArgumentOutOfRangeException(nameof(length));

            return new(_text, _start + start, length);
        }

        public override string ToString() => AsSpan().ToString();
    }
}
