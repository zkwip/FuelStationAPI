using TextScraper;
using Xunit;

namespace TextScraperTests
{
    public class SmartSubstringTest
    {
        private readonly string _text = "hello how are you";

        private SmartSubstring _sut;

        public SmartSubstringTest()
        {
            _sut = new SmartSubstring(_text);
        }


        [Theory]
        [InlineData(0,0)]
        [InlineData(0,1)]
        [InlineData(5,5)]
        [InlineData(10,0)]
        public void SubstringShouldMatchStringSubstring(int start, int length)
        {
            Assert.Equal(_text.Substring(start,length), _sut.Substring(start, length).ToString());
        }
    }
}