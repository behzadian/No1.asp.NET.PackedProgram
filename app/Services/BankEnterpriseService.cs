using No1.FaraBank.Api.Contracts;
using No1.FaraBank.Api.Data.Repos;
using No1.FaraBank.Api.Data.Shared;
using No1.FaraBank.Api.Exceptions;
using No1.FaraBank.Api.Models;
using No1.FaraBank.Api.Portables.Manager;

namespace No1.FaraBank.Api.Services;

public class BankEnterpriseService(BankEnterpriseRepo repo, IClockContract clock, IFileContract files) : IBankEnterpriseContract
{
	Task<Paged<BankEnterpriseManagerPortable>> IBankEnterpriseContract.PagedAsync(Pagination<BankEnterpriseEntity> pagination, CancellationToken cancellationToken) {
		throw new NotImplementedException();
	}

	async Task<BankEnterpriseManagerPortable> IBankEnterpriseContract.PersistAsync(BankEnterpriseManagerInput input, CancellationToken cancellationToken) {
		BankEnterpriseEntity bank;
		if (input.GlobalID.HasValue) {
			bank = (await repo.GetBankByGlobalIdAsync(input.GlobalID.Value)) ?? throw new EntityNotFoundException<BankEnterpriseEntity>(input.GlobalID.Value);
		} else {
			bank = new BankEnterpriseEntity() { Created = clock.Now("BankOfficialEntity.Created") };
		}

		bank.Updated = clock.Now("BankOfficialEntity.Updated");
		bank.Logo = await files.ProcessAsync<BankEnterpriseEntity>(bank.Logo, input.Logo, bank.GlobalID, "logo", cancellationToken);
		bank.Name = input.Name;

		await repo.PersistAsync(bank, cancellationToken);

		return BankEnterpriseManagerPortable.New(bank);
	}
}