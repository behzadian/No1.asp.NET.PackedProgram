using No1.FaraBank.Api.Models;

namespace No1.FaraBank.Api.Portables.Manager;

public record BankEnterpriseManagerPortable(long ID, Guid GlobalID, string Name, FileManagerPortable? Logo)
{
	internal static BankEnterpriseManagerPortable New(BankEnterpriseEntity bank) => new(
		bank.ID,
		bank.GlobalID,
		bank.Name,
		bank.Logo?.New()
	);
}