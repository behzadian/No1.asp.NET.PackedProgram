using JetBrains.Annotations;
using No1.FaraBank.Api.Data.Shared;
using No1.FaraBank.Api.Models;

namespace No1.FaraBank.Api.Data.Repos;

[UsedImplicitly]
public class BankAccountRepo(NHibernate.ISession session)
{
	public IList<BankAccountEntity> Paged(Pagination<BankAccountEntity> pagination) {
		return [.. session
				.Query<BankAccountEntity>()
				.OrderBy(x => x.ID)
				.Skip(pagination.StartRowIndex)
				.Take(pagination.Size)]
			;
	}

	public BankAccountEntity Persist(BankAccountEntity bankAccount) {
		session.SaveOrUpdate(bankAccount);
		return bankAccount;
	}

	internal BankAccountEntity? FindByGlobalIdAndOwnerID(Guid? iD, Guid ownerId) {
		return session
			.Query<BankAccountEntity>()
			.FirstOrDefault(x => x.GlobalID == iD && x.OwnerID == ownerId);
	}

	internal BankAccountEntity? FindByOwnerIdAndIban(Guid ownerId, string iban) {
		return session
			.Query<BankAccountEntity>()
			.FirstOrDefault(x => x.OwnerID == ownerId && x.IBAN == iban);
	}
}