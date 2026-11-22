using NullSupportERP.Application.Common.Persistence;
using NullSupportERP.Domain.Users;

namespace NullSupportERP.Application.Users.Persistence;

public interface ICompanyMembershipRepository
    : IRepository<CompanyMembership>
{
    Task<CompanyMembership?> GetByCompanyAndUserAsync(
        Guid companyId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CompanyMembership>> GetByCompanyIdAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CompanyMembership>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
