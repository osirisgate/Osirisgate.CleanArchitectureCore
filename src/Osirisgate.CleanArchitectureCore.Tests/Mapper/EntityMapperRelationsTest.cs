using Osirisgate.CleanArchitectureCore.Mapper;

namespace Osirisgate.CleanArchitectureCore.Tests.Mapper;

/// <summary>
/// Tests for EntityMapper with object relations and collections.
/// </summary>
public sealed class EntityMapperRelationsTest
{
    private sealed class DomainOrder
    {
        public string Id { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public List<DomainOrderItem> Items { get; set; } = [];
        public DomainAddress? ShippingAddress { get; set; }
        public DomainCustomer? Customer { get; set; }
    }

    private sealed class DomainOrderItem
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DomainProduct? Product { get; set; }
    }

    private sealed class DomainAddress
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    private sealed class DomainCustomer
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<DomainAddress> Addresses { get; set; } = [];
    }

    private sealed class DomainProduct
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<string> Tags { get; set; } = [];
    }

    private sealed class PersistenceOrderDto
    {
        public string Id { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public List<PersistenceOrderItemDto> Items { get; set; } = [];
        public PersistenceAddressDto? ShippingAddress { get; set; }
        public PersistenceCustomerDto? Customer { get; set; }
    }

    private sealed class PersistenceOrderItemDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public PersistenceProductDto? Product { get; set; }
    }

    private sealed class PersistenceAddressDto
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    private sealed class PersistenceCustomerDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<PersistenceAddressDto> Addresses { get; set; } = [];
    }

    private sealed class PersistenceProductDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<string> Tags { get; set; } = [];
    }

    [Fact]
    public void ToPersistenceWithNestedObjectsShouldMapCorrectly()
    {
        var domain = new DomainOrder
        {
            Id = "order-1",
            CustomerId = "customer-1",
            OrderDate = new DateTime(2024, 1, 15),
            ShippingAddress = new DomainAddress
            {
                Street = "123 Main St",
                City = "Paris",
                ZipCode = "75001",
                Country = "France"
            }
        };

        var persistence = EntityMapper.ToPersistence<DomainOrder, PersistenceOrderDto>(domain);

        Assert.Equal(domain.Id, persistence.Id);
        Assert.Equal(domain.CustomerId, persistence.CustomerId);
        Assert.Equal(domain.OrderDate, persistence.OrderDate);
        Assert.NotNull(persistence.ShippingAddress);
        Assert.Equal(domain.ShippingAddress!.Street, persistence.ShippingAddress.Street);
        Assert.Equal(domain.ShippingAddress.City, persistence.ShippingAddress.City);
        Assert.Equal(domain.ShippingAddress.ZipCode, persistence.ShippingAddress.ZipCode);
        Assert.Equal(domain.ShippingAddress.Country, persistence.ShippingAddress.Country);
    }

    [Fact]
    public void ToPersistenceWithCollectionOfObjectsShouldMapCorrectly()
    {
        var domain = new DomainOrder
        {
            Id = "order-1",
            Items =
            [
                new()
                {
                    ProductId = "prod-1",
                    ProductName = "Product 1",
                    Quantity = 2,
                    Price = 29.99m
                },
                new()
                {
                    ProductId = "prod-2",
                    ProductName = "Product 2",
                    Quantity = 1,
                    Price = 49.99m
                }
            ]
        };

        var persistence = EntityMapper.ToPersistence<DomainOrder, PersistenceOrderDto>(domain);

        Assert.Equal(2, persistence.Items.Count);
        Assert.Equal(domain.Items[0].ProductId, persistence.Items[0].ProductId);
        Assert.Equal(domain.Items[0].ProductName, persistence.Items[0].ProductName);
        Assert.Equal(domain.Items[0].Quantity, persistence.Items[0].Quantity);
        Assert.Equal(domain.Items[0].Price, persistence.Items[0].Price);
        Assert.Equal(domain.Items[1].ProductId, persistence.Items[1].ProductId);
    }

    [Fact]
    public void ToPersistenceWithNestedCollectionsShouldMapCorrectly()
    {
        var domain = new DomainOrder
        {
            Id = "order-1",
            Customer = new DomainCustomer
            {
                Id = "customer-1",
                Name = "John Doe",
                Email = "john@example.com",
                Addresses =
                [
                    new()
                    {
                        Street = "123 Main St",
                        City = "Paris",
                        ZipCode = "75001",
                        Country = "France"
                    },
                    new()
                    {
                        Street = "456 Oak Ave",
                        City = "Lyon",
                        ZipCode = "69001",
                        Country = "France"
                    }
                ]
            }
        };

        var persistence = EntityMapper.ToPersistence<DomainOrder, PersistenceOrderDto>(domain);

        Assert.NotNull(persistence.Customer);
        Assert.Equal(domain.Customer!.Id, persistence.Customer.Id);
        Assert.Equal(2, persistence.Customer.Addresses.Count);
        Assert.Equal(domain.Customer.Addresses[0].Street, persistence.Customer.Addresses[0].Street);
        Assert.Equal(domain.Customer.Addresses[1].City, persistence.Customer.Addresses[1].City);
    }

    [Fact]
    public void ToPersistenceWithDeeplyNestedRelationsShouldMapCorrectly()
    {
        var domain = new DomainOrder
        {
            Id = "order-1",
            Items =
            [
                new()
                {
                    ProductId = "prod-1",
                    ProductName = "Product 1",
                    Quantity = 2,
                    Price = 29.99m,
                    Product = new DomainProduct
                    {
                        Id = "prod-1",
                        Name = "Product 1",
                        Price = 29.99m,
                        Tags = ["electronics", "popular", "sale"]
                    }
                }
            ]
        };

        var persistence = EntityMapper.ToPersistence<DomainOrder, PersistenceOrderDto>(domain);

        Assert.Single(persistence.Items);
        var product = persistence.Items[0].Product;
        Assert.NotNull(product);
        var domainProduct = domain.Items[0].Product;
        Assert.NotNull(domainProduct);
        Assert.Equal(domainProduct.Id, product.Id);
        Assert.Equal(domainProduct.Name, product.Name);
        Assert.Equal(3, product.Tags.Count);
        Assert.Equal("electronics", product.Tags[0]);
        Assert.Equal("popular", product.Tags[1]);
        Assert.Equal("sale", product.Tags[2]);
    }

    [Fact]
    public void ToDomainWithNestedObjectsShouldMapCorrectly()
    {
        var persistence = new PersistenceOrderDto
        {
            Id = "order-1",
            CustomerId = "customer-1",
            OrderDate = new DateTime(2024, 1, 15),
            ShippingAddress = new PersistenceAddressDto
            {
                Street = "123 Main St",
                City = "Paris",
                ZipCode = "75001",
                Country = "France"
            }
        };

        var domain = EntityMapper.ToDomain<PersistenceOrderDto, DomainOrder>(persistence);

        Assert.Equal(persistence.Id, domain.Id);
        Assert.NotNull(domain.ShippingAddress);
        Assert.Equal(persistence.ShippingAddress!.Street, domain.ShippingAddress.Street);
        Assert.Equal(persistence.ShippingAddress.City, domain.ShippingAddress.City);
    }

    [Fact]
    public void ToDomainWithCollectionOfObjectsShouldMapCorrectly()
    {
        var persistence = new PersistenceOrderDto
        {
            Id = "order-1",
            Items =
            [
                new()
                {
                    ProductId = "prod-1",
                    ProductName = "Product 1",
                    Quantity = 2,
                    Price = 29.99m
                },
                new()
                {
                    ProductId = "prod-2",
                    ProductName = "Product 2",
                    Quantity = 1,
                    Price = 49.99m
                }
            ]
        };

        var domain = EntityMapper.ToDomain<PersistenceOrderDto, DomainOrder>(persistence);

        Assert.Equal(2, domain.Items.Count);
        Assert.Equal(persistence.Items[0].ProductId, domain.Items[0].ProductId);
        Assert.Equal(persistence.Items[0].Quantity, domain.Items[0].Quantity);
    }

    [Fact]
    public void ToPersistenceListWithRelationsShouldMapList()
    {
        var domains = new List<DomainOrder>
        {
            new()
            {
                Id = "order-1",
                CustomerId = "customer-1",
                Items =
                [
                    new() { ProductId = "prod-1", Quantity = 2 }
                ]
            },
            new()
            {
                Id = "order-2",
                CustomerId = "customer-2",
                Items =
                [
                    new() { ProductId = "prod-2", Quantity = 1 }
                ]
            }
        };

        var persistences = EntityMapper.ToPersistenceList<DomainOrder, PersistenceOrderDto>(domains);

        Assert.Equal(2, persistences.Count);
        Assert.Equal(domains[0].Id, persistences[0].Id);
        Assert.Single(persistences[0].Items);
        Assert.Equal(domains[1].Id, persistences[1].Id);
        Assert.Single(persistences[1].Items);
    }

    [Fact]
    public void ToDomainListWithRelationsShouldMapList()
    {
        var persistences = new List<PersistenceOrderDto>
        {
            new()
            {
                Id = "order-1",
                Items =
                [
                    new() { ProductId = "prod-1", Quantity = 2 }
                ]
            },
            new()
            {
                Id = "order-2",
                Items =
                [
                    new() { ProductId = "prod-2", Quantity = 1 }
                ]
            }
        };

        var domains = EntityMapper.ToDomainList<PersistenceOrderDto, DomainOrder>(persistences);

        Assert.Equal(2, domains.Count);
        Assert.Equal(persistences[0].Id, domains[0].Id);
        Assert.Single(domains[0].Items);
        Assert.Equal(persistences[1].Id, domains[1].Id);
        Assert.Single(domains[1].Items);
    }

    [Fact]
    public void ToPersistenceWithEmptyCollectionsShouldMapCorrectly()
    {
        var domain = new DomainOrder
        {
            Id = "order-1",
            Items = []
        };

        var persistence = EntityMapper.ToPersistence<DomainOrder, PersistenceOrderDto>(domain);

        Assert.NotNull(persistence.Items);
        Assert.Empty(persistence.Items);
    }

    [Fact]
    public void ToPersistenceWithNullNestedObjectShouldMapCorrectly()
    {
        var domain = new DomainOrder
        {
            Id = "order-1",
            ShippingAddress = null,
            Customer = null
        };

        var persistence = EntityMapper.ToPersistence<DomainOrder, PersistenceOrderDto>(domain);

        Assert.Null(persistence.ShippingAddress);
        Assert.Null(persistence.Customer);
    }

    [Fact]
    public void ToPersistenceWithComplexNestedStructureShouldMapAllLevels()
    {
        var domain = new DomainOrder
        {
            Id = "order-1",
            Customer = new DomainCustomer
            {
                Id = "customer-1",
                Addresses =
                [
                    new() { Street = "123 Main St", City = "Paris" }
                ]
            },
            Items =
            [
                new()
                {
                    Product = new DomainProduct
                    {
                        Id = "prod-1",
                        Tags = ["tag1", "tag2"]
                    }
                }
            ]
        };

        var persistence = EntityMapper.ToPersistence<DomainOrder, PersistenceOrderDto>(domain);

        Assert.NotNull(persistence.Customer);
        Assert.Single(persistence.Customer!.Addresses);
        Assert.Single(persistence.Items);
        Assert.NotNull(persistence.Items[0].Product);
        Assert.Equal(2, persistence.Items[0].Product!.Tags.Count);
    }

}
