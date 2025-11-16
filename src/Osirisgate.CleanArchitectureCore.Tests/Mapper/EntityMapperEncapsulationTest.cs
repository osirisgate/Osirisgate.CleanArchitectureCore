using Osirisgate.CleanArchitectureCore.Mapper;

namespace Osirisgate.CleanArchitectureCore.Tests.Mapper;

/// <summary>
/// Tests for EntityMapper with encapsulated domain models (private setters, constructors).
/// </summary>
public sealed class EntityMapperEncapsulationTest
{
    private sealed class EncapsulatedDomainUser
    {
        private readonly string _id;
        private readonly string _name;
        private readonly string _email;

        public string Id
        {
            get => _id;
            private init => _id = value;
        }

        public string Name
        {
            get => _name;
            private init => _name = value;
        }

        public string Email
        {
            get => _email;
            private init => _email = value;
        }

        public EncapsulatedDomainUser(string id, string name, string email)
        {
            _id = id;
            _name = name;
            _email = email;
        }

        public EncapsulatedDomainUser()
            : this(string.Empty, string.Empty, string.Empty)
        {
        }
    }

    private sealed class DomainUserWithPrivateSetters
    {
        private string _id = string.Empty;
        private string _name = string.Empty;

        public string Id
        {
            get => _id;
            private set => _id = value;
        }

        public string Name
        {
            get => _name;
            private set => _name = value;
        }

        public DomainUserWithPrivateSetters(string id, string name)
        {
            _id = id;
            _name = name;
        }

        public DomainUserWithPrivateSetters()
        {
        }
    }

    private sealed class PersistenceUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    [Fact]
    public void ToPersistenceWithEncapsulatedDomainShouldMapCorrectly()
    {
        var domain = new EncapsulatedDomainUser("1", "John", "john@example.com");

        var persistence = EntityMapper.ToPersistence<EncapsulatedDomainUser, PersistenceUserDto>(domain);

        Assert.Equal(domain.Id, persistence.Id);
        Assert.Equal(domain.Name, persistence.Name);
        Assert.Equal(domain.Email, persistence.Email);
    }

    [Fact]
    public void ToDomainWithEncapsulatedDomainShouldMapCorrectly()
    {
        var persistence = new PersistenceUserDto { Id = "1", Name = "John", Email = "john@example.com" };

        var domain = EntityMapper.MapToDomain(
            persistence,
            p => new EncapsulatedDomainUser(p.Id, p.Name, p.Email));

        Assert.Equal(persistence.Id, domain.Id);
        Assert.Equal(persistence.Name, domain.Name);
        Assert.Equal(persistence.Email, domain.Email);
    }

    [Fact]
    public void ToPersistenceListWithEncapsulatedDomainShouldMapList()
    {
        var domains = new List<EncapsulatedDomainUser>
        {
            new("1", "John", "john@example.com"),
            new("2", "Jane", "jane@example.com")
        };

        var persistences = EntityMapper.ToPersistenceList<EncapsulatedDomainUser, PersistenceUserDto>(domains);

        Assert.Equal(2, persistences.Count);
        Assert.Equal(domains[0].Id, persistences[0].Id);
        Assert.Equal(domains[1].Id, persistences[1].Id);
    }

    [Fact]
    public void ToDomainListWithEncapsulatedDomainShouldMapList()
    {
        var persistences = new List<PersistenceUserDto>
        {
            new() { Id = "1", Name = "John", Email = "john@example.com" },
            new() { Id = "2", Name = "Jane", Email = "jane@example.com" }
        };

        var domains = EntityMapper.MapListToDomain(
            persistences,
            p => new EncapsulatedDomainUser(p.Id, p.Name, p.Email));

        Assert.Equal(2, domains.Count);
        Assert.Equal(persistences[0].Id, domains[0].Id);
        Assert.Equal(persistences[1].Id, domains[1].Id);
    }

    [Fact]
    public void ToPersistenceWithPrivateSettersShouldMapCorrectly()
    {
        var domain = new DomainUserWithPrivateSetters("1", "John");

        var persistence = EntityMapper.ToPersistence<DomainUserWithPrivateSetters, PersistenceUserDto>(domain);

        Assert.Equal(domain.Id, persistence.Id);
        Assert.Equal(domain.Name, persistence.Name);
    }

    [Fact]
    public void ToDomainWithPrivateSettersShouldMapCorrectly()
    {
        var persistence = new PersistenceUserDto { Id = "1", Name = "John", Email = "john@example.com" };

        var domain = EntityMapper.MapToDomain(
            persistence,
            p => new DomainUserWithPrivateSetters(p.Id, p.Name));

        Assert.Equal(persistence.Id, domain.Id);
        Assert.Equal(persistence.Name, domain.Name);
    }

    [Fact]
    public void ToDomainWithReadOnlyPropertiesShouldMapUsingConstructor()
    {
        var persistence = new PersistenceUserDto { Id = "1", Name = "John", Email = "john@example.com" };

        var domain = EntityMapper.MapToDomain(
            persistence,
            p => new EncapsulatedDomainUser(p.Id, p.Name, p.Email));

        Assert.Equal(persistence.Id, domain.Id);
        Assert.Equal(persistence.Name, domain.Name);
        Assert.Equal(persistence.Email, domain.Email);
    }

    [Fact]
    public void ToPersistenceListWithPrivateSettersShouldMapList()
    {
        var domains = new List<DomainUserWithPrivateSetters>
        {
            new("1", "John"),
            new("2", "Jane")
        };

        var persistences = EntityMapper.ToPersistenceList<DomainUserWithPrivateSetters, PersistenceUserDto>(domains);

        Assert.Equal(2, persistences.Count);
        Assert.Equal(domains[0].Id, persistences[0].Id);
        Assert.Equal(domains[1].Id, persistences[1].Id);
    }

    [Fact]
    public void ToDomainListWithPrivateSettersShouldMapList()
    {
        var persistences = new List<PersistenceUserDto>
        {
            new() { Id = "1", Name = "John", Email = "john@example.com" },
            new() { Id = "2", Name = "Jane", Email = "jane@example.com" }
        };

        var domains = EntityMapper.MapListToDomain(
            persistences,
            p => new DomainUserWithPrivateSetters(p.Id, p.Name)
        );

        Assert.Equal(2, domains.Count);
        Assert.Equal(persistences[0].Id, domains[0].Id);
        Assert.Equal(persistences[1].Id, domains[1].Id);
    }
}

