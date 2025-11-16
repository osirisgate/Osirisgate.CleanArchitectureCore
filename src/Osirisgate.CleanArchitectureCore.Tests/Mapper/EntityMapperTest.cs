using Osirisgate.CleanArchitectureCore.Mapper;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;

namespace Osirisgate.CleanArchitectureCore.Tests.Mapper;

public sealed class EntityMapperTest
{
    private sealed class DomainUser
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    private sealed class PersistenceUser
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    [Fact]
    public void TestToPersistenceShouldMapDomainToPersistence()
    {
        var domain = new DomainUser { Id = "1", Name = "John", Email = "john@example.com" };

        var persistence = EntityMapper.ToPersistence<DomainUser, PersistenceUser>(domain);

        Assert.Equal(domain.Id, persistence.Id);
        Assert.Equal(domain.Name, persistence.Name);
        Assert.Equal(domain.Email, persistence.Email);
    }

    [Fact]
    public void TestToDomainShouldMapPersistenceToDomain()
    {
        var persistence = new PersistenceUser { Id = "1", Name = "John", Email = "john@example.com" };

        var domain = EntityMapper.ToDomain<PersistenceUser, DomainUser>(persistence);

        Assert.Equal(persistence.Id, domain.Id);
        Assert.Equal(persistence.Name, domain.Name);
        Assert.Equal(persistence.Email, domain.Email);
    }

    [Fact]
    public void TestToPersistenceWithNullDomainShouldThrowException()
    {
        DomainUser domain = null!;
        Assert.Throws<ArgumentNullException>(() => EntityMapper.ToPersistence<DomainUser, PersistenceUser>(domain));
    }

    [Fact]
    public void TestToDomainWithNullPersistenceShouldThrowException()
    {
        PersistenceUser persistence = null!;
        Assert.Throws<ArgumentNullException>(() => EntityMapper.ToDomain<PersistenceUser, DomainUser>(persistence));
    }

    [Fact]
    public void TestToPersistenceListShouldMapDomainListToPersistenceList()
    {
        var domains = new List<DomainUser>
        {
            new() { Id = "1", Name = "John", Email = "john@example.com" },
            new() { Id = "2", Name = "Jane", Email = "jane@example.com" }
        };

        var persistences = EntityMapper.ToPersistenceList<DomainUser, PersistenceUser>(domains);

        Assert.Equal(2, persistences.Count);
        Assert.Equal(domains[0].Id, persistences[0].Id);
        Assert.Equal(domains[1].Id, persistences[1].Id);
    }

    [Fact]
    public void TestToDomainListShouldMapPersistenceListToDomainList()
    {
        var persistences = new List<PersistenceUser>
        {
            new() { Id = "1", Name = "John", Email = "john@example.com" },
            new() { Id = "2", Name = "Jane", Email = "jane@example.com" }
        };

        var domains = EntityMapper.ToDomainList<PersistenceUser, DomainUser>(persistences);

        Assert.Equal(2, domains.Count);
        Assert.Equal(persistences[0].Id, domains[0].Id);
        Assert.Equal(persistences[1].Id, domains[1].Id);
    }

    [Fact]
    public void TestToPersistenceListWithNullListShouldThrowException()
    {
        IEnumerable<DomainUser> domains = null!;
        Assert.Throws<ArgumentNullException>(() => EntityMapper.ToPersistenceList<DomainUser, PersistenceUser>(domains));
    }

    [Fact]
    public void TestToDomainListWithNullListShouldThrowException()
    {
        IEnumerable<PersistenceUser> persistences = null!;
        Assert.Throws<ArgumentNullException>(() => EntityMapper.ToDomainList<PersistenceUser, DomainUser>(persistences));
    }

    [Fact]
    public void TestMapToPersistenceWithCustomMapperShouldMapCorrectly()
    {
        var domain = new DomainUser { Id = "1", Name = "John", Email = "john@example.com" };

        var persistence = EntityMapper.MapToPersistence<DomainUser, PersistenceUser>(
            domain,
            d => new PersistenceUser { Id = d.Id, Name = d.Name.ToUpper(), Email = d.Email });

        Assert.Equal(domain.Id, persistence.Id);
        Assert.Equal("JOHN", persistence.Name);
        Assert.Equal(domain.Email, persistence.Email);
    }

    [Fact]
    public void TestMapToDomainWithCustomMapperShouldMapCorrectly()
    {
        var persistence = new PersistenceUser { Id = "1", Name = "JOHN", Email = "john@example.com" };

        var domain = EntityMapper.MapToDomain<PersistenceUser, DomainUser>(
            persistence,
            p => new DomainUser { Id = p.Id, Name = p.Name.ToLower(), Email = p.Email });

        Assert.Equal(persistence.Id, domain.Id);
        Assert.Equal("john", domain.Name);
        Assert.Equal(persistence.Email, domain.Email);
    }

