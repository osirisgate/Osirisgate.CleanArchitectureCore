using Osirisgate.CleanArchitectureCore.Pagination;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;
using ArgumentOutOfRangeException = Osirisgate.CleanArchitectureCore.Exception.ArgumentOutOfRangeException;

namespace Osirisgate.CleanArchitectureCore.Tests.Pagination;

public sealed class PaginationRequestTest
{
    [Fact]
    public void ConstructorWithDefaultValuesShouldCreateDefaultPagination()
    {
        var pagination = new PaginationRequest();

        Assert.Equal(PaginationRequest.MinPageNumber, pagination.PageNumber);
        Assert.Equal(PaginationRequest.DefaultPageSize, pagination.PageSize);
        Assert.Equal(0, pagination.Skip);
        Assert.Equal(PaginationRequest.DefaultPageSize, pagination.Take);
    }

    [Fact]
    public void ConstructorWithValidValuesShouldCreatePagination()
    {
        var pagination = new PaginationRequest(2, 20);

        Assert.Equal(2, pagination.PageNumber);
        Assert.Equal(20, pagination.PageSize);
        Assert.Equal(20, pagination.Skip);
        Assert.Equal(20, pagination.Take);
    }

    [Fact]
    public void ConstructorWithPageNumberLessThanMinimumShouldThrowException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PaginationRequest(0, 10));
    }

    [Fact]
    public void ConstructorWithPageSizeZeroShouldThrowException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PaginationRequest(1, 0));
    }

    [Fact]
    public void ConstructorWithLargePageSizeShouldAcceptAnyValue()
    {
        var pagination = new PaginationRequest(1, 1000);

        Assert.Equal(1, pagination.PageNumber);
        Assert.Equal(1000, pagination.PageSize);
    }

    [Fact]
    public void SkipShouldCalculateCorrectly()
    {
        var pagination = new PaginationRequest(3, 10);
        Assert.Equal(20, pagination.Skip);
    }

    [Fact]
    public void TakeShouldEqualPageSize()
    {
        var pagination = new PaginationRequest(2, 15);
        Assert.Equal(15, pagination.Take);
    }

    [Fact]
    public void FromDictionaryWithPageNumberAndPageSizeShouldCreatePagination()
    {
        var dict = new Dictionary<string, object>
        {
            ["pageNumber"] = 2,
            ["pageSize"] = 20
        };

        var pagination = PaginationRequest.FromDictionary(dict);

        Assert.Equal(2, pagination.PageNumber);
        Assert.Equal(20, pagination.PageSize);
    }

    [Fact]
    public void FromDictionaryWithPageAndLimitShouldCreatePagination()
    {
        var dict = new Dictionary<string, object>
        {
            ["page"] = 3,
            ["limit"] = 15
        };

        var pagination = PaginationRequest.FromDictionary(dict);

        Assert.Equal(3, pagination.PageNumber);
        Assert.Equal(15, pagination.PageSize);
    }

    [Fact]
    public void FromDictionaryWithStringValuesShouldParseAndCreatePagination()
    {
        var dict = new Dictionary<string, object>
        {
            ["pageNumber"] = "2",
            ["pageSize"] = "20"
        };

        var pagination = PaginationRequest.FromDictionary(dict);

        Assert.Equal(2, pagination.PageNumber);
        Assert.Equal(20, pagination.PageSize);
    }

    [Fact]
    public void FromDictionaryWithoutParametersShouldUseDefaults()
    {
        var dict = new Dictionary<string, object>();

        var pagination = PaginationRequest.FromDictionary(dict);

        Assert.Equal(PaginationRequest.MinPageNumber, pagination.PageNumber);
        Assert.Equal(PaginationRequest.DefaultPageSize, pagination.PageSize);
    }

    [Fact]
    public void FromDictionaryWithNullDictionaryShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => PaginationRequest.FromDictionary(null!));
    }

    [Fact]
    public void FromDictionaryWithPageKeyShouldUsePageValue()
    {
        var parameters = new Dictionary<string, object> { ["page"] = 3 };
        var result = PaginationRequest.FromDictionary(parameters);
        Assert.Equal(3, result.PageNumber);
    }

    [Fact]
    public void FromDictionaryWithLimitKeyShouldUseLimitValue()
    {
        var parameters = new Dictionary<string, object> { ["limit"] = 50 };
        var result = PaginationRequest.FromDictionary(parameters);
        Assert.Equal(50, result.PageSize);
    }

    [Fact]
    public void FromDictionaryWithLongValueShouldConvertToInt()
    {
        var parameters = new Dictionary<string, object> { ["pageNumber"] = 5L };
        var result = PaginationRequest.FromDictionary(parameters);
        Assert.Equal(5, result.PageNumber);
    }

    [Fact]
    public void FromDictionaryWithStringValueShouldParseToInt()
    {
        var parameters = new Dictionary<string, object> { ["pageNumber"] = "7" };
        var result = PaginationRequest.FromDictionary(parameters);
        Assert.Equal(7, result.PageNumber);
    }

    [Fact]
    public void FromDictionaryWithInvalidStringValueShouldUseDefault()
    {
        var parameters = new Dictionary<string, object> { ["pageNumber"] = "invalid" };
        var result = PaginationRequest.FromDictionary(parameters);
        Assert.Equal(PaginationRequest.MinPageNumber, result.PageNumber);
    }

    [Fact]
    public void FromDictionaryWithUnsupportedTypeShouldUseDefault()
    {
        var parameters = new Dictionary<string, object> { ["pageNumber"] = 3.14 };
        var result = PaginationRequest.FromDictionary(parameters);
        Assert.Equal(PaginationRequest.MinPageNumber, result.PageNumber);
    }
}

