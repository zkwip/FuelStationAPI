using TextScanner;
using Xunit;

namespace TextScannerTests
{
    public class ScanPatternTest
    {
        private readonly string _text = "Hello world, my name is ScanPatternTest! how are you doing today?";

        private readonly TextScanner.TextScanner _scanner;

        public ScanPatternTest()
        {
            _scanner = new TextScanner.TextScanner(_text);
        }

        [Fact]
        public void NameShouldBeSelected()
        {
            ScanPattern pattern = ScanPattern.Create()
                .AddHandle("my name is ")
                .AddGetter("name")
                .AddHandle("!");

            var result = pattern.RunOn(_scanner);
            var text = result["name"].ToString();

            Assert.Equal("ScanPatternTest", text);
        }

        [Fact]
        public void MultipleShouldBeSelected()
        {
            ScanPattern pattern = ScanPattern.Create()
                .AddHandle("Hello ")
                .AddGetter("planet")
                .AddHandle(",")
                .AddHandle("my name is ")
                .AddGetter("name")
                .AddHandle("!");

            var result = pattern.RunOn(_scanner);

            Assert.Equal("world", result["planet"].ToString());
            Assert.Equal("ScanPatternTest", result["name"].ToString());
        }
    }
}