    [Fact]
    public void TestMapListToPersistenceWithCustomMapperShouldMapList()
    {
        var domains = new List<DomainUser>
        {
            new() { Id = "1", Name = "John", Email = "john@example.com" }
        };

        var persistences = EntityMapper.MapListToPersistence<DomainUser, PersistenceUser>(
            domains,
            d => new PersistenceUser { Id = d.Id, Name = d.Name.ToUpper(), Email = d.Email });

        Assert.Single(persistences);
        Assert.Equal("JOHN", persistences[0].Name);
    }

    [Fact]
    public void TestMapListToDomainWithCustomMapperShouldMapList()
    {
        var persistences = new List<PersistenceUser>
        {
            new() { Id = "1", Name = "JOHN", Email = "john@example.com" }
        };

        var domains = EntityMapper.MapListToDomain<PersistenceUser, DomainUser>(
            persistences,
            p => new DomainUser { Id = p.Id, Name = p.Name.ToLower(), Email = p.Email });

        Assert.Single(domains);
        Assert.Equal("john", domains[0].Name);
    }

    [Fact]
    public void TestMapToPersistenceWithNullMapperShouldThrowException()
    {
        var domain = new DomainUser();
        Func<DomainUser, PersistenceUser> mapper = null!;

        Assert.Throws<ArgumentNullException>(() => EntityMapper.MapToPersistence(domain, mapper));
    }

    [Fact]
    public void TestMapToDomainWithNullMapperShouldThrowException()
    {
        var persistence = new PersistenceUser();
        Func<PersistenceUser, DomainUser> mapper = null!;

        Assert.Throws<ArgumentNullException>(() => EntityMapper.MapToDomain(persistence, mapper));
    }

    [Fact]
    public void TestUpdatePersistenceShouldUpdateExistingEntity()
    {
        var existing = new PersistenceUser { Id = "1", Name = "Old", Email = "old@example.com" };
        var domain = new DomainUser { Id = "1", Name = "New", Email = "new@example.com" };

        EntityMapper.UpdatePersistence<DomainUser, PersistenceUser>(domain, existing);

        Assert.Equal("1", existing.Id);
        Assert.Equal("New", existing.Name);
        Assert.Equal("new@example.com", existing.Email);
    }

    [Fact]
    public void TestUpdateDomainShouldUpdateExistingEntity()
    {
        var existing = new DomainUser { Id = "1", Name = "Old", Email = "old@example.com" };
        var persistence = new PersistenceUser { Id = "1", Name = "New", Email = "new@example.com" };

        EntityMapper.UpdateDomain<PersistenceUser, DomainUser>(persistence, existing);

        Assert.Equal("1", existing.Id);
        Assert.Equal("New", existing.Name);
        Assert.Equal("new@example.com", existing.Email);
    }

    [Fact]
    public void TestUpdatePersistenceWithNullDomainShouldThrowException()
    {
        DomainUser domain = null!;
        var existing = new PersistenceUser { Id = "1", Name = "Existing" };

        Assert.Throws<ArgumentNullException>(() => EntityMapper.UpdatePersistence(domain, existing));
    }

    [Fact]
    public void TestUpdatePersistenceWithNullExistingShouldThrowException()
    {
        var domain = new DomainUser { Id = "1", Name = "New" };
        PersistenceUser existing = null!;

        Assert.Throws<ArgumentNullException>(() => EntityMapper.UpdatePersistence(domain, existing));
    }

    [Fact]
    public void TestUpdateDomainWithNullPersistenceShouldThrowException()
    {
        PersistenceUser persistence = null!;
        var existing = new DomainUser { Id = "1", Name = "Existing" };

        Assert.Throws<ArgumentNullException>(() => EntityMapper.UpdateDomain(persistence, existing));
    }

    [Fact]
    public void TestUpdateDomainWithNullExistingShouldThrowException()
    {
        var persistence = new PersistenceUser { Id = "1", Name = "New" };
        DomainUser existing = null!;

        Assert.Throws<ArgumentNullException>(() => EntityMapper.UpdateDomain(persistence, existing));
    }

    [Fact]
    public void TestUpdatePersistenceWithNullValueShouldClearProperty()
    {
        var existing = new PersistenceUser { Id = "1", Name = "John", Email = "john@example.com" };
        var domain = new DomainUser { Id = "1", Name = "John", Email = null! };

        EntityMapper.UpdatePersistence<DomainUser, PersistenceUser>(domain, existing);

        Assert.Equal("1", existing.Id);
        Assert.Equal("John", existing.Name);
        Assert.Null(existing.Email); // Email should be cleared (set to null)
    }

