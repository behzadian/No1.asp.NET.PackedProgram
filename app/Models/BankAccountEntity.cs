using No1.FaraBank.Api.Enums;
using No1.FaraBank.Api.Models.Core;
using NodaTime;

namespace No1.FaraBank.Api.Models;

public class BankAccountEntity : EntityBase
{
	public virtual Guid OwnerID { get; set; }

	public virtual Currency Currency { get; set; }

	public virtual string? Name { get; set; }

	public virtual decimal BalanceSigned { get; set; } = 0;

	public virtual decimal BalanceAbsolute { get; set; } = 0;

	public virtual string Number { get; set; } = string.Empty;

	public virtual string IBAN { get; set; } = string.Empty;

	public virtual string CustomerID { get; set; } = string.Empty;

	public virtual string Comment { get; set; } = string.Empty;

	public virtual Instant? BalanceLastUpdate { get; set; }

	public virtual BankEnterpriseEntity? BankEnterprise { get; set; }
}