using NullSupportERP.Domain.Companies;

namespace NullSupportERP.Domain.Tests.Companies;

public sealed partial class CompanyTests
{
    private static readonly Guid CompanyId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static readonly Guid UserId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 7, 28, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidValues_CreatesActiveCompany()
    {
        Company company = Company.Create(
            CompanyId,
            "Acme Heating",
            "acme-heating",
            CreatedAtUtc,
            UserId);

        Assert.Equal(CompanyId, company.Id);
        Assert.Equal("Acme Heating", company.Name);
        Assert.Equal("acme-heating", company.Slug);
        Assert.True(company.IsActive);
        Assert.Equal(CreatedAtUtc, company.CreatedAtUtc);
        Assert.Equal(UserId, company.CreatedByUserId);
    }

    [Fact]
    public void Create_TrimsNameAndNormalizesSlug()
    {
        Company company = Company.Create(
            CompanyId,
            "  Acme Heating  ",
            "  ACME-HEATING  ",
            CreatedAtUtc,
            UserId);

        Assert.Equal("Acme Heating", company.Name);
        Assert.Equal("acme-heating", company.Slug);
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => Company.Create(
                CompanyId,
                " ",
                "acme-heating",
                CreatedAtUtc,
                UserId));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Rename_WithValidName_UpdatesNameAndAuditInformation()
    {
        Company company = Company.Create(
            CompanyId,
            "Acme Heating",
            "acme-heating",
            CreatedAtUtc,
            UserId);

        Guid modifiedByUserId =
            Guid.Parse("33333333-3333-3333-3333-333333333333");

        DateTimeOffset modifiedAtUtc =
            new(2026, 7, 29, 15, 30, 0, TimeSpan.Zero);

        company.Rename(
            "Acme Heating and Cooling",
            modifiedAtUtc,
            modifiedByUserId);

        Assert.Equal("Acme Heating and Cooling", company.Name);
        Assert.Equal(modifiedAtUtc, company.LastModifiedAtUtc);
        Assert.Equal(modifiedByUserId, company.LastModifiedByUserId);
    }
}

public sealed partial class CompanyTests
{
    [Fact]
    public void Create_WithInvalidSlugCharacters_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => Company.Create(
                CompanyId,
                "Acme Heating",
                "acme_heating",
                CreatedAtUtc,
                UserId));

        Assert.Equal("slug", exception.ParamName);
    }

    [Fact]
    public void Create_WithNameLongerThan200Characters_ThrowsArgumentException()
    {
        string name = new('A', 201);

        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => Company.Create(
                CompanyId,
                name,
                "acme-heating",
                CreatedAtUtc,
                UserId));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Deactivate_SetsCompanyInactiveAndUpdatesAuditInformation()
    {
        Company company = Company.Create(
            CompanyId,
            "Acme Heating",
            "acme-heating",
            CreatedAtUtc,
            UserId);

        Guid modifiedByUserId =
            Guid.Parse("33333333-3333-3333-3333-333333333333");

        DateTimeOffset modifiedAtUtc =
            new(2026, 7, 29, 15, 30, 0, TimeSpan.Zero);

        company.Deactivate(modifiedAtUtc, modifiedByUserId);

        Assert.False(company.IsActive);
        Assert.Equal(modifiedAtUtc, company.LastModifiedAtUtc);
        Assert.Equal(modifiedByUserId, company.LastModifiedByUserId);
    }

    [Fact]
    public void Reactivate_SetsCompanyActiveAndUpdatesAuditInformation()
    {
        Company company = Company.Create(
            CompanyId,
            "Acme Heating",
            "acme-heating",
            CreatedAtUtc,
            UserId);

        Guid modifiedByUserId =
            Guid.Parse("33333333-3333-3333-3333-333333333333");

        DateTimeOffset deactivatedAtUtc =
            new(2026, 7, 29, 15, 30, 0, TimeSpan.Zero);

        DateTimeOffset reactivatedAtUtc =
            new(2026, 7, 30, 10, 0, 0, TimeSpan.Zero);

        company.Deactivate(deactivatedAtUtc, modifiedByUserId);
        company.Reactivate(reactivatedAtUtc, modifiedByUserId);

        Assert.True(company.IsActive);
        Assert.Equal(reactivatedAtUtc, company.LastModifiedAtUtc);
        Assert.Equal(modifiedByUserId, company.LastModifiedByUserId);
    }

    [Fact]
    public void Rename_WithEmptyModifierUserId_ThrowsArgumentException()
    {
        Company company = Company.Create(
            CompanyId,
            "Acme Heating",
            "acme-heating",
            CreatedAtUtc,
            UserId);

        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => company.Rename(
                "Acme Heating and Cooling",
                CreatedAtUtc.AddDays(1),
                Guid.Empty));

        Assert.Equal("userId", exception.ParamName);
    }
}
