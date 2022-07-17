namespace TextScraper
{
    public interface IScrapeResultMapper<TOut>
    {
        TOut Map(ScrapeResult scrapeResult);
    }
}