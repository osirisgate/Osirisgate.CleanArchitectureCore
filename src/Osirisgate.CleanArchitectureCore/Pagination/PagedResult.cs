using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;

namespace Osirisgate.CleanArchitectureCore.Pagination;

/// <summary>
/// Represents a paginated result with metadata.
/// </summary>
/// <typeparam name="T">The type of items in the page.</typeparam>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class PagedResult<T>
{

    /// <summary>
    /// Gets the items in the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; private set; }

    /// <summary>
    /// Gets the current page number (starts at 1).
    /// </summary>
    public int PageNumber { get; private set; }

    /// <summary>
    /// Gets the page size (number of items per page).
    /// </summary>
    public int PageSize { get; private set; }

    /// <summary>
    /// Gets the total number of items.
    /// </summary>
    public int TotalCount { get; private set; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => TotalCount > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > PaginationRequest.MinPageNumber;

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Initializes a new instance of <see cref="PagedResult{T}"/>.
    /// </summary>
    /// <param name="items">The items in the current page.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="totalCount">The total number of items.</param>
    /// <exception cref="ArgumentNullException">Thrown if items is null.</exception>
    private PagedResult(IReadOnlyList<T> items, int pageNumber, int pageSize, int totalCount)
    {
        ArgumentNullException.ThrowIfNull(items, nameof(items));

        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// <summary>
    /// Creates a new paginated result.
    /// </summary>
    /// <param name="items">The items in the current page.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="totalCount">The total number of items.</param>
    /// <returns>A new <see cref="PagedResult{T}"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if items is null.</exception>
    public static PagedResult<T> Create(IReadOnlyList<T> items, int pageNumber, int pageSize, int totalCount)
    {
        return new PagedResult<T>(items, pageNumber, pageSize, totalCount);
    }

    /// <summary>
    /// Creates an empty paginated result.
    /// </summary>
    /// <param name="pageNumber">The page number (default: 1).</param>
    /// <param name="pageSize">The page size (default: 10).</param>
    /// <returns>An empty <see cref="PagedResult{T}"/> instance.</returns>
    public static PagedResult<T> Empty(int pageNumber = PaginationRequest.MinPageNumber, int pageSize = PaginationRequest.DefaultPageSize)
    {
        return new PagedResult<T>([], pageNumber, pageSize, 0);
    }

    /// <summary>
    /// Converts the paginated result to a dictionary for API response.
    /// </summary>
    /// <returns>A dictionary containing items and pagination metadata.</returns>
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            ["items"] = Items,
            ["pagination"] = new Dictionary<string, object>
            {
                ["pageNumber"] = PageNumber,
                ["pageSize"] = PageSize,
                ["totalCount"] = TotalCount,
                ["totalPages"] = TotalPages,
                ["hasPreviousPage"] = HasPreviousPage,
                ["hasNextPage"] = HasNextPage
            }
        };
    }
}


