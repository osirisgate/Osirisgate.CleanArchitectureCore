using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;

namespace Osirisgate.CleanArchitectureCore.Pagination;

/// <summary>
/// Extension methods for paginating collections and data sources.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public static class PaginationExtensions
{
    /// <summary>
    /// Paginates an enumerable collection based on pagination parameters.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="source">The source collection to paginate.</param>
    /// <param name="pagination">The pagination parameters.</param>
    /// <returns>A <see cref="PagedResult{T}"/> containing the paginated items and metadata.</returns>
    /// <exception cref="ArgumentNullException">Thrown if source or pagination is null.</exception>
    public static PagedResult<T> ToPagedResult<T>(this IEnumerable<T> source, PaginationRequest pagination)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(pagination, nameof(pagination));

        var items = source as IList<T> ?? [.. source];
        var totalCount = items.Count;
        var pagedItems = items
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList()
            .AsReadOnly();

        return PagedResult<T>.Create(pagedItems, pagination.PageNumber, pagination.PageSize, totalCount);
    }

    /// <summary>
    /// Paginates an enumerable collection with a total count provided separately.
    /// Useful when the total count comes from a separate query (e.g., database count).
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="source">The source collection to paginate (should already be limited to the current page).</param>
    /// <param name="pagination">The pagination parameters.</param>
    /// <param name="totalCount">The total number of items across all pages.</param>
    /// <returns>A <see cref="PagedResult{T}"/> containing the paginated items and metadata.</returns>
    /// <exception cref="ArgumentNullException">Thrown if source or pagination is null.</exception>
    public static PagedResult<T> ToPagedResult<T>(this IEnumerable<T> source, PaginationRequest pagination, int totalCount)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(pagination, nameof(pagination));

        var items = source as IReadOnlyList<T> ?? source.ToList().AsReadOnly();
        return PagedResult<T>.Create(items, pagination.PageNumber, pagination.PageSize, totalCount);
    }

    /// <summary>
    /// Paginates an enumerable collection with a total count function.
    /// The total count function is called once to get the total number of items.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="source">The source collection to paginate.</param>
    /// <param name="pagination">The pagination parameters.</param>
    /// <param name="totalCountFunc">A function that returns the total count of items.</param>
    /// <returns>A <see cref="PagedResult{T}"/> containing the paginated items and metadata.</returns>
    /// <exception cref="ArgumentNullException">Thrown if source, pagination, or totalCountFunc is null.</exception>
    public static PagedResult<T> ToPagedResult<T>(this IEnumerable<T> source, PaginationRequest pagination, Func<int> totalCountFunc)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(pagination, nameof(pagination));
        ArgumentNullException.ThrowIfNull(totalCountFunc, nameof(totalCountFunc));

        var totalCount = totalCountFunc();
        var items = source
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList()
            .AsReadOnly();

        return PagedResult<T>.Create(items, pagination.PageNumber, pagination.PageSize, totalCount);
    }

    /// <summary>
    /// Paginates an enumerable collection asynchronously with a total count function.
    /// Useful for async operations like database queries.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="source">The source collection to paginate.</param>
    /// <param name="pagination">The pagination parameters.</param>
    /// <param name="totalCountFunc">An async function that returns the total count of items.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="PagedResult{T}"/> containing the paginated items and metadata.</returns>
    /// <exception cref="ArgumentNullException">Thrown if source, pagination, or totalCountFunc is null.</exception>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IEnumerable<T> source,
        PaginationRequest pagination,
        Func<CancellationToken, Task<int>> totalCountFunc,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(pagination, nameof(pagination));
        ArgumentNullException.ThrowIfNull(totalCountFunc, nameof(totalCountFunc));

        cancellationToken.ThrowIfCancellationRequested();

        var totalCount = await totalCountFunc(cancellationToken);
        var items = source
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList()
            .AsReadOnly();

        return PagedResult<T>.Create(items, pagination.PageNumber, pagination.PageSize, totalCount);
    }

    /// <summary>
    /// Creates a paginated result from a collection of items and total count.
    /// Useful when you already have the paginated items and total count from a data source.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="items">The items for the current page.</param>
    /// <param name="pagination">The pagination parameters.</param>
    /// <param name="totalCount">The total number of items across all pages.</param>
    /// <returns>A <see cref="PagedResult{T}"/> containing the items and metadata.</returns>
    /// <exception cref="ArgumentNullException">Thrown if items or pagination is null.</exception>
    public static PagedResult<T> ToPagedResult<T>(this IReadOnlyList<T> items, PaginationRequest pagination, int totalCount)
    {
        ArgumentNullException.ThrowIfNull(items, nameof(items));
        ArgumentNullException.ThrowIfNull(pagination, nameof(pagination));

        return PagedResult<T>.Create(items, pagination.PageNumber, pagination.PageSize, totalCount);
    }

    /// <summary>
    /// Creates an empty paginated result.
    /// </summary>
    /// <typeparam name="T">The type of items.</typeparam>
    /// <param name="pagination">The pagination parameters.</param>
    /// <returns>An empty <see cref="PagedResult{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if pagination is null.</exception>
    public static PagedResult<T> EmptyPagedResult<T>(this PaginationRequest pagination)
    {
        ArgumentNullException.ThrowIfNull(pagination, nameof(pagination));

        return PagedResult<T>.Empty(pagination.PageNumber, pagination.PageSize);
    }
}
