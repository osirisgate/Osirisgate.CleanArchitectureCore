using Osirisgate.CleanArchitectureCore.Pagination;
using Osirisgate.CleanArchitectureCore.Response;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;

namespace Osirisgate.CleanArchitectureCore.Tests.Pagination;

public sealed class PaginationResponseExtensionsTest
{
    [Fact]
    public void SetPagedResultShouldSetCorrectData()
    {
        var items = new List<string> { "item1", "item2" };
        var pagedResult = PagedResult<string>.Create(items, 2, 10, 25);
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "success", new Dictionary<string, object>());

        response.SetPagedResult(pagedResult);

        var retrieved = response.GetPagedResult<string>();
        Assert.NotNull(retrieved);
        Assert.Equal(2, retrieved!.Items.Count);
        Assert.Equal(2, retrieved.PageNumber);
        Assert.Equal(10, retrieved.PageSize);
        Assert.Equal(25, retrieved.TotalCount);
    }

    [Fact]
    public void GetPagedResultWithValidDataShouldReturnPagedResult()
    {
        var data = new Dictionary<string, object>
        {
            ["items"] = new List<string> { "item1", "item2" },
            ["pagination"] = new Dictionary<string, object>
            {
                ["pageNumber"] = 2,
                ["pageSize"] = 10,
                ["totalCount"] = 25,
                ["totalPages"] = 3,
                ["hasPreviousPage"] = true,
                ["hasNextPage"] = true
            }
        };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "success", data);

        var pagedResult = response.GetPagedResult<string>();

        Assert.NotNull(pagedResult);
        Assert.Equal(2, pagedResult!.Items.Count);
        Assert.Equal(2, pagedResult.PageNumber);
        Assert.Equal(10, pagedResult.PageSize);
        Assert.Equal(25, pagedResult.TotalCount);
    }

    [Fact]
    public void GetPagedResultWithoutItemsShouldReturnNull()
    {
        var data = new Dictionary<string, object>
        {
            ["pagination"] = new Dictionary<string, object>
            {
                ["pageNumber"] = 2,
                ["pageSize"] = 10,
                ["totalCount"] = 25
            }
        };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "success", data);

        var pagedResult = response.GetPagedResult<string>();

        Assert.Null(pagedResult);
    }

    [Fact]
    public void GetPagedResultWithoutPaginationShouldReturnNull()
    {
        var data = new Dictionary<string, object>
        {
            ["items"] = new List<string> { "item1" }
        };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "success", data);

        var pagedResult = response.GetPagedResult<string>();

        Assert.Null(pagedResult);
    }

    [Fact]
    public void SetPagedResultWithNullResponseShouldThrowException()
    {
        var items = new List<string> { "item1" };
        var pagedResult = PagedResult<string>.Create(items, 1, 10, 1);

        IResponse response = null!;
        Assert.Throws<ArgumentNullException>(() => response.SetPagedResult(pagedResult));
    }

    [Fact]
    public void GetPagedResultWithNullResponseShouldThrowException()
    {
        IResponse response = null!;
        Assert.Throws<ArgumentNullException>(() => response.GetPagedResult<string>());
    }
}

