using Osirisgate.CleanArchitectureCore.Mapper;

namespace Osirisgate.CleanArchitectureCore.Tests.Mapper;

/// <summary>
/// Tests for EntityMapper with advanced setter patterns (fluent API, factory methods, etc.).
/// </summary>
public sealed class EntityMapperAdvancedSettersTest
{
    private sealed class FluentUser
    {
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public int Age { get; private set; }
        public bool IsActive { get; private set; }

        public FluentUser WithName(string name)
        {
            Name = name;
            return this;
        }

        public FluentUser WithEmail(string email)
        {
            Email = email;
            return this;
        }

        public FluentUser SetAge(int age)
        {
            Age = age;
            return this;
        }

        public FluentUser WithIsActive(bool isActive)
        {
            IsActive = isActive;
            return this;
        }
    }

    private sealed class FactoryUser
    {
        public string Id { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;

        public FactoryUser()
        {
        }

        public static FactoryUser Create(string id, string name, string email)
        {
            return new FactoryUser
            {
                Id = id,
                Name = name,
                Email = email
            };
        }

        public static FactoryUser From(string id, string name)
        {
            return new FactoryUser
            {
                Id = id,
                Name = name,
                Email = $"{name}@example.com"
            };
        }
    }

    private sealed class BooleanPropertyUser
    {
        public bool IsActive { get; private set; }
        public bool HasPermission { get; private set; }
        public bool Enabled { get; private set; }
    }

    private sealed class PrivateFieldUser
    {
        private string _name = string.Empty;
        private int _age;

        public string Name
        {
            get => _name;
            private set => _name = value;
        }

        public int Age
        {
            get => _age;
            private set => _age = value;
        }
    }

    private sealed class StandardUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool IsActive { get; set; }
        public bool HasPermission { get; set; }
        public bool Enabled { get; set; }
        public string Id { get; set; } = string.Empty;
    }

    [Fact]
    public void ToDomainWithFluentMethodsShouldMapCorrectly()
    {
        var dto = new StandardUserDto
        {
            Name = "John Doe",
            Email = "john@example.com",
            Age = 30,
            IsActive = true
        };

        var domain = EntityMapper.MapToDomain(
            dto,
            d => new FluentUser()
                .WithName(d.Name)
                .WithEmail(d.Email)
                .SetAge(d.Age)
                .WithIsActive(d.IsActive));

        Assert.Equal(dto.Name, domain.Name);
        Assert.Equal(dto.Email, domain.Email);
        Assert.Equal(dto.Age, domain.Age);
        Assert.Equal(dto.IsActive, domain.IsActive);
    }

    [Fact]
    public void ToPersistenceWithFluentMethodsShouldMapCorrectly()
    {
        var domain = new FluentUser()
            .WithName("Jane Doe")
            .WithEmail("jane@example.com")
            .SetAge(25)
            .WithIsActive(true);

        var dto = EntityMapper.ToPersistence<FluentUser, StandardUserDto>(domain);

        Assert.Equal(domain.Name, dto.Name);
        Assert.Equal(domain.Email, dto.Email);
        Assert.Equal(domain.Age, dto.Age);
        Assert.Equal(domain.IsActive, dto.IsActive);
    }

    [Fact]
    public void ToDomainWithFactoryMethodShouldUseFactory()
    {
        var dto = new StandardUserDto
        {
            Id = "123",
            Name = "Factory User",
            Email = "factory@example.com"
        };

        var domain = EntityMapper.MapToDomain(
            dto,
            d => FactoryUser.Create(d.Id, d.Name, d.Email)
        );

        Assert.Equal(dto.Id, domain.Id);
        Assert.Equal(dto.Name, domain.Name);
        Assert.Equal(dto.Email, domain.Email);
    }

