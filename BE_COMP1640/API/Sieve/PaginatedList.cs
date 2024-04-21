using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace API.Sieve;

public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int CurrentPage { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }
    public string? NextPage { get; }
    public string? PreviousPage { get; }

    public PaginatedList(IReadOnlyCollection<T> items, int count, int currentPage = 1, int pageSize = 100, string? nextPage = null, string? previousPage = null)
    {
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
        NextPage = nextPage;
        PreviousPage = previousPage;
    }


    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, ISieveProcessor sieveProcessor, SieveModel sieveModel, IHttpContextAccessor httpContextAccessor)
    {
        var request = httpContextAccessor.HttpContext.Request;

        var items = sieveProcessor.Apply(sieveModel, source, applyPagination: false);

        var count = await items.CountAsync();

        items = sieveProcessor.Apply(sieveModel, items, applyFiltering: false, applySorting: false);

        var currentPage = sieveModel.Page ?? 1;
        var pageSize = sieveModel.PageSize ?? 100;
        var baseUrl = $"{request.Scheme}://{request.Host}{request.Path}";

        var queryString = GetQueryString(request.Query);

        // Remove page and pageSize from query string before regenerating
        queryString = RemovePageAndPageSizeFromQueryString(queryString);

        var nextPage = currentPage < (int)Math.Ceiling(count / (double)pageSize) ? $"{baseUrl}?{queryString}&page={currentPage + 1}&pageSize={pageSize}" : null;
        var previousPage = currentPage > 1 ? $"{baseUrl}?{queryString}&page={currentPage - 1}&pageSize={pageSize}" : null;

        return new PaginatedList<T>(await items.ToListAsync(), count, currentPage, pageSize, nextPage, previousPage);
    }

    private static string GetQueryString(IQueryCollection query)
    {
        var dict = new Dictionary<string, string>();
        foreach (var kvp in query)
        {
            dict.Add(kvp.Key, kvp.Value);
        }
        return string.Join("&", dict.Select(x => $"{x.Key}={x.Value}"));
    }


    private static string RemovePageAndPageSizeFromQueryString(string queryString)
    {
        // Divide query string into key-value pairs
        var queryParams = queryString.Split('&').ToList();
        // Remove page and pageSize parameter
        queryParams.RemoveAll(x => x.StartsWith("page=") || x.StartsWith("pageSize="));
        // Regenerate the rest parameters
        return string.Join("&", queryParams);
    }

}

public static class MappingExtensions
{
    public static Task<PaginatedList<TDestination>> ToPaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, ISieveProcessor sieveProcessor, SieveModel sieveModel, IHttpContextAccessor httpContextAccessor) where TDestination : class
        => PaginatedList<TDestination>.CreateAsync(queryable, sieveProcessor, sieveModel, httpContextAccessor);
}