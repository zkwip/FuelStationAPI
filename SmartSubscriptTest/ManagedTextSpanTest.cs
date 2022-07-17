using System;
using TextScanner;
using Xunit;

namespace TextScannerTests
{
    public class ManagedTextSpanTest
    {
        private readonly string _text = "hello how are you";

        private readonly ManagedTextSpan _sut;

        public ManagedTextSpanTest()
        {
            _sut = new ManagedTextSpan(_text);
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

        [Theory]
        [InlineData("0",0)]
        [InlineData("10",10)]
        [InlineData("2",2)]
        [InlineData("123",123)]
        [InlineData("-10",-10)]
        public void ToIntShouldParseAnIntCorrectly(string text, int number)
        {
            var sub = new ManagedTextSpan(text);

            Assert.Equal(number, sub.ToInt());
        }

        [Theory]
        [InlineData("0.0",0.0)]
        [InlineData("1.3",1.3)]
        [InlineData("-3",-3)]
        [InlineData("-3.2",-3.2)]
        [InlineData("-2.<sub>96</sub>",-2.96)]
        public void ToDoubleShouldParseAnDoubleCorrectly(string text, double number)
        {
            const double precision = 0.00001;

            var sub = new ManagedTextSpan(text);

            Assert.InRange(sub.ToDouble(), number - precision, number + precision);
        }
    }
}