    [Fact]
    public void TestUpdateDomainWithNullValueShouldClearProperty()
    {
        var existing = new DomainUser { Id = "1", Name = "John", Email = "john@example.com" };
        var persistence = new PersistenceUser { Id = "1", Name = "John", Email = null! };

        EntityMapper.UpdateDomain<PersistenceUser, DomainUser>(persistence, existing);

        Assert.Equal("1", existing.Id);
        Assert.Equal("John", existing.Name);
        Assert.Null(existing.Email); // Email should be cleared (set to null)
    }

    private sealed class DomainUserWithNullable
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int? Age { get; set; }
    }

    private sealed class PersistenceUserWithNullable
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int? Age { get; set; }
    }

    [Fact]
    public void TestUpdatePersistenceWithNullablePropertiesShouldHandleNull()
    {
        var existing = new PersistenceUserWithNullable
        {
            Id = "1",
            Name = "John",
            Phone = "06000000",
            Age = 30
        };
        var domain = new DomainUserWithNullable
        {
            Id = "1",
            Name = "John",
            Phone = null,
            Age = null
        };

        EntityMapper.UpdatePersistence<DomainUserWithNullable, PersistenceUserWithNullable>(domain, existing);

        Assert.Equal("1", existing.Id);
        Assert.Equal("John", existing.Name);
        Assert.Null(existing.Phone); // Phone should be cleared
        Assert.Null(existing.Age); // Age should be cleared
    }

    [Fact]
    public void TestUpdateDomainWithNullablePropertiesShouldHandleNull()
    {
        var existing = new DomainUserWithNullable
        {
            Id = "1",
            Name = "John",
            Phone = "06000000",
            Age = 25
        };
        var persistence = new PersistenceUserWithNullable
        {
            Id = "1",
            Name = "John",
            Phone = null,
            Age = null
        };

        EntityMapper.UpdateDomain<PersistenceUserWithNullable, DomainUserWithNullable>(persistence, existing);

        Assert.Equal("1", existing.Id);
        Assert.Equal("John", existing.Name);
        Assert.Null(existing.Phone); // Phone should be cleared
        Assert.Null(existing.Age); // Age should be cleared
    }

    // Tests for recursive object updates
    private sealed class DomainCompany
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DomainAddress? Address { get; set; }
        public DomainManager? Manager { get; set; }
    }

    private sealed class DomainAddress
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
    }

    private sealed class DomainManager
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    private sealed class PersistenceCompanyDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public PersistenceAddressDto? Address { get; set; }
        public PersistenceManagerDto? Manager { get; set; }
    }

    private sealed class PersistenceAddressDto
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
    }

    private sealed class PersistenceManagerDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    [Fact]
    public void TestUpdatePersistenceWithNestedObjectShouldUpdateRecursively()
    {
        var existing = new PersistenceCompanyDto
        {
            Id = "company-1",
            Name = "Old Company",
            Address = new PersistenceAddressDto
            {
                Street = "Old Street",
                City = "Old City",
                ZipCode = "00000"
            },
            Manager = new PersistenceManagerDto
            {
                Id = "manager-1",
                Name = "Old Manager",
                Email = "old@example.com"
            }
        };

        var domain = new DomainCompany
        {
            Id = "company-1",
            Name = "New Company",
            Address = new DomainAddress
            {
                Street = "New Street",
                City = "New City",
                ZipCode = "75001"
            },
            Manager = new DomainManager
            {
                Id = "manager-1",
                Name = "New Manager",
                Email = "new@example.com"
            }
        };

        EntityMapper.UpdatePersistence<DomainCompany, PersistenceCompanyDto>(domain, existing);

        Assert.Equal("company-1", existing.Id);
        Assert.Equal("New Company", existing.Name);
        Assert.NotNull(existing.Address);
        Assert.Equal("New Street", existing.Address.Street);
        Assert.Equal("New City", existing.Address.City);
        Assert.Equal("75001", existing.Address.ZipCode);
        Assert.NotNull(existing.Manager);
        Assert.Equal("manager-1", existing.Manager.Id);
        Assert.Equal("New Manager", existing.Manager.Name);
        Assert.Equal("new@example.com", existing.Manager.Email);
    }

    [Fact]
    public void TestUpdateDomainWithNestedObjectShouldUpdateRecursively()
    {
        var existing = new DomainCompany
        {
            Id = "company-1",
            Name = "Old Company",
            Address = new DomainAddress
            {
                Street = "Old Street",
                City = "Old City",
                ZipCode = "00000"
            },
            Manager = new DomainManager
            {
                Id = "manager-1",
                Name = "Old Manager",
                Email = "old@example.com"
            }
        };

        var persistence = new PersistenceCompanyDto
        {
            Id = "company-1",
            Name = "New Company",
            Address = new PersistenceAddressDto
            {
                Street = "New Street",
                City = "New City",
                ZipCode = "75001"
            },
            Manager = new PersistenceManagerDto
            {
                Id = "manager-1",
                Name = "New Manager",
                Email = "new@example.com"
            }
        };

        EntityMapper.UpdateDomain<PersistenceCompanyDto, DomainCompany>(persistence, existing);

        Assert.Equal("company-1", existing.Id);
        Assert.Equal("New Company", existing.Name);
        Assert.NotNull(existing.Address);
        Assert.Equal("New Street", existing.Address.Street);
        Assert.Equal("New City", existing.Address.City);
        Assert.Equal("75001", existing.Address.ZipCode);
        Assert.NotNull(existing.Manager);
        Assert.Equal("manager-1", existing.Manager.Id);
        Assert.Equal("New Manager", existing.Manager.Name);
        Assert.Equal("new@example.com", existing.Manager.Email);
    }

    [Fact]
    public void TestUpdatePersistenceWithNullNestedObjectShouldCreateNewNestedObject()
    {
        var existing = new PersistenceCompanyDto
        {
            Id = "company-1",
            Name = "Company",
            Address = null,
            Manager = null
        };

        var domain = new DomainCompany
        {
            Id = "company-1",
            Name = "Company",
            Address = new DomainAddress
            {
                Street = "New Street",
                City = "New City",
                ZipCode = "75001"
            },
            Manager = new DomainManager
            {
                Id = "manager-1",
                Name = "New Manager",
                Email = "new@example.com"
            }
        };

        EntityMapper.UpdatePersistence<DomainCompany, PersistenceCompanyDto>(domain, existing);

        Assert.NotNull(existing.Address);
        Assert.Equal("New Street", existing.Address.Street);
        Assert.Equal("New City", existing.Address.City);
        Assert.NotNull(existing.Manager);
        Assert.Equal("New Manager", existing.Manager.Name);
    }

    [Fact]
    public void TestUpdateDomainWithNullNestedObjectShouldCreateNewNestedObject()
    {
        var existing = new DomainCompany
        {
            Id = "company-1",
            Name = "Company",
            Address = null,
            Manager = null
        };

        var persistence = new PersistenceCompanyDto
        {
            Id = "company-1",
            Name = "Company",
            Address = new PersistenceAddressDto
            {
                Street = "New Street",
                City = "New City",
                ZipCode = "75001"
            },
            Manager = new PersistenceManagerDto
            {
                Id = "manager-1",
                Name = "New Manager",
                Email = "new@example.com"
            }
        };

        EntityMapper.UpdateDomain<PersistenceCompanyDto, DomainCompany>(persistence, existing);

        Assert.NotNull(existing.Address);
        Assert.Equal("New Street", existing.Address.Street);
        Assert.Equal("New City", existing.Address.City);
        Assert.NotNull(existing.Manager);
        Assert.Equal("New Manager", existing.Manager.Name);
    }

    [Fact]
    public void TestUpdatePersistenceWithDeeplyNestedObjectsShouldUpdateAllLevels()
    {
        var existing = new PersistenceCompanyDto
        {
            Id = "company-1",
            Name = "Old Company",
            Address = new PersistenceAddressDto
            {
                Street = "Old Street",
                City = "Old City",
                ZipCode = "00000"
            }
        };

        var domain = new DomainCompany
        {
            Id = "company-1",
            Name = "New Company",
            Address = new DomainAddress
            {
                Street = "New Street",
                City = "New City",
                ZipCode = "75001"
            },
            Manager = new DomainManager
            {
                Id = "manager-1",
                Name = "New Manager",
                Email = "new@example.com"
            }
        };

        EntityMapper.UpdatePersistence<DomainCompany, PersistenceCompanyDto>(domain, existing);

        Assert.Equal("New Company", existing.Name);
        Assert.NotNull(existing.Address);
        Assert.Equal("New Street", existing.Address.Street);
        Assert.Equal("New City", existing.Address.City);
        Assert.Equal("75001", existing.Address.ZipCode);
        Assert.NotNull(existing.Manager);
        Assert.Equal("New Manager", existing.Manager.Name);
    }

    // Tests for very complex objects
    private sealed class DomainEnterprise
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DomainHeadquarters? Headquarters { get; set; }
        public List<DomainDepartment> Departments { get; set; } = new();
        public Dictionary<string, DomainEmployee> Employees { get; set; } = new();
        public DomainFinancialData? FinancialData { get; set; }
        public List<List<DomainProject>> Projects { get; set; } = new();
    }

    private sealed class DomainHeadquarters
    {
        public string Id { get; set; } = string.Empty;
        public DomainAddress Address { get; set; } = new();
        public DomainManager Director { get; set; } = new();
        public List<DomainBuilding> Buildings { get; set; } = new();
        public DomainSecuritySystem? SecuritySystem { get; set; }
    }

    private sealed class DomainBuilding
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Floors { get; set; }
        public DomainAddress Address { get; set; } = new();
        public List<DomainFloor> FloorDetails { get; set; } = new();
    }

    private sealed class DomainFloor
    {
        public int Number { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public List<DomainRoom> Rooms { get; set; } = new();
    }

    private sealed class DomainRoom
    {
        public string Number { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }

    private sealed class DomainSecuritySystem
    {
        public string SystemId { get; set; } = string.Empty;
        public List<DomainSecurityCamera> Cameras { get; set; } = new();
        public Dictionary<string, DomainAccessLevel> AccessLevels { get; set; } = new();
    }

    private sealed class DomainSecurityCamera
    {
        public string CameraId { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    private sealed class DomainAccessLevel
    {
        public string Level { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }

    private sealed class DomainDepartment
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DomainManager Manager { get; set; } = new();
        public List<DomainTeam> Teams { get; set; } = new();
        public DomainBudget? Budget { get; set; }
    }

    private sealed class DomainTeam
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<DomainEmployee> Members { get; set; } = new();
        public DomainProject? CurrentProject { get; set; }
    }

    private sealed class DomainEmployee
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DomainAddress? Address { get; set; }
        public DomainEmployee? Manager { get; set; }
        public List<DomainSkill> Skills { get; set; } = new();
    }

    private sealed class DomainSkill
    {
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public List<string> Certifications { get; set; } = new();
    }

    private sealed class DomainProject
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DomainProjectStatus Status { get; set; } = new();
        public List<DomainTask> Tasks { get; set; } = new();
        public Dictionary<string, DomainMilestone> Milestones { get; set; } = new();
    }

    private sealed class DomainProjectStatus
    {
        public string Current { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string> History { get; set; } = new();
    }

    private sealed class DomainTask
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DomainEmployee? Assignee { get; set; }
        public List<DomainSubTask> SubTasks { get; set; } = new();
    }

    private sealed class DomainSubTask
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }

    private sealed class DomainMilestone
    {
        public string Name { get; set; } = string.Empty;
        public DateTime TargetDate { get; set; }
        public bool IsReached { get; set; }
    }

    private sealed class DomainBudget
    {
        public decimal Total { get; set; }
        public decimal Spent { get; set; }
        public Dictionary<string, decimal> CategoryBudgets { get; set; } = new();
        public List<DomainExpense> Expenses { get; set; } = new();
    }

    private sealed class DomainExpense
    {
        public string Id { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    private sealed class DomainFinancialData
    {
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public List<DomainQuarterlyReport> QuarterlyReports { get; set; } = new();
        public Dictionary<int, DomainYearlySummary> YearlySummaries { get; set; } = new();
    }

    private sealed class DomainQuarterlyReport
    {
        public int Quarter { get; set; }
        public int Year { get; set; }
        public decimal Revenue { get; set; }
        public List<DomainRevenueSource> RevenueSources { get; set; } = new();
    }

    private sealed class DomainRevenueSource
    {
        public string Source { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    private sealed class DomainYearlySummary
    {
        public int Year { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public List<DomainQuarterlyReport> Quarters { get; set; } = new();
    }

    // Persistence DTOs
    private sealed class PersistenceEnterpriseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public PersistenceHeadquartersDto? Headquarters { get; set; }
        public List<PersistenceDepartmentDto> Departments { get; set; } = new();
        public Dictionary<string, PersistenceEmployeeDto> Employees { get; set; } = new();
        public PersistenceFinancialDataDto? FinancialData { get; set; }
        public List<List<PersistenceProjectDto>> Projects { get; set; } = new();
    }

    private sealed class PersistenceHeadquartersDto
    {
        public string Id { get; set; } = string.Empty;
        public PersistenceAddressDto Address { get; set; } = new();
        public PersistenceManagerDto Director { get; set; } = new();
        public List<PersistenceBuildingDto> Buildings { get; set; } = new();
        public PersistenceSecuritySystemDto? SecuritySystem { get; set; }
    }

    private sealed class PersistenceBuildingDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Floors { get; set; }
        public PersistenceAddressDto Address { get; set; } = new();
        public List<PersistenceFloorDto> FloorDetails { get; set; } = new();
    }

    private sealed class PersistenceFloorDto
    {
        public int Number { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public List<PersistenceRoomDto> Rooms { get; set; } = new();
    }

    private sealed class PersistenceRoomDto
    {
        public string Number { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }

    private sealed class PersistenceSecuritySystemDto
    {
        public string SystemId { get; set; } = string.Empty;
        public List<PersistenceSecurityCameraDto> Cameras { get; set; } = new();
        public Dictionary<string, PersistenceAccessLevelDto> AccessLevels { get; set; } = new();
    }

    private sealed class PersistenceSecurityCameraDto
    {
        public string CameraId { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    private sealed class PersistenceAccessLevelDto
    {
        public string Level { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }

    private sealed class PersistenceDepartmentDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public PersistenceManagerDto Manager { get; set; } = new();
        public List<PersistenceTeamDto> Teams { get; set; } = new();
        public PersistenceBudgetDto? Budget { get; set; }
    }

    private sealed class PersistenceTeamDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<PersistenceEmployeeDto> Members { get; set; } = new();
        public PersistenceProjectDto? CurrentProject { get; set; }
    }

    private sealed class PersistenceEmployeeDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public PersistenceAddressDto? Address { get; set; }
        public PersistenceEmployeeDto? Manager { get; set; }
        public List<PersistenceSkillDto> Skills { get; set; } = new();
    }

    private sealed class PersistenceSkillDto
    {
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public List<string> Certifications { get; set; } = new();
    }

    private sealed class PersistenceProjectDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public PersistenceProjectStatusDto Status { get; set; } = new();
        public List<PersistenceTaskDto> Tasks { get; set; } = new();
        public Dictionary<string, PersistenceMilestoneDto> Milestones { get; set; } = new();
    }

    private sealed class PersistenceProjectStatusDto
    {
        public string Current { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string> History { get; set; } = new();
    }

    private sealed class PersistenceTaskDto
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PersistenceEmployeeDto? Assignee { get; set; }
        public List<PersistenceSubTaskDto> SubTasks { get; set; } = new();
    }

    private sealed class PersistenceSubTaskDto
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }

    private sealed class PersistenceMilestoneDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime TargetDate { get; set; }
        public bool IsReached { get; set; }
    }

    private sealed class PersistenceBudgetDto
    {
        public decimal Total { get; set; }
        public decimal Spent { get; set; }
        public Dictionary<string, decimal> CategoryBudgets { get; set; } = new();
        public List<PersistenceExpenseDto> Expenses { get; set; } = new();
    }

    private sealed class PersistenceExpenseDto
    {
        public string Id { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    private sealed class PersistenceFinancialDataDto
    {
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public List<PersistenceQuarterlyReportDto> QuarterlyReports { get; set; } = new();
        public Dictionary<int, PersistenceYearlySummaryDto> YearlySummaries { get; set; } = new();
    }

    private sealed class PersistenceQuarterlyReportDto
    {
        public int Quarter { get; set; }
        public int Year { get; set; }
        public decimal Revenue { get; set; }
        public List<PersistenceRevenueSourceDto> RevenueSources { get; set; } = new();
    }

    private sealed class PersistenceRevenueSourceDto
    {
        public string Source { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    private sealed class PersistenceYearlySummaryDto
    {
        public int Year { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public List<PersistenceQuarterlyReportDto> Quarters { get; set; } = new();
    }

    [Fact]
    public void TestToPersistenceWithVeryComplexObjectShouldMapAllLevels()
    {
        var domain = new DomainEnterprise
        {
            Id = "enterprise-1",
            Name = "Tech Corp",
            Headquarters = new DomainHeadquarters
            {
                Id = "hq-1",
                Address = new DomainAddress { Street = "Main St", City = "Paris", ZipCode = "75001" },
                Director = new DomainManager { Id = "dir-1", Name = "CEO", Email = "ceo@tech.com" },
                Buildings = new List<DomainBuilding>
                {
                    new()
                    {
                        Id = "bld-1",
                        Name = "Tower A",
                        Floors = 10,
                        Address = new DomainAddress { Street = "Tower St", City = "Paris", ZipCode = "75002" },
                        FloorDetails = new List<DomainFloor>
                        {
                            new()
                            {
                                Number = 1,
                                Purpose = "Reception",
                                Rooms = new List<DomainRoom>
                                {
                                    new() { Number = "101", Type = "Office", Capacity = 4 },
                                    new() { Number = "102", Type = "Meeting", Capacity = 10 }
                                }
                            }
                        }
                    }
                },
                SecuritySystem = new DomainSecuritySystem
                {
                    SystemId = "sec-1",
                    Cameras = new List<DomainSecurityCamera>
                    {
                        new() { CameraId = "cam-1", Location = "Entrance", IsActive = true }
                    },
                    AccessLevels = new Dictionary<string, DomainAccessLevel>
                    {
                        ["admin"] = new DomainAccessLevel { Level = "Admin", Permissions = new List<string> { "all" } }
                    }
                }
            },
            Departments = new List<DomainDepartment>
            {
                new()
                {
                    Id = "dept-1",
                    Name = "Engineering",
                    Manager = new DomainManager { Id = "mgr-1", Name = "Eng Manager", Email = "eng@tech.com" },
                    Teams = new List<DomainTeam>
                    {
                        new()
                        {
                            Id = "team-1",
                            Name = "Backend Team",
                            Members = new List<DomainEmployee>
                            {
                                new()
                                {
                                    Id = "emp-1",
                                    Name = "John Doe",
                                    Email = "john@tech.com",
                                    Skills = new List<DomainSkill>
                                    {
                                        new() { Name = "C#", Level = 8, Certifications = new List<string> { "MCSD" } }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            Employees = new Dictionary<string, DomainEmployee>
            {
                ["emp-1"] = new DomainEmployee
                {
                    Id = "emp-1",
                    Name = "John Doe",
                    Email = "john@tech.com"
                }
            },
            FinancialData = new DomainFinancialData
            {
                Revenue = 1000000m,
                Expenses = 500000m,
                QuarterlyReports = new List<DomainQuarterlyReport>
                {
                    new()
                    {
                        Quarter = 1,
                        Year = 2024,
                        Revenue = 250000m,
                        RevenueSources = new List<DomainRevenueSource>
                        {
                            new() { Source = "Products", Amount = 200000m },
                            new() { Source = "Services", Amount = 50000m }
                        }
                    }
                },
                YearlySummaries = new Dictionary<int, DomainYearlySummary>
                {
                    [2024] = new DomainYearlySummary
                    {
                        Year = 2024,
                        TotalRevenue = 1000000m,
                        TotalExpenses = 500000m
                    }
                }
            },
            Projects = new List<List<DomainProject>>
            {
                new()
                {
                    new DomainProject
                    {
                        Id = "proj-1",
                        Name = "Project Alpha",
                        Status = new DomainProjectStatus
                        {
                            Current = "In Progress",
                            StartDate = new DateTime(2024, 1, 1),
                            History = new List<string> { "Started", "In Progress" }
                        },
                        Tasks = new List<DomainTask>
                        {
                            new()
                            {
                                Id = "task-1",
                                Description = "Implement feature",
                                SubTasks = new List<DomainSubTask>
                                {
                                    new() { Id = "sub-1", Description = "Design", IsCompleted = true }
                                }
                            }
                        },
                        Milestones = new Dictionary<string, DomainMilestone>
                        {
                            ["m1"] = new DomainMilestone { Name = "Phase 1", TargetDate = new DateTime(2024, 3, 1), IsReached = true }
                        }
                    }
                }
            }
        };

        var persistence = EntityMapper.ToPersistence<DomainEnterprise, PersistenceEnterpriseDto>(domain);

        Assert.Equal("enterprise-1", persistence.Id);
        Assert.Equal("Tech Corp", persistence.Name);
        Assert.NotNull(persistence.Headquarters);
        Assert.Equal("hq-1", persistence.Headquarters!.Id);
        Assert.Single(persistence.Headquarters.Buildings);
        Assert.Equal("Tower A", persistence.Headquarters.Buildings[0].Name);
        Assert.Single(persistence.Headquarters.Buildings[0].FloorDetails);
        Assert.Equal(2, persistence.Headquarters.Buildings[0].FloorDetails[0].Rooms.Count);
        Assert.NotNull(persistence.Headquarters.SecuritySystem);
        Assert.Single(persistence.Headquarters.SecuritySystem!.Cameras);
        Assert.Single(persistence.Departments);
        Assert.Single(persistence.Departments[0].Teams);
        Assert.Single(persistence.Departments[0].Teams[0].Members);
        Assert.Single(persistence.Employees);
        Assert.NotNull(persistence.FinancialData);
        Assert.Single(persistence.FinancialData!.QuarterlyReports);
        Assert.Equal(2, persistence.FinancialData.QuarterlyReports[0].RevenueSources.Count);
        Assert.Single(persistence.FinancialData.YearlySummaries);
        Assert.Single(persistence.Projects);
        Assert.Single(persistence.Projects[0]);
        Assert.Equal("Project Alpha", persistence.Projects[0][0].Name);
        Assert.Single(persistence.Projects[0][0].Tasks);
        Assert.Single(persistence.Projects[0][0].Tasks[0].SubTasks);
        Assert.Single(persistence.Projects[0][0].Milestones);
    }

    [Fact]
    public void TestUpdatePersistenceWithVeryComplexObjectShouldUpdateAllLevels()
    {
        var existing = new PersistenceEnterpriseDto
        {
            Id = "enterprise-1",
            Name = "Old Corp",
            Headquarters = new PersistenceHeadquartersDto
            {
                Id = "hq-1",
                Address = new PersistenceAddressDto { Street = "Old St", City = "Lyon", ZipCode = "69001" },
                Director = new PersistenceManagerDto { Id = "dir-1", Name = "Old CEO", Email = "old@tech.com" }
            }
        };

        var domain = new DomainEnterprise
        {
            Id = "enterprise-1",
            Name = "New Tech Corp",
            Headquarters = new DomainHeadquarters
            {
                Id = "hq-1",
                Address = new DomainAddress { Street = "New Main St", City = "Paris", ZipCode = "75001" },
                Director = new DomainManager { Id = "dir-1", Name = "New CEO", Email = "newceo@tech.com" },
                Buildings = new List<DomainBuilding>
                {
                    new()
                    {
                        Id = "bld-1",
                        Name = "Tower B",
                        Floors = 15,
                        FloorDetails = new List<DomainFloor>
                        {
                            new()
                            {
                                Number = 1,
                                Purpose = "Lobby",
                                Rooms = new List<DomainRoom>
                                {
                                    new() { Number = "201", Type = "Office", Capacity = 6 }
                                }
                            }
                        }
                    }
                }
            },
            FinancialData = new DomainFinancialData
            {
                Revenue = 2000000m,
                Expenses = 1000000m
            }
        };

        EntityMapper.UpdatePersistence<DomainEnterprise, PersistenceEnterpriseDto>(domain, existing);

        Assert.Equal("enterprise-1", existing.Id);
        Assert.Equal("New Tech Corp", existing.Name);
        Assert.NotNull(existing.Headquarters);
        Assert.Equal("New Main St", existing.Headquarters!.Address.Street);
        Assert.Equal("Paris", existing.Headquarters.Address.City);
        Assert.Equal("New CEO", existing.Headquarters.Director.Name);
        Assert.Single(existing.Headquarters.Buildings);
        Assert.Equal("Tower B", existing.Headquarters.Buildings[0].Name);
        Assert.Equal(15, existing.Headquarters.Buildings[0].Floors);
        Assert.NotNull(existing.FinancialData);
        Assert.Equal(2000000m, existing.FinancialData!.Revenue);
    }

    [Fact]
    public void TestToDomainWithVeryComplexObjectShouldMapAllLevels()
    {
        var persistence = new PersistenceEnterpriseDto
        {
            Id = "enterprise-1",
            Name = "Tech Corp",
            Headquarters = new PersistenceHeadquartersDto
            {
                Id = "hq-1",
                Address = new PersistenceAddressDto { Street = "Main St", City = "Paris", ZipCode = "75001" },
                Director = new PersistenceManagerDto { Id = "dir-1", Name = "CEO", Email = "ceo@tech.com" },
                Buildings = new List<PersistenceBuildingDto>
                {
                    new()
                    {
                        Id = "bld-1",
                        Name = "Tower A",
                        Floors = 10,
                        FloorDetails = new List<PersistenceFloorDto>
                        {
                            new()
                            {
                                Number = 1,
                                Purpose = "Reception",
                                Rooms = new List<PersistenceRoomDto>
                                {
                                    new() { Number = "101", Type = "Office", Capacity = 4 }
                                }
                            }
                        }
                    }
                }
            },
            Projects = new List<List<PersistenceProjectDto>>
            {
                new()
                {
                    new PersistenceProjectDto
                    {
                        Id = "proj-1",
                        Name = "Project Beta",
                        Status = new PersistenceProjectStatusDto { Current = "Planning" },
                        Tasks = new List<PersistenceTaskDto>
                        {
                            new()
                            {
                                Id = "task-1",
                                Description = "Plan features",
                                SubTasks = new List<PersistenceSubTaskDto>
                                {
                                    new() { Id = "sub-1", Description = "Research", IsCompleted = false }
                                }
                            }
                        }
                    }
                }
            }
        };

        var domain = EntityMapper.ToDomain<PersistenceEnterpriseDto, DomainEnterprise>(persistence);

        Assert.Equal("enterprise-1", domain.Id);
        Assert.Equal("Tech Corp", domain.Name);
        Assert.NotNull(domain.Headquarters);
        Assert.Equal("hq-1", domain.Headquarters!.Id);
        Assert.Single(domain.Headquarters.Buildings);
        Assert.Equal("Tower A", domain.Headquarters.Buildings[0].Name);
        Assert.Single(domain.Headquarters.Buildings[0].FloorDetails);
        Assert.Single(domain.Headquarters.Buildings[0].FloorDetails[0].Rooms);
        Assert.Single(domain.Projects);
        Assert.Single(domain.Projects[0]);
        Assert.Equal("Project Beta", domain.Projects[0][0].Name);
        Assert.Single(domain.Projects[0][0].Tasks);
        Assert.Single(domain.Projects[0][0].Tasks[0].SubTasks);
    }
}
