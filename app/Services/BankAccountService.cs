using No1.FaraBank.Api.Contracts;
using No1.FaraBank.Api.Data.Repos;
using No1.FaraBank.Api.Data.Shared;
using No1.FaraBank.Api.Exceptions;
using No1.FaraBank.Api.Models;
using No1.FaraBank.Api.Portables.Owner;

namespace No1.FaraBank.Api.Services;

public class BankAccountService(BankAccountRepo repo, IClockContract cloak, BankEnterpriseRepo bankOfficialRepo) : IBankAccountContract
{
	Task<Paged<BankAccountOwnerPortable>> IBankAccountContract.GetAccountsAsync(Pagination<BankAccountEntity> pagination, CancellationToken cancellationToken) {
		throw new NotImplementedException();
	}

	async Task<BankAccountOwnerPortable> IBankAccountContract.PersistAsync(Guid ownerId, BankAccountOwnerInput input, CancellationToken cancellationToken) {
		ArgumentNullException.ThrowIfNull(input);
		var bankEnterprise = bankOfficialRepo.GetBankByGlobalIdAsync(input.BankEnterpriseGlobalID);

		BankAccountEntity? bankAccount;
		if (input.GlobalID.HasValue) {
			bankAccount = repo.FindByGlobalIdAndOwnerID(input.GlobalID, ownerId) ?? throw new ArgumentException($"No BankAccountEntity with GlobalID: {input.GlobalID} found for user: {ownerId}");
		} else {
			bankAccount = repo.FindByOwnerIdAndIban(ownerId, input.IBAN);
			if (bankAccount != null) {
				throw new ArgumentException($"Already there is a BankAccountEntity with IBAN: {input.IBAN} for user: {ownerId}");
			}
		}

		bankAccount ??= new BankAccountEntity() { OwnerID = ownerId, Created = cloak.Now("BankAccountCreate") };
		bankAccount.Currency = input.Currency;
		bankAccount.Updated = cloak.Now("BankAccountUpdate");
		bankAccount.CustomerID = input.CustomerID;
		bankAccount.Number = input.Number;
		bankAccount.IBAN = input.IBAN;
		bankAccount.Name = input.Name;
		bankAccount.BankEnterprise = (await bankEnterprise) ?? throw new EntityNotFoundException<BankEnterpriseEntity>(input.BankEnterpriseGlobalID);
		return BankAccountOwnerPortable.New(bankAccount);
	}
}