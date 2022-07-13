using TextScraper;
using Xunit;

namespace TextScraperTest
{
    public class ScrapePatternTest
    {
        string _text = "Hello world, my name is ScrapePatternTest! how are you doing today?";

        Scraper _scraper;

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
    }
}
