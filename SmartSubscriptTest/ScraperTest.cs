using Xunit;
using TextScraper;

namespace TextScraperTests
{
    public class ScraperTest
    {
        private readonly string _text = "Hello world, this is the ScraperTest test string";
        private readonly Scraper _sut;

        public ScraperTest()
        {
            _sut  = new Scraper(_text);
        }

        [Fact]
        public void ToStringOfNewScraperShouldMatchWholeString()
        {
            Assert.Equal(_text,_sut.GetRemainingText().ToString());
        }

        [Fact]
        public void ReadToShouldReturnUpToTheHandle()
        {
            var res = _sut.ReadTo("ScraperTest");
            Assert.Equal("Hello world, this is the ", res.ToString());
        }

        [Fact]
        public void ReadToShouldMoveTrimTheRest()
        {
            _sut.ReadTo("ScraperTest");
            Assert.Equal(" test string", _sut.GetRemainingText().ToString());
        }

        [Fact]
        public void SkipToShouldMoveTrimTheRest()
        {
            _sut.SkipTo("ScraperTest");
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
