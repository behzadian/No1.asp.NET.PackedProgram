using No1.FaraBank.Api.Models.Core;

namespace No1.FaraBank.Api.Models;

public class BankEnterpriseEntity : EntityBase
{
	public virtual string Name { get; set; } = string.Empty;

	public virtual string BIC { get; set; } = string.Empty;

	public virtual FileEntity? Logo { get; set; }
}