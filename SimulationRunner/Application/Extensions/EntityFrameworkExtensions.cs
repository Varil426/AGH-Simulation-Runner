namespace Application.Extensions;

internal static class EntityFrameworkExtensions
{
    public static Task<List<TSource>> ToListAsyncSafe<TSource>(this IQueryable<TSource> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (source is not IAsyncEnumerable<TSource>)
            return Task.FromResult(source.ToList());
        return source.ToListAsync();
    }
}