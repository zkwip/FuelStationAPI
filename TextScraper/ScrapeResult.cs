namespace TextScraper
{
    public class ScrapeResult
    {
        private readonly Dictionary<string, SmartSubstring>? _resultData;

        public readonly Boolean Succes;
        public readonly string Message;

        public ScrapeResult(Dictionary<string, SmartSubstring> resultData)
        {
            _resultData = resultData;
            Succes = true;
            Message = "Succes";
        }

        public ScrapeResult(string msg)
        {
            Succes = false;
            Message = msg;
        }

        public SmartSubstring this[string name] => TryGet(name);

        private SmartSubstring TryGet(string name)
        {
            if (!Succes)
                throw new InvalidOperationException("Tried to read data from failed scrape result");

            return _resultData![name];
        }
    }
}