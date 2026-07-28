using HvacERP.Application.Common.Persistence;
using HvacERP.Domain.Companies;

namespace HvacERP.Application.Companies.Persistence;

public interface ICompanyRepository : IRepository<Company>
{
    Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default);
}
