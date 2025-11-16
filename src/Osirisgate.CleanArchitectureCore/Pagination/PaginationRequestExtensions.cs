using Osirisgate.CleanArchitectureCore.Request;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;

namespace Osirisgate.CleanArchitectureCore.Pagination;

/// <summary>
/// Extension methods for <see cref="IRequest"/> to support pagination.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public static class PaginationRequestExtensions
{
    /// <summary>
    /// Gets pagination parameters from the request.
    /// </summary>
    /// <param name="request">The request to extract pagination from.</param>
    /// <returns>A <see cref="PaginationRequest"/> instance, or null if pagination parameters are not present.</returns>
    public static PaginationRequest? GetPagination(this IRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var payload = request.GetPayload();
        var hasPageNumber = payload.ContainsKey("pageNumber") || payload.ContainsKey("page");
        var hasPageSize = payload.ContainsKey("pageSize") || payload.ContainsKey("limit");

        if (!hasPageNumber && !hasPageSize)
        {
            return null;
        }

        return PaginationRequest.FromDictionary(payload);
    }

    /// <summary>
    /// Gets pagination parameters from the request, or returns a default pagination request.
    /// </summary>
    /// <param name="request">The request to extract pagination from.</param>
    /// <param name="defaultPageNumber">The default page number if not specified (default: 1).</param>
    /// <param name="defaultPageSize">The default page size if not specified (default: 10).</param>
    /// <returns>A <see cref="PaginationRequest"/> instance.</returns>
    public static PaginationRequest GetPaginationOrDefault(
        this IRequest request,
        int defaultPageNumber = PaginationRequest.MinPageNumber,
        int defaultPageSize = PaginationRequest.DefaultPageSize)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        return request.GetPagination() ?? new PaginationRequest(defaultPageNumber, defaultPageSize);
    }
}