    [Fact]
    public void ToDomainWithBooleanPropertiesShouldMapCorrectly()
    {
        var dto = new StandardUserDto
        {
            IsActive = true,
            HasPermission = false,
            Enabled = true
        };

        var domain = EntityMapper.MapToDomain(
            dto,
            d =>
            {
                var user = new BooleanPropertyUser();
                typeof(BooleanPropertyUser).GetProperty("IsActive")!.GetSetMethod(true)!.Invoke(user, [d.IsActive]);
                typeof(BooleanPropertyUser).GetProperty("HasPermission")!.GetSetMethod(true)!.Invoke(user, [d.HasPermission]);
                typeof(BooleanPropertyUser).GetProperty("Enabled")!.GetSetMethod(true)!.Invoke(user, [d.Enabled]);
                return user;
            }
        );

        Assert.Equal(dto.IsActive, domain.IsActive);
        Assert.Equal(dto.HasPermission, domain.HasPermission);
        Assert.Equal(dto.Enabled, domain.Enabled);
    }

    [Fact]
    public void ToDomainWithPrivateFieldsShouldMapCorrectly()
    {
        var dto = new StandardUserDto
        {
            Name = "Private Field User",
            Age = 35
        };

        var domain = EntityMapper.MapToDomain(
            dto,
            d =>
            {
                var user = new PrivateFieldUser();
                typeof(PrivateFieldUser).GetProperty("Name")!.GetSetMethod(true)!.Invoke(user, [d.Name]);
                typeof(PrivateFieldUser).GetProperty("Age")!.GetSetMethod(true)!.Invoke(user, [d.Age]);
                return user;
            }
        );

        Assert.Equal(dto.Name, domain.Name);
        Assert.Equal(dto.Age, domain.Age);
    }

    [Fact]
    public void ToPersistenceWithPrivateFieldsShouldMapCorrectly()
    {
        var domain = new PrivateFieldUser();
        typeof(PrivateFieldUser).GetProperty("Name")!.GetSetMethod(true)!.Invoke(domain, ["Test User"]);
        typeof(PrivateFieldUser).GetProperty("Age")!.GetSetMethod(true)!.Invoke(domain, [28]);

        var dto = EntityMapper.ToPersistence<PrivateFieldUser, StandardUserDto>(domain);

        Assert.Equal(domain.Name, dto.Name);
        Assert.Equal(domain.Age, dto.Age);
    }

    [Fact]
    public void ToDomainListWithFluentMethodsShouldMapList()
    {
        var dtos = new List<StandardUserDto>
        {
            new() { Name = "User 1", Email = "user1@example.com", Age = 20 },
            new() { Name = "User 2", Email = "user2@example.com", Age = 25 }
        };

        var domains = EntityMapper.MapListToDomain(
            dtos,
            d => new FluentUser()
                .WithName(d.Name)
                .WithEmail(d.Email)
                .SetAge(d.Age)
        );

        Assert.Equal(2, domains.Count);
        Assert.Equal(dtos[0].Name, domains[0].Name);
        Assert.Equal(dtos[1].Name, domains[1].Name);
    }

    [Fact]
    public void ToDomainListWithFactoryMethodsShouldMapList()
    {
        var dtos = new List<StandardUserDto>
        {
            new() { Id = "1", Name = "Factory User 1", Email = "factory1@example.com" },
            new() { Id = "2", Name = "Factory User 2", Email = "factory2@example.com" }
        };

        var domains = EntityMapper.MapListToDomain(
            dtos,
            d => FactoryUser.Create(d.Id, d.Name, d.Email)
        );

        Assert.Equal(2, domains.Count);
        Assert.Equal(dtos[0].Id, domains[0].Id);
        Assert.Equal(dtos[1].Id, domains[1].Id);
    }

    [Fact]
    public void ToPersistenceListWithFluentMethodsShouldMapList()
    {
        var domains = new List<FluentUser>
        {
            new FluentUser().WithName("User 1").WithEmail("user1@example.com").SetAge(20),
            new FluentUser().WithName("User 2").WithEmail("user2@example.com").SetAge(25)
        };

        var dtos = EntityMapper.ToPersistenceList<FluentUser, StandardUserDto>(domains);

        Assert.Equal(2, dtos.Count);
        Assert.Equal(domains[0].Name, dtos[0].Name);
        Assert.Equal(domains[1].Name, dtos[1].Name);
    }
}
