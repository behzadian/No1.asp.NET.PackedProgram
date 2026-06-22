using No1.FaraBank.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace No1.FaraBank.Api.Portables.Owner;

/// <summary>
/// Used to create new Bank object.
/// </summary>
/// <param name="GlobalID">Unique identifier for the bank account.</param>
/// <param name="Name">Can be null.</param>
/// <param name="Number">Bank internal account number.</param>
/// <param name="IBAN">Bank Account number.</param>
/// <param name="BankEnterpriseGlobalID">Bank that holds the account.</param>
/// <param name="Currency">Currency of Account.</param>
/// <param name="CustomerID">CustomerID of account.</param>
public record BankAccountOwnerInput(
	Guid? GlobalID,
	[StringLength(256)] string? Name,
	[StringLength(32)] string Number,
	[StringLength(32)] string CustomerID,
	[Required, StringLength(26), RegularExpression(@"\w{2}\d{24}")] string IBAN,
	Guid BankEnterpriseGlobalID,
	Currency Currency
);