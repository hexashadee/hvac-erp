using NullSupportERP.Application.Common.Persistence;
using NullSupportERP.Domain.Authorization;

namespace NullSupportERP.Application.Authorization.Persistence;

public interface IPermissionDefinitionRepository
    : IRepository<PermissionDefinition>
{
    Task<PermissionDefinition?> GetByKeyAsync(
        string key,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PermissionDefinition>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
