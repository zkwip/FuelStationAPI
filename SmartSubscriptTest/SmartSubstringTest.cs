using System;
using TextScraper;
using Xunit;

namespace TextScraperTests
{
    public class SmartSubstringTest
    {
        private readonly string _text = "hello how are you";

        private readonly SmartSubstring _sut;

        public SmartSubstringTest()
        {
            _sut = new SmartSubstring(_text);
        }


        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(5, 5)]
        [InlineData(10, 0)]
        public void SubstringShouldMatchStringSubstring(int start, int length)
        {
            Assert.Equal(_text.Substring(start, length), _sut.Substring(start, length).ToString());
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        public void SubstringWithoutLengthShouldMatchStringSubstring(int start)
        {
            Assert.Equal(_text.Substring(start), _sut.Substring(start).ToString());
        }

        [Fact]
        public void AsSpanShouldHaveSameLengthAsString()
        {
            Assert.Equal(_text.Length, _sut.AsSpan().Length);

            Assert.Equal(_text, _sut.AsSpan().ToString());
        }

        [Fact]
        public void SubstringWithInvalidStartShouldThrowException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Substring(-1));

            Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Substring(_text.Length + 1));
        }

        [Fact]
        public void SubstringWithInvalidLengthShouldThrowException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Substring(0, -1));

            Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Substring(0, _text.Length + 1));

            Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Substring(_text.Length, 1));
        }
    }
}