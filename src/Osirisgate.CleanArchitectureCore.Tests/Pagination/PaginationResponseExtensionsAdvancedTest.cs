using Osirisgate.CleanArchitectureCore.Pagination;
using Osirisgate.CleanArchitectureCore.Response;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;

namespace Osirisgate.CleanArchitectureCore.Tests.Pagination;

public sealed class PaginationResponseExtensionsAdvancedTest
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
    public void SetPagedDataWithCollectionShouldPaginateAndSetInResponse()
    {
        var response = CleanArchitectureCore.Response.Response.Create(
            true,
            StatusCode.Ok,
            "Users retrieved",
            new Dictionary<string, object>());

        var allUsers = GenerateUsers(64);
        var pagination = new PaginationRequest(2, 15);

        var result = response.SetPagedData(allUsers, pagination);

        var pagedResult = response.GetPagedResult<User>();
        Assert.NotNull(pagedResult);
        Assert.Equal(15, pagedResult!.Items.Count);
        Assert.Equal(2, pagedResult.PageNumber);
        Assert.Equal(15, pagedResult.PageSize);
        Assert.Equal(64, pagedResult.TotalCount);
        Assert.Equal(5, pagedResult.TotalPages);
        Assert.Equal("user-16", pagedResult.Items[0].Id);
        Assert.Equal("user-30", pagedResult.Items[14].Id);
        Assert.True(pagedResult.HasPreviousPage);
        Assert.True(pagedResult.HasNextPage);
        Assert.Same(response, result);
    }

    [Fact]
    public void SetPagedDataWithTotalCountShouldUseProvidedCount()
    {
        var response = CleanArchitectureCore.Response.Response.Create(
            true,
            StatusCode.Ok,
            "Products retrieved",
            new Dictionary<string, object>());

        var allProducts = GenerateProducts(35);
        var pagination = new PaginationRequest(1, 10);
        var totalCount = allProducts.Count;
        var pageProducts = allProducts.Skip(pagination.Skip).Take(pagination.Take).ToList();

        response.SetPagedData(pageProducts, pagination, totalCount);

        var pagedResult = response.GetPagedResult<Product>();
        Assert.NotNull(pagedResult);
        Assert.Equal(10, pagedResult!.Items.Count);
        Assert.Equal(1, pagedResult.PageNumber);
        Assert.Equal(35, pagedResult.TotalCount);
        Assert.Equal(4, pagedResult.TotalPages);
        Assert.Equal("prod-1", pagedResult.Items[0].Id);
        Assert.False(pagedResult.HasPreviousPage);
        Assert.True(pagedResult.HasNextPage);
    }

    [Fact]
    public void SetPagedDataWithTotalCountFuncShouldCallFunction()
    {
        var response = CleanArchitectureCore.Response.Response.Create(
            true,
            StatusCode.Ok,
            "Users retrieved",
            new Dictionary<string, object>());

        var allUsers = GenerateUsers(100);
        var pagination = new PaginationRequest(3, 20);
        var callCount = 0;
        int totalCountFunc() { callCount++; return allUsers.Count; }

        response.SetPagedData(allUsers, pagination, totalCountFunc);

        Assert.Equal(1, callCount);
        var pagedResult = response.GetPagedResult<User>();
        Assert.NotNull(pagedResult);
        Assert.Equal(100, pagedResult!.TotalCount);
        Assert.Equal(5, pagedResult.TotalPages);
        Assert.Equal(20, pagedResult.Items.Count);
        Assert.Equal("user-41", pagedResult.Items[0].Id);
    }

    [Fact]
    public async Task TestSetPagedDataAsyncShouldPaginateAsync()
    {
        var response = CleanArchitectureCore.Response.Response.Create(
            true,
            StatusCode.Ok,
            "Users retrieved",
            new Dictionary<string, object>());

        var allUsers = GenerateUsers(87);
        var pagination = new PaginationRequest(3, 20);
        var callCount = 0;
        var totalCountFunc = new Func<CancellationToken, Task<int>>(ct =>
        {
            callCount++;
            return Task.FromResult(allUsers.Count);
        });

        var result = await response.SetPagedDataAsync(allUsers, pagination, totalCountFunc);

        Assert.Equal(1, callCount);
        var pagedResult = response.GetPagedResult<User>();
        Assert.NotNull(pagedResult);
        Assert.Equal(87, pagedResult!.TotalCount);
        Assert.Equal(5, pagedResult.TotalPages);
        Assert.Equal(20, pagedResult.Items.Count);
        Assert.Equal("user-41", pagedResult.Items[0].Id);
        Assert.Equal("user-60", pagedResult.Items[19].Id);
        Assert.Same(response, result);
    }

    [Fact]
    public void SetPagedDataFirstPageShouldHaveNoPreviousPage()
    {
        var response = CleanArchitectureCore.Response.Response.Create(
            true,
            StatusCode.Ok,
            "Products retrieved",
            new Dictionary<string, object>());

        var allProducts = GenerateProducts(35);
        var pagination = new PaginationRequest(1, 10);
        var totalCount = allProducts.Count;
        var pageProducts = allProducts.Skip(pagination.Skip).Take(pagination.Take).ToList();

        response.SetPagedData(pageProducts, pagination, totalCount);

        var pagedResult = response.GetPagedResult<Product>();
        Assert.NotNull(pagedResult);
        Assert.Equal(10, pagedResult!.Items.Count);
        Assert.Equal(1, pagedResult.PageNumber);
        Assert.Equal(35, pagedResult.TotalCount);
        Assert.False(pagedResult.HasPreviousPage);
        Assert.True(pagedResult.HasNextPage);
    }

    [Fact]
    public void SetPagedDataLastPageShouldHaveNoNextPage()
    {
        var response = CleanArchitectureCore.Response.Response.Create(
            true,
            StatusCode.Ok,
            "Users retrieved",
            new Dictionary<string, object>());

        var allUsers = GenerateUsers(42);
        var pagination = new PaginationRequest(5, 10);
        var totalCount = allUsers.Count;
        var pageUsers = allUsers.Skip(pagination.Skip).Take(pagination.Take).ToList();

        response.SetPagedData(pageUsers, pagination, totalCount);

        var pagedResult = response.GetPagedResult<User>();
        Assert.NotNull(pagedResult);
        Assert.Equal(2, pagedResult!.Items.Count);
        Assert.Equal(5, pagedResult.PageNumber);
        Assert.Equal(42, pagedResult.TotalCount);
        Assert.True(pagedResult.HasPreviousPage);
        Assert.False(pagedResult.HasNextPage);
        Assert.Equal("user-41", pagedResult.Items[0].Id);
        Assert.Equal("user-42", pagedResult.Items[1].Id);
    }

    [Fact]
    public void SetPagedDataWithNullResponseShouldThrowException()
    {
        var items = GenerateUsers(3);
        var pagination = new PaginationRequest(1, 10);

        IResponse response = null!;
        Assert.Throws<ArgumentNullException>(() => response.SetPagedData(items, pagination));
    }

    [Fact]
    public void SetPagedDataWithNullSourceShouldThrowException()
    {
        var response = CleanArchitectureCore.Response.Response.Create(
            true,
            StatusCode.Ok,
            "success",
            new Dictionary<string, object>());

        IEnumerable<User> items = null!;
        var pagination = new PaginationRequest(1, 10);

        Assert.Throws<ArgumentNullException>(() => response.SetPagedData(items, pagination));
    }

    [Fact]
    public void SetPagedDataWithNullPaginationShouldThrowException()
    {
        var response = CleanArchitectureCore.Response.Response.Create(
            true,
            StatusCode.Ok,
            "success",
            new Dictionary<string, object>());

        var items = GenerateUsers(3);
        PaginationRequest pagination = null!;

        Assert.Throws<ArgumentNullException>(() => response.SetPagedData(items, pagination));
    }

    [Fact]
    public void SetPagedDataWithNullTotalCountFuncShouldThrowException()
    {
        var response = CleanArchitectureCore.Response.Response.Create(
            true,
            StatusCode.Ok,
            "success",
            new Dictionary<string, object>());

        var items = GenerateUsers(3);
        var pagination = new PaginationRequest(1, 10);
        Func<int> totalCountFunc = null!;

        Assert.Throws<ArgumentNullException>(() => response.SetPagedData(items, pagination, totalCountFunc));
    }

    [Fact]
    public async Task TestSetPagedDataAsyncWithNullTotalCountFuncShouldThrowException()
    {
        var response = CleanArchitectureCore.Response.Response.Create(
            true,
            StatusCode.Ok,
            "success",
            new Dictionary<string, object>());

        var items = GenerateUsers(3);
        var pagination = new PaginationRequest(1, 10);
        Func<CancellationToken, Task<int>> totalCountFunc = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => response.SetPagedDataAsync(items, pagination, totalCountFunc));
    }

    [Fact]
    public void SetPagedResultShouldReturnResponseForChaining()
    {
        var response = CleanArchitectureCore.Response.Response.Create(
            true,
            StatusCode.Ok,
            "success",
            new Dictionary<string, object>());

        var items = GenerateUsers(3);
        var pagedResult = PagedResult<User>.Create(items, 1, 10, 3);

        var result = response.SetPagedResult(pagedResult);

        Assert.Same(response, result);
    }
}

