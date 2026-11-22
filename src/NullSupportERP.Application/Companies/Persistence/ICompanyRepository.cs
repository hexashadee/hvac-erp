using NullSupportERP.Application.Common.Persistence;
using NullSupportERP.Domain.Companies;

namespace NullSupportERP.Application.Companies.Persistence;

public interface ICompanyRepository : IRepository<Company>
{
    Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default);
}
