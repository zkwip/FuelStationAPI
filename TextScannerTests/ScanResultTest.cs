using System;
using System.Collections.Generic;
using TextScanner;
using Xunit;

namespace TextScannerTests
{
    public class ScanResultTest
    {
        private const string _message = "hallo";
        private readonly Dictionary<string, ManagedTextSpan> _data;

        public ScanResultTest()
        {
            _data = new Dictionary<string, ManagedTextSpan>
            {
                { _message, new ManagedTextSpan(_message) }
            };
        }

        [Fact]
        public void FailedResultShouldShowSuccesFalse()
        {
            var res = new ScanResult(_message);

            Assert.False(res.Succes);
        }

        [Fact]
        public void FailedResultShouldContainMessage()
        {
            var res = new ScanResult(_message);

            Assert.Equal(_message, res.Message);
        }

        [Fact]
        public void FailedResultShouldThrowOnGet()
        {
            var res = new ScanResult(_message);

            Assert.Throws<InvalidOperationException>(() => res["item"]);
        }

        [Fact]
        public void SuccesfulResultShouldShowSuccesTrue()
        {
            var res = new ScanResult(_data);

            Assert.True(res.Succes);
        }

        [Fact]
        public void SuccesfulResultShouldContainData()
        {
            var res = new ScanResult(_data);

            foreach (var item in _data)
                Assert.Equal(_data[item.Key], res[item.Key]);
        }
    }
}
