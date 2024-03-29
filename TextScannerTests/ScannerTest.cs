﻿using TextScanner;
using Xunit;

namespace TextScannerTests
{
    public class ScannerTest
    {
        private readonly string _text = "Hello world, this is the TextScannerTest test string";
        private readonly Scanner _sut;

        public ScannerTest()
        {
            _sut = new Scanner(_text);
        }

        [Fact]
        public void ToStringOfNewScraperShouldMatchWholeString()
        {
            Assert.Equal(_text, _sut.GetRemainingText().ToString());
        }

        [Fact]
        public void ReadToShouldReturnUpToTheHandle()
        {
            var res = _sut.ReadTo("TextScannerTest");
            Assert.Equal("Hello world, this is the ", res.ToString());
        }

        [Fact]
        public void ReadToShouldMoveTrimTheRest()
        {
            _sut.ReadTo("TextScannerTest");
            Assert.Equal(" test string", _sut.GetRemainingText().ToString());
        }

        [Fact]
        public void SkipToShouldMoveTrimTheRest()
        {
            _sut.SkipTo("TextScannerTest");
            Assert.Equal(" test string", _sut.GetRemainingText().ToString());
        }

        [Fact]
        public void ContainsShouldOnlyDetectActualRemainingSubstrings()
        {
            Assert.True(_sut.Contains("world"));
            Assert.False(_sut.Contains("woldr"));

            _sut.ReadTo("world");

            Assert.False(_sut.Contains("world"));
            Assert.True(_sut.Contains("this"));
        }
    }
}
