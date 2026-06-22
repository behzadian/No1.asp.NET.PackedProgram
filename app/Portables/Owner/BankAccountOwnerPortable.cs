using No1.FaraBank.Api.Enums;
using No1.FaraBank.Api.Models;
using NodaTime;

namespace No1.FaraBank.Api.Portables.Owner;

public record BankAccountOwnerPortable(
	Guid GlobalID,
	bool SyncForced,
	Instant Created,
	Instant Updated,
	Currency Currency,
	string? Name,
	decimal BalanceSigned,
	decimal BalanceAbsolute,
	Instant? BalanceLastUpdate,
	string Number,
	string IBAN,
	string? CustomerID,
	string? Comment
)
{
	public static BankAccountOwnerPortable New(BankAccountEntity bankAccount) {
		return new BankAccountOwnerPortable(
			bankAccount.GlobalID,
			bankAccount.SyncForced,
			bankAccount.Created,
			bankAccount.Updated,
			bankAccount.Currency,
			bankAccount.Name,
			bankAccount.BalanceSigned,
			bankAccount.BalanceAbsolute,
			bankAccount.BalanceLastUpdate,
			bankAccount.Number,
			bankAccount.IBAN,
			bankAccount.CustomerID,
			bankAccount.Comment
		);
	}
}