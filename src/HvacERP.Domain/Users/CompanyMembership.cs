using HvacERP.Domain.Common.Base;

namespace HvacERP.Domain.Users;

public sealed class CompanyMembership : AggregateRoot
{
    private readonly List<Guid> _roleIds = [];

    public Guid CompanyId { get; private set; }

    public Guid UserId { get; private set; }

    public string? EmployeeNumber { get; private set; }

    public string? JobTitle { get; private set; }

    public EmploymentStatus EmploymentStatus { get; private set; }

    public bool IsCompanyOwner { get; private set; }

    public DateOnly? HireDate { get; private set; }

    public DateOnly? TerminationDate { get; private set; }

    public IReadOnlyCollection<Guid> RoleIds =>
        _roleIds.AsReadOnly();

    public bool IsActive =>
        EmploymentStatus != EmploymentStatus.Terminated;

    private CompanyMembership(
        Guid id,
        Guid companyId,
        Guid userId,
        string? employeeNumber,
        string? jobTitle,
        bool isCompanyOwner,
        DateOnly? hireDate,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
        : base(id, createdAtUtc, createdByUserId)
    {
        CompanyId = companyId;
        UserId = userId;
        EmployeeNumber = employeeNumber;
        JobTitle = jobTitle;
        EmploymentStatus = EmploymentStatus.Active;
        IsCompanyOwner = isCompanyOwner;
        HireDate = hireDate;
    }

    public static CompanyMembership Create(
        Guid id,
        Guid companyId,
        Guid userId,
        string? employeeNumber,
        string? jobTitle,
        bool isCompanyOwner,
        DateOnly? hireDate,
        DateTimeOffset createdAtUtc,
        Guid createdByUserId)
    {
        if (companyId == Guid.Empty)
        {
            throw new ArgumentException(
                "Company ID cannot be empty.",
                nameof(companyId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));
        }

        string? normalizedEmployeeNumber =
            NormalizeOptionalValue(employeeNumber);

        string? normalizedJobTitle =
            NormalizeOptionalValue(jobTitle);

        ValidateEmployeeNumber(normalizedEmployeeNumber);
        ValidateJobTitle(normalizedJobTitle);

        return new CompanyMembership(
            id,
            companyId,
            userId,
            normalizedEmployeeNumber,
            normalizedJobTitle,
            isCompanyOwner,
            hireDate,
            createdAtUtc,
            createdByUserId);
    }

    public void UpdateEmploymentDetails(
        string? employeeNumber,
        string? jobTitle,
        DateOnly? hireDate,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotTerminated();

        string? normalizedEmployeeNumber =
            NormalizeOptionalValue(employeeNumber);

        string? normalizedJobTitle =
            NormalizeOptionalValue(jobTitle);

        ValidateEmployeeNumber(normalizedEmployeeNumber);
        ValidateJobTitle(normalizedJobTitle);

        EmployeeNumber = normalizedEmployeeNumber;
        JobTitle = normalizedJobTitle;
        HireDate = hireDate;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void PlaceOnLeave(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotTerminated();

        EmploymentStatus = EmploymentStatus.LeaveOfAbsence;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void ReturnFromLeave(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotTerminated();

        EmploymentStatus = EmploymentStatus.Active;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void Terminate(
        DateOnly terminationDate,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        if (IsCompanyOwner)
        {
            throw new InvalidOperationException(
                "A company owner cannot be terminated.");
        }

        if (EmploymentStatus == EmploymentStatus.Terminated)
        {
            throw new InvalidOperationException(
                "The membership is already terminated.");
        }

        if (HireDate.HasValue &&
            terminationDate < HireDate.Value)
        {
            throw new ArgumentException(
                "Termination date cannot be before the hire date.",
                nameof(terminationDate));
        }

        EmploymentStatus = EmploymentStatus.Terminated;
        TerminationDate = terminationDate;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void TransferOwnership(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotTerminated();

        IsCompanyOwner = true;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void RemoveOwnership(
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotTerminated();

        IsCompanyOwner = false;

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void AssignRole(
        Guid roleId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotTerminated();

        if (roleId == Guid.Empty)
        {
            throw new ArgumentException(
                "Role ID cannot be empty.",
                nameof(roleId));
        }

        if (_roleIds.Contains(roleId))
        {
            return;
        }

        _roleIds.Add(roleId);

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public void RemoveRole(
        Guid roleId,
        DateTimeOffset modifiedAtUtc,
        Guid modifiedByUserId)
    {
        EnsureNotTerminated();

        if (roleId == Guid.Empty)
        {
            throw new ArgumentException(
                "Role ID cannot be empty.",
                nameof(roleId));
        }

        bool removed = _roleIds.Remove(roleId);

        if (!removed)
        {
            return;
        }

        MarkModified(modifiedAtUtc, modifiedByUserId);
    }

    public bool HasRole(Guid roleId)
    {
        if (roleId == Guid.Empty)
        {
            return false;
        }

        return _roleIds.Contains(roleId);
    }

    private void EnsureNotTerminated()
    {
        if (EmploymentStatus == EmploymentStatus.Terminated)
        {
            throw new InvalidOperationException(
                "A terminated membership cannot be modified.");
        }
    }

    private static string? NormalizeOptionalValue(string? value)
    {
        string? normalizedValue = value?.Trim();

        return string.IsNullOrWhiteSpace(normalizedValue)
            ? null
            : normalizedValue;
    }

    private static void ValidateEmployeeNumber(string? employeeNumber)
    {
        if (employeeNumber?.Length > 50)
        {
            throw new ArgumentException(
                "Employee number cannot exceed 50 characters.",
                nameof(employeeNumber));
        }
    }

    private static void ValidateJobTitle(string? jobTitle)
    {
        if (jobTitle?.Length > 100)
        {
            throw new ArgumentException(
                "Job title cannot exceed 100 characters.",
                nameof(jobTitle));
        }
    }
}

