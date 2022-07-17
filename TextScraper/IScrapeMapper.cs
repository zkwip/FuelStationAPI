using System;
using System.Collections.Generic;

namespace TextScraper
{
    public interface IScrapeMapper<TOut>
    {
        TOut Map(SmartSubstring text);
    }

    public class DoubleMapper : IScrapeMapper<double>
    {
        private string _locale;

        public DoubleMapper(string locale = "en-US")
        {
            _locale = locale;
        }

        public double Map(SmartSubstring text) => text.ToDouble(_locale);
    }

    public class IntMapper : IScrapeMapper<int>
    {
        public int Map(SmartSubstring text) => text.ToInt();
    }

    public class StringMapper : IScrapeMapper<string>
    {
        public string Map(SmartSubstring text) => text.ToString();
    }

    public class PatternMapper<TOut> : IScrapeMapper<TOut>
    {
        private ScrapePattern _pattern;
        private IScrapeResultMapper<TOut> _mapper;

        public PatternMapper(ScrapePattern pattern, IScrapeResultMapper<TOut> mapper)
        {
            _pattern = pattern;
            _mapper = mapper;
        }

        public TOut Map(SmartSubstring text)
        {
            Scraper scraper = new Scraper(text);
            var result = _pattern.RunOn(scraper);
            return _mapper.Map(result);
        }
    }
}
