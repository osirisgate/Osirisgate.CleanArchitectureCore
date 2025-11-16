using Osirisgate.CleanArchitectureCore.Pagination;
using Osirisgate.CleanArchitectureCore.Request;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;
using RequestBase = Osirisgate.CleanArchitectureCore.Request.Request;

namespace Osirisgate.CleanArchitectureCore.Tests.Pagination;

public sealed class PaginationRequestExtensionsTest
{
    [Fact]
    public void GetPaginationWithPageNumberAndPageSizeShouldReturnPagination()
    {
        var payload = new Dictionary<string, object>
        {
            ["pageNumber"] = 2,
            ["pageSize"] = 20
        };
        var request = new TestRequest(payload);

        var pagination = request.GetPagination();

        Assert.NotNull(pagination);
        Assert.Equal(2, pagination!.PageNumber);
        Assert.Equal(20, pagination.PageSize);
    }

    [Fact]
    public void GetPaginationWithoutPaginationParamsShouldReturnNull()
    {
        var payload = new Dictionary<string, object>
        {
            ["name"] = "test"
        };
        var request = new TestRequest(payload);

        var pagination = request.GetPagination();

        Assert.Null(pagination);
    }

    [Fact]
    public void GetPaginationOrDefaultWithPaginationParamsShouldReturnPagination()
    {
        var payload = new Dictionary<string, object>
        {
            ["pageNumber"] = 3,
            ["pageSize"] = 15
        };
        var request = new TestRequest(payload);

        var pagination = request.GetPaginationOrDefault();

        Assert.Equal(3, pagination.PageNumber);
        Assert.Equal(15, pagination.PageSize);
    }

    [Fact]
    public void GetPaginationOrDefaultWithoutPaginationParamsShouldReturnDefault()
    {
        var payload = new Dictionary<string, object>();
        var request = new TestRequest(payload);

        var pagination = request.GetPaginationOrDefault(2, 25);

        Assert.Equal(2, pagination.PageNumber);
        Assert.Equal(25, pagination.PageSize);
    }

    [Fact]
    public void GetPaginationWithNullRequestShouldThrowException()
    {
        IRequest request = null!;
        Assert.Throws<ArgumentNullException>(() => request.GetPagination());
    }

    private sealed class TestRequest : RequestBase
    {
        public TestRequest(IReadOnlyDictionary<string, object> payload) : base(payload)
        {
        }

        protected override IDictionary<string, object> RequestStructure => new Dictionary<string, object>();
    }
}

