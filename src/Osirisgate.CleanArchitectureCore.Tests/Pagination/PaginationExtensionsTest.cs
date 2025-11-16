using Osirisgate.CleanArchitectureCore.Pagination;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;

namespace Osirisgate.CleanArchitectureCore.Tests.Pagination;

public sealed class PaginationExtensionsTest
{
    private sealed class User
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    private sealed class Product
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }

    private static List<User> GenerateUsers(int count)
    {
        return [.. Enumerable.Range(1, count)
            .Select(i => new User
            {
                Id = $"user-{i}",
                Name = $"User {i}",
                Email = $"user{i}@example.com",
                Age = 20 + (i % 50)
            })];
    }

    private static List<Product> GenerateProducts(int count)
    {
        var products = new[] { "Laptop", "Phone", "Tablet", "Monitor", "Keyboard", "Mouse", "Headphones", "Speaker" };
        return [.. Enumerable.Range(1, count)
            .Select(i => new Product
            {
                Id = $"prod-{i}",
                Name = $"{products[i % products.Length]} {i}",
                Price = 99.99m + (i * 10),
                Stock = 100 - (i % 50)
            })];
    }

    [Fact]
    public void ToPagedResultWithEnumerableShouldPaginateCorrectly()
    {
        var allUsers = GenerateUsers(47);
        var pagination = new PaginationRequest(2, 10);

        var result = allUsers.ToPagedResult(pagination);

        Assert.Equal(10, result.Items.Count);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(47, result.TotalCount);
        Assert.Equal(5, result.TotalPages);
        Assert.Equal("user-11", result.Items[0].Id);
        Assert.Equal("user-20", result.Items[9].Id);
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public void ToPagedResultWithTotalCountShouldUseProvidedCount()
    {
        var allUsers = GenerateUsers(64);
        var pagination = new PaginationRequest(2, 15);
        var totalCount = allUsers.Count;
        var pageUsers = allUsers.Skip(pagination.Skip).Take(pagination.Take).ToList();

        var result = pageUsers.ToPagedResult(pagination, totalCount);

        Assert.Equal(15, result.Items.Count);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(15, result.PageSize);
        Assert.Equal(64, result.TotalCount);
        Assert.Equal(5, result.TotalPages);
        Assert.Equal("user-16", result.Items[0].Id);
        Assert.Equal("user-30", result.Items[14].Id);
    }

    [Fact]
    public void ToPagedResultWithTotalCountFuncShouldCallFunction()
    {
        var allUsers = GenerateUsers(73);
        var pagination = new PaginationRequest(3, 15);
        var callCount = 0;
        int totalCountFunc() { callCount++; return allUsers.Count; }

        var result = allUsers.ToPagedResult(pagination, totalCountFunc);

        Assert.Equal(1, callCount);
        Assert.Equal(15, result.Items.Count);
        Assert.Equal(73, result.TotalCount);
        Assert.Equal(5, result.TotalPages);
        Assert.Equal("user-31", result.Items[0].Id);
    }

    [Fact]
    public async Task TestToPagedResultAsyncShouldPaginateAsync()
    {
        var allUsers = GenerateUsers(87);
        var pagination = new PaginationRequest(3, 20);
        var callCount = 0;
        var totalCountFunc = new Func<CancellationToken, Task<int>>(ct =>
        {
            callCount++;
            return Task.FromResult(allUsers.Count);
        });

        var result = await allUsers.ToPagedResultAsync(pagination, totalCountFunc);

        Assert.Equal(1, callCount);
        Assert.Equal(20, result.Items.Count);
        Assert.Equal(87, result.TotalCount);
        Assert.Equal(5, result.TotalPages);
        Assert.Equal("user-41", result.Items[0].Id);
        Assert.Equal("user-60", result.Items[19].Id);
    }

    [Fact]
    public void ToPagedResultWithReadOnlyListShouldCreateResult()
    {
        var allUsers = GenerateUsers(42);
        var pagination = new PaginationRequest(5, 10);
        var totalCount = allUsers.Count;
        var pageUsers = allUsers.Skip(pagination.Skip).Take(pagination.Take).ToList().AsReadOnly();

        var result = pageUsers.ToPagedResult(pagination, totalCount);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(42, result.TotalCount);
        Assert.Equal(5, result.TotalPages);
        Assert.Equal("user-41", result.Items[0].Id);
        Assert.Equal("user-42", result.Items[1].Id);
        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public void EmptyPagedResultShouldCreateEmptyResult()
    {
        var pagination = new PaginationRequest(1, 10);

        var result = pagination.EmptyPagedResult<User>();

        Assert.Empty(result.Items);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
        Assert.False(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public void ToPagedResultWithEmptyCollectionShouldReturnEmptyResult()
    {
        var items = new List<User>();
        var pagination = new PaginationRequest(1, 10);

        var result = items.ToPagedResult(pagination);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
        Assert.False(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public void ToPagedResultWithSinglePageShouldWorkCorrectly()
    {
        var allUsers = GenerateUsers(5);
        var pagination = new PaginationRequest(1, 10);

        var result = allUsers.ToPagedResult(pagination);

        Assert.Equal(5, result.Items.Count);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
        Assert.False(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public void ToPagedResultWithLastPageShouldReturnRemainingItems()
    {
        var allProducts = GenerateProducts(47);
        var pagination = new PaginationRequest(5, 10);

        var result = allProducts.ToPagedResult(pagination);

        Assert.Equal(7, result.Items.Count);
        Assert.Equal(47, result.TotalCount);
        Assert.Equal(5, result.TotalPages);
        Assert.Equal("prod-41", result.Items[0].Id);
        Assert.Equal("prod-47", result.Items[6].Id);
        Assert.False(result.HasNextPage);
        Assert.True(result.HasPreviousPage);
    }

    [Fact]
    public void ToPagedResultFirstPageShouldHaveNoPreviousPage()
    {
        var allUsers = GenerateUsers(35);
        var pagination = new PaginationRequest(1, 10);

        var result = allUsers.ToPagedResult(pagination);

        Assert.Equal(10, result.Items.Count);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(35, result.TotalCount);
        Assert.Equal(4, result.TotalPages);
        Assert.Equal("user-1", result.Items[0].Id);
        Assert.False(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public void ToPagedResultExactPageSizeShouldReturnFullPage()
    {
        var allProducts = GenerateProducts(50);
        var pagination = new PaginationRequest(2, 25);

        var result = allProducts.ToPagedResult(pagination);

        Assert.Equal(25, result.Items.Count);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(25, result.PageSize);
        Assert.Equal(50, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal("prod-26", result.Items[0].Id);
        Assert.Equal("prod-50", result.Items[24].Id);
        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public void ToPagedResultAllPagesShouldBeConsistent()
    {
        var allUsers = GenerateUsers(47);
        var pageSize = 10;

        for (var page = 1; page <= 5; page++)
        {
            var pagination = new PaginationRequest(page, pageSize);
            var result = allUsers.ToPagedResult(pagination);

            Assert.Equal(page, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(47, result.TotalCount);
            Assert.Equal(5, result.TotalPages);

            var expectedItemCount = page == 5 ? 7 : 10;
            Assert.Equal(expectedItemCount, result.Items.Count);

            var expectedFirstId = $"user-{((page - 1) * pageSize) + 1}";
            Assert.Equal(expectedFirstId, result.Items[0].Id);

            Assert.Equal(page > 1, result.HasPreviousPage);
            Assert.Equal(page < 5, result.HasNextPage);
        }
    }

    [Fact]
    public void ToPagedResultWithNullSourceShouldThrowException()
    {
        IEnumerable<User> items = null!;
        var pagination = new PaginationRequest(1, 10);

        Assert.Throws<ArgumentNullException>(() => items.ToPagedResult(pagination));
    }

    [Fact]
    public void ToPagedResultWithNullPaginationShouldThrowException()
    {
        var items = GenerateUsers(3);
        PaginationRequest pagination = null!;

        Assert.Throws<ArgumentNullException>(() => items.ToPagedResult(pagination));
    }

    [Fact]
    public void ToPagedResultWithNullTotalCountFuncShouldThrowException()
    {
        var items = GenerateUsers(3);
        var pagination = new PaginationRequest(1, 10);
        Func<int> totalCountFunc = null!;

        Assert.Throws<ArgumentNullException>(() => items.ToPagedResult(pagination, totalCountFunc));
    }

    [Fact]
    public async Task ToPagedResultAsyncWithNullTotalCountFuncShouldThrowException()
    {
        var items = GenerateUsers(3);
        var pagination = new PaginationRequest(1, 10);
        Func<CancellationToken, Task<int>> totalCountFunc = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => items.ToPagedResultAsync(pagination, totalCountFunc));
    }

    [Fact]
    public void ToPagedResultWithQueryableShouldPaginateCorrectly()
    {
        var allUsers = GenerateUsers(73);
        var queryable = allUsers.AsQueryable();
        var pagination = new PaginationRequest(4, 15);

        var result = queryable.ToPagedResult(pagination);

        Assert.Equal(15, result.Items.Count);
        Assert.Equal(4, result.PageNumber);
        Assert.Equal(15, result.PageSize);
        Assert.Equal(73, result.TotalCount);
        Assert.Equal(5, result.TotalPages);
        Assert.Equal("user-46", result.Items[0].Id);
        Assert.Equal("user-60", result.Items[14].Id);
    }

    [Fact]
    public async Task ToPagedResultAsyncWithCancellationTokenShouldRespectCancellation()
    {
        var allUsers = GenerateUsers(47);
        var pagination = new PaginationRequest(2, 10);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var totalCountFunc = new Func<CancellationToken, Task<int>>(ct =>
        {
            ct.ThrowIfCancellationRequested();
            return Task.FromResult(allUsers.Count);
        });

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => allUsers.ToPagedResultAsync(pagination, totalCountFunc, cts.Token));
    }
}

