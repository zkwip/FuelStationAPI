using TextScraper;
using Xunit;

namespace TextScraperTests
{
    public class ScrapePatternTest
    {
        private readonly string _text = "Hello world, my name is ScrapePatternTest! how are you doing today?";

        private readonly Scraper _scraper;

        public ScrapePatternTest()
        {
            _scraper = new Scraper(_text);
        }

        [Fact]
        public void NameShouldBeSelected()
        {
            ScrapePattern pattern = ScrapePattern.Create()
                .AddHandle("my name is ")
                .AddGetter("name")
                .AddHandle("!");

            var result = pattern.Run(_scraper);
            var text = result["name"].ToString();

            Assert.Equal("ScrapePatternTest", text);
        }

        [Fact]
        public void MultipleShouldBeSelected()
        {
            ScrapePattern pattern = ScrapePattern.Create()
                .AddHandle("Hello ")
                .AddGetter("planet")
                .AddHandle(",")
                .AddHandle("my name is ")
                .AddGetter("name")
                .AddHandle("!");

            var result = pattern.Run(_scraper);

            Assert.Equal("world", result["planet"].ToString());
            Assert.Equal("ScrapePatternTest", result["name"].ToString());
        }
    }
}
