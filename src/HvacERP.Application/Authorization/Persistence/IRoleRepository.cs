using HvacERP.Application.Common.Persistence;
using HvacERP.Domain.Authorization;

namespace HvacERP.Application.Authorization.Persistence;

public interface IRoleRepository : IRepository<Role>
{
    Task<IReadOnlyList<Role>> GetByCompanyIdAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        Guid companyId,
        string name,
        CancellationToken cancellationToken = default);
}

