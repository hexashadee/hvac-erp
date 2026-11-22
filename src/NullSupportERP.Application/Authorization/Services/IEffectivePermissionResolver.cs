namespace NullSupportERP.Application.Authorization.Services;

public interface IEffectivePermissionResolver
{
    Task<IReadOnlySet<string>> ResolveAsync(
        Guid companyId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
