using HvacERP.Domain.Companies;

namespace HvacERP.Domain.Tests.Companies;

public sealed class BranchTests
{
    private static readonly Guid BranchId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static readonly Guid CompanyId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static readonly Guid UserId =
        Guid.Parse("33333333-3333-3333-3333-333333333333");

    private static readonly DateTimeOffset CreatedAtUtc =
        new(2026, 7, 28, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidValues_CreatesActiveBranch()
    {
        Branch branch = Branch.Create(
            BranchId,
            CompanyId,
            "Main Office",
            "MAIN",
            true,
            CreatedAtUtc,
            UserId);

        Assert.Equal(BranchId, branch.Id);
        Assert.Equal(CompanyId, branch.CompanyId);
        Assert.Equal("Main Office", branch.Name);
        Assert.Equal("MAIN", branch.Code);
        Assert.True(branch.IsPrimary);
        Assert.True(branch.IsActive);
        Assert.Equal(CreatedAtUtc, branch.CreatedAtUtc);
        Assert.Equal(UserId, branch.CreatedByUserId);
    }

    [Fact]
    public void Create_TrimsNameAndNormalizesCode()
    {
        Branch branch = Branch.Create(
            BranchId,
            CompanyId,
            "  Main Office  ",
            "  main-01  ",
            false,
            CreatedAtUtc,
            UserId);

        Assert.Equal("Main Office", branch.Name);
        Assert.Equal("MAIN-01", branch.Code);
    }

    [Fact]
    public void Create_WithEmptyCompanyId_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => Branch.Create(
                BranchId,
                Guid.Empty,
                "Main Office",
                "MAIN",
                false,
                CreatedAtUtc,
                UserId));

        Assert.Equal("companyId", exception.ParamName);
    }

    [Fact]
    public void Create_WithInvalidCodeCharacters_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => Branch.Create(
                BranchId,
                CompanyId,
                "Main Office",
                "MAIN_01",
                false,
                CreatedAtUtc,
                UserId));

        Assert.Equal("code", exception.ParamName);
    }

    [Fact]
    public void Rename_WithValidName_UpdatesNameAndAuditInformation()
    {
        Branch branch = CreateBranch();

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddDays(1);

        branch.Rename(
            "North Office",
            modifiedAtUtc,
            UserId);

        Assert.Equal("North Office", branch.Name);
        Assert.Equal(modifiedAtUtc, branch.LastModifiedAtUtc);
        Assert.Equal(UserId, branch.LastModifiedByUserId);
    }

    [Fact]
    public void ChangeCode_NormalizesCodeAndUpdatesAuditInformation()
    {
        Branch branch = CreateBranch();

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddDays(1);

        branch.ChangeCode(
            "  north-01  ",
            modifiedAtUtc,
            UserId);

        Assert.Equal("NORTH-01", branch.Code);
        Assert.Equal(modifiedAtUtc, branch.LastModifiedAtUtc);
        Assert.Equal(UserId, branch.LastModifiedByUserId);
    }

    [Fact]
    public void MakePrimary_SetsPrimaryAndUpdatesAuditInformation()
    {
        Branch branch = CreateBranch();

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddDays(1);

        branch.MakePrimary(
            modifiedAtUtc,
            UserId);

        Assert.True(branch.IsPrimary);
        Assert.Equal(modifiedAtUtc, branch.LastModifiedAtUtc);
        Assert.Equal(UserId, branch.LastModifiedByUserId);
    }

    [Fact]
    public void Deactivate_WhenBranchIsNotPrimary_DeactivatesBranch()
    {
        Branch branch = CreateBranch();

        DateTimeOffset modifiedAtUtc =
            CreatedAtUtc.AddDays(1);

        branch.Deactivate(
            modifiedAtUtc,
            UserId);

        Assert.False(branch.IsActive);
        Assert.Equal(modifiedAtUtc, branch.LastModifiedAtUtc);
        Assert.Equal(UserId, branch.LastModifiedByUserId);
    }

    [Fact]
    public void Deactivate_WhenBranchIsPrimary_ThrowsInvalidOperationException()
    {
        Branch branch = Branch.Create(
            BranchId,
            CompanyId,
            "Main Office",
            "MAIN",
            true,
            CreatedAtUtc,
            UserId);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => branch.Deactivate(
                    CreatedAtUtc.AddDays(1),
                    UserId));

        Assert.Equal(
            "A primary branch cannot be deactivated.",
            exception.Message);

        Assert.True(branch.IsActive);
    }

    private static Branch CreateBranch()
    {
        return Branch.Create(
            BranchId,
            CompanyId,
            "Main Office",
            "MAIN",
            false,
            CreatedAtUtc,
            UserId);
    }
}
