using HvacERP.Application.Common.Persistence;
using HvacERP.Domain.Authorization;

namespace HvacERP.Application.Authorization.Persistence;

public interface IPermissionDefinitionRepository
    : IRepository<PermissionDefinition>
{
    Task<PermissionDefinition?> GetByKeyAsync(
        string key,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PermissionDefinition>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
