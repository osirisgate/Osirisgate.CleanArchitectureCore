using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;
using ArgumentOutOfRangeException = Osirisgate.CleanArchitectureCore.Exception.ArgumentOutOfRangeException;

namespace Osirisgate.CleanArchitectureCore.Pagination;

/// <summary>
/// Represents pagination parameters for requests.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class PaginationRequest
{
    /// <summary>
    /// Default page size.
    /// </summary>
    public const int DefaultPageSize = 10;

    /// <summary>
    /// Minimum page number (starts at 1).
    /// </summary>
    public const int MinPageNumber = 1;

    /// <summary>
    /// Gets the page number (starts at 1).
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the page size (number of items per page).
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="PaginationRequest"/>.
    /// </summary>
    /// <param name="pageNumber">The page number (default: 1).</param>
    /// <param name="pageSize">The page size (default: 10).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if pageNumber is less than 1 or pageSize is less than 1.</exception>
    public PaginationRequest(int pageNumber = MinPageNumber, int pageSize = DefaultPageSize)
    {
        if (pageNumber < MinPageNumber)
        {
            throw new ArgumentOutOfRangeException(new Dictionary<string, object>
            {
                ["message"] = "pagination.invalid.pageNumber",
                ["details"] = new Dictionary<string, object>
                {
                    ["parameter"] = nameof(pageNumber),
                    ["value"] = pageNumber,
                    ["minimum"] = MinPageNumber,
                    ["maximum"] = int.MaxValue,
                    ["reason"] = "Page number must be greater than or equal to 1."
                }
            });
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(new Dictionary<string, object>
            {
                ["message"] = "pagination.invalid.pageSize",
                ["details"] = new Dictionary<string, object>
                {
                    ["parameter"] = nameof(pageSize),
                    ["value"] = pageSize,
                    ["minimum"] = 1,
                    ["maximum"] = int.MaxValue,
                    ["reason"] = "Page size must be greater than 0."
                }
            });
        }

        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>
    /// Gets the number of items to skip (calculated from page number and page size).
    /// </summary>
    public int Skip => (PageNumber - MinPageNumber) * PageSize;

    /// <summary>
    /// Gets the number of items to take (same as PageSize).
    /// </summary>
    public int Take => PageSize;

    /// <summary>
    /// Creates a <see cref="PaginationRequest"/> from a dictionary of parameters.
    /// Supports multiple key names: "pageNumber"/"page", "pageSize"/"limit".
    /// </summary>
    /// <param name="parameters">The dictionary containing pagination parameters.</param>
    /// <returns>A new <see cref="PaginationRequest"/> instance.</returns>
    public static PaginationRequest FromDictionary(IReadOnlyDictionary<string, object> parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters, nameof(parameters));

        var pageNumber = MinPageNumber;
        if (parameters.TryGetValue("pageNumber", out var pageNumValue))
        {
            pageNumber = ConvertToInt(pageNumValue) ?? MinPageNumber;
        }
        else if (parameters.TryGetValue("page", out var pageValue))
        {
            pageNumber = ConvertToInt(pageValue) ?? MinPageNumber;
        }

        var pageSize = DefaultPageSize;
        if (parameters.TryGetValue("pageSize", out var pageSizeValue))
        {
            pageSize = ConvertToInt(pageSizeValue) ?? DefaultPageSize;
        }
        else if (parameters.TryGetValue("limit", out var limitValue))
        {
            pageSize = ConvertToInt(limitValue) ?? DefaultPageSize;
        }

        return new PaginationRequest(pageNumber, pageSize);
    }

    private static int? ConvertToInt(object value)
    {
        return value switch
        {
            int intValue => intValue,
            long longValue => (int)longValue,
            string stringValue when int.TryParse(stringValue, out var parsed) => parsed,
            _ => null
        };
    }
}

