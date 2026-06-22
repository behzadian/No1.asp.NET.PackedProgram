using No1.FaraBank.Api.Data.Shared;
using No1.FaraBank.Api.Models;
using No1.FaraBank.Api.Portables.Manager;

namespace No1.FaraBank.Api.Contracts;

public interface IBankEnterpriseContract
{
	Task<Paged<BankEnterpriseManagerPortable>> PagedAsync(Pagination<BankEnterpriseEntity> pagination, CancellationToken cancellationToken = default);

	Task<BankEnterpriseManagerPortable> PersistAsync(BankEnterpriseManagerInput input, CancellationToken cancellationToken = default);
}