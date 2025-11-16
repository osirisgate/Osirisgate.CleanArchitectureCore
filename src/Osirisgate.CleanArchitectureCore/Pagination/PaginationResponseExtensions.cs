using Osirisgate.CleanArchitectureCore.Response;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;

namespace Osirisgate.CleanArchitectureCore.Pagination;

/// <summary>
/// Extension methods for <see cref="IResponse"/> to support pagination.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public static class PaginationResponseExtensions
{
    /// <summary>
    /// Sets a paginated result in the response.
    /// </summary>
    /// <typeparam name="T">The type of items in the paginated result.</typeparam>
    /// <param name="response">The response to update.</param>
    /// <param name="pagedResult">The paginated result to set.</param>
    /// <returns>The response instance for method chaining.</returns>
    public static IResponse SetPagedResult<T>(this IResponse response, PagedResult<T> pagedResult)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(response));
        ArgumentNullException.ThrowIfNull(pagedResult, nameof(pagedResult));

        var data = pagedResult.ToDictionary();
        response.ReplaceData(data);
        return response;
    }

    /// <summary>
    /// Paginates a collection and sets it in the response data.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="response">The response to update.</param>
    /// <param name="source">The collection to paginate.</param>
    /// <param name="pagination">The pagination parameters.</param>
    /// <returns>The response instance for method chaining.</returns>
    public static IResponse SetPagedData<T>(this IResponse response, IEnumerable<T> source, PaginationRequest pagination)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(response));
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(pagination, nameof(pagination));

        var pagedResult = source.ToPagedResult(pagination);
        response.SetPagedResult(pagedResult);
        return response;
    }

    /// <summary>
    /// Paginates a collection with a provided total count and sets it in the response data.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="response">The response to update.</param>
    /// <param name="source">The collection to paginate (should already be limited to the current page).</param>
    /// <param name="pagination">The pagination parameters.</param>
    /// <param name="totalCount">The total number of items across all pages.</param>
    /// <returns>The response instance for method chaining.</returns>
    public static IResponse SetPagedData<T>(this IResponse response, IEnumerable<T> source, PaginationRequest pagination, int totalCount)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(response));
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(pagination, nameof(pagination));

        var pagedResult = source.ToPagedResult(pagination, totalCount);
        response.SetPagedResult(pagedResult);
        return response;
    }

    /// <summary>
    /// Paginates a collection with a total count function and sets it in the response data.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="response">The response to update.</param>
    /// <param name="source">The collection to paginate.</param>
    /// <param name="pagination">The pagination parameters.</param>
    /// <param name="totalCountFunc">A function that returns the total count of items.</param>
    /// <returns>The response instance for method chaining.</returns>
    public static IResponse SetPagedData<T>(this IResponse response, IEnumerable<T> source, PaginationRequest pagination, Func<int> totalCountFunc)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(response));
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(pagination, nameof(pagination));
        ArgumentNullException.ThrowIfNull(totalCountFunc, nameof(totalCountFunc));

        var pagedResult = source.ToPagedResult(pagination, totalCountFunc);
        response.SetPagedResult(pagedResult);
        return response;
    }

    /// <summary>
    /// Paginates a collection asynchronously with a total count function and sets it in the response data.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="response">The response to update.</param>
    /// <param name="source">The collection to paginate.</param>
    /// <param name="pagination">The pagination parameters.</param>
    /// <param name="totalCountFunc">An async function that returns the total count of items.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The response instance for method chaining.</returns>
    public static async Task<IResponse> SetPagedDataAsync<T>(this IResponse response, IEnumerable<T> source, PaginationRequest pagination, Func<CancellationToken, Task<int>> totalCountFunc, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(response));
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(pagination, nameof(pagination));
        ArgumentNullException.ThrowIfNull(totalCountFunc, nameof(totalCountFunc));

        cancellationToken.ThrowIfCancellationRequested();

        var pagedResult = await source.ToPagedResultAsync(pagination, totalCountFunc, cancellationToken);
        response.SetPagedResult(pagedResult);
        return response;
    }

    /// <summary>
    /// Gets a paginated result from the response.
    /// </summary>
    /// <typeparam name="T">The type of items expected in the paginated result.</typeparam>
    /// <param name="response">The response to extract pagination from.</param>
    /// <returns>A <see cref="PagedResult{T}"/> instance, or null if the response does not contain pagination data.</returns>
    public static PagedResult<T>? GetPagedResult<T>(this IResponse response)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(response));

        var items = response.Get<IReadOnlyList<T>>("items");
        if (items == null)
        {
            return null;
        }

        var pagination = response.Get<Dictionary<string, object>>("pagination");
        if (pagination == null)
        {
            return null;
        }

        var pageNumber = pagination.TryGetValue("pageNumber", out var pageNum) && pageNum is int pn
            ? pn
            : PaginationRequest.MinPageNumber;

        var pageSize = pagination.TryGetValue("pageSize", out var pageSizeVal) && pageSizeVal is int ps
            ? ps
            : PaginationRequest.DefaultPageSize;

        var totalCount = pagination.TryGetValue("totalCount", out var totalCountVal) && totalCountVal is int tc
            ? tc
            : 0;

        return PagedResult<T>.Create(items, pageNumber, pageSize, totalCount);
    }
}
