using No1.FaraBank.Api.Data.Shared;
using No1.FaraBank.Api.Models;
using No1.FaraBank.Api.Portables.Owner;

namespace No1.FaraBank.Api.Contracts;

public interface IBankAccountContract
{
	Task<Paged<BankAccountOwnerPortable>> GetAccountsAsync(Pagination<BankAccountEntity> pagination, CancellationToken cancellationToken);

	Task<BankAccountOwnerPortable> PersistAsync(Guid ownerId, BankAccountOwnerInput input, CancellationToken cancellationToken = default);
}