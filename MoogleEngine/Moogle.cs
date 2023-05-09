namespace MoogleEngine;

public static class Moogle
{
    public static TextAnalize? textAnalize;
    
    public static SearchResult Query(string query)
    {
        SearchItem[] items = textAnalize!.Query(query);
        SearchItem[] best = items.OrderByDescending(x => x.Score).Where(x => x.Score > 0).ToArray();

        return new SearchResult(best, query);
    }

}