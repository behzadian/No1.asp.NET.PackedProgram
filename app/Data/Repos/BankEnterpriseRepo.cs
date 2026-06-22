using JetBrains.Annotations;
using NHibernate.Linq;
using No1.FaraBank.Api.Models;

namespace No1.FaraBank.Api.Data.Repos;

[UsedImplicitly]
public class BankEnterpriseRepo(NHibernate.ISession session)
{
	internal async Task<BankEnterpriseEntity?> GetBankByGlobalIdAsync(Guid globalId) {
		return await session.Query<BankEnterpriseEntity>().Where(x => x.Deleted == null && x.GlobalID == globalId).FirstOrDefaultAsync();
	}

	internal async Task PersistAsync(BankEnterpriseEntity bank, CancellationToken cancellationToken = default) {
		await session.SaveOrUpdateAsync(bank, cancellationToken);
	}
}