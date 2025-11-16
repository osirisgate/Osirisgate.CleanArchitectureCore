using Osirisgate.CleanArchitectureCore.Pagination;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;

namespace Osirisgate.CleanArchitectureCore.Tests.Pagination;

public sealed class PagedResultTest
{
    [Fact]
    public void ConstructorWithValidValuesShouldCreatePagedResult()
    {
        var items = new List<string> { "item1", "item2", "item3" };
        var result = PagedResult<string>.Create(items, 1, 10, 25);

        Assert.Equal(3, result.Items.Count);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(3, result.TotalPages);
    }

    [Fact]
    public void TotalPagesShouldCalculateCorrectly()
    {
        var items = new List<string> { "item1", "item2" };
        var result = PagedResult<string>.Create(items, 1, 10, 25);

        Assert.Equal(3, result.TotalPages);
    }

    [Fact]
    public void TotalPagesWithZeroTotalShouldReturnZero()
    {
        var items = new List<string>();
        var result = PagedResult<string>.Create(items, 1, 10, 0);

        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public void HasPreviousPageOnFirstPageShouldReturnFalse()
    {
        var items = new List<string> { "item1" };
        var result = PagedResult<string>.Create(items, 1, 10, 25);

        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public void HasPreviousPageOnSecondPageShouldReturnTrue()
    {
        var items = new List<string> { "item1" };
        var result = PagedResult<string>.Create(items, 2, 10, 25);

        Assert.True(result.HasPreviousPage);
    }

    [Fact]
    public void HasNextPageOnLastPageShouldReturnFalse()
    {
        var items = new List<string> { "item1" };
        var result = PagedResult<string>.Create(items, 3, 10, 25);

        Assert.False(result.HasNextPage);
    }

    [Fact]
    public void HasNextPageOnFirstPageShouldReturnTrue()
    {
        var items = new List<string> { "item1" };
        var result = PagedResult<string>.Create(items, 1, 10, 25);

        Assert.True(result.HasNextPage);
    }

    [Fact]
    public void EmptyShouldCreateEmptyPagedResult()
    {
        var result = PagedResult<string>.Empty();

        Assert.Empty(result.Items);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public void EmptyWithCustomValuesShouldCreateEmptyPagedResult()
    {
        var result = PagedResult<string>.Empty(2, 20);

        Assert.Empty(result.Items);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(20, result.PageSize);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public void ToDictionaryShouldReturnCorrectStructure()
    {
        var items = new List<string> { "item1", "item2" };
        var result = PagedResult<string>.Create(items, 2, 10, 25);

        var dict = result.ToDictionary();

        Assert.True(dict.ContainsKey("items"));
        Assert.True(dict.ContainsKey("pagination"));

        var pagination = dict["pagination"] as Dictionary<string, object>;
        Assert.NotNull(pagination);
        Assert.Equal(2, pagination!["pageNumber"]);
        Assert.Equal(10, pagination["pageSize"]);
        Assert.Equal(25, pagination["totalCount"]);
        Assert.Equal(3, pagination["totalPages"]);
        Assert.True((bool)pagination["hasPreviousPage"]);
        Assert.True((bool)pagination["hasNextPage"]);
    }

    [Fact]
    public void CreateWithNullItemsShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => PagedResult<string>.Create(null!, 1, 10, 0));
    }
}
