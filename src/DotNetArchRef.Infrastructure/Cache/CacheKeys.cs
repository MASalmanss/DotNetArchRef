namespace DotNetArchRef.Infrastructure.Cache;

public static class CacheKeys
{
    public static string AllBooks => "books_all";
    public static string BookById(int id) => $"book_{id}";

    public static string AllAuthors => "authors_all";
    public static string AuthorById(int id) => $"author_{id}";
}
