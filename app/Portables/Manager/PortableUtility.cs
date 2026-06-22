using No1.FaraBank.Api.Models;

namespace No1.FaraBank.Api.Portables.Manager;

public static class PortableUtility
{
	internal static FileManagerPortable New(this FileEntity file) => new(file.ID, file.GlobalID, file.Path);

	internal static BankEnterpriseManagerPortable New(this BankEnterpriseEntity bank) => new(
		bank.ID,
		bank.GlobalID,
		bank.Name,
		bank.Logo?.New()
	);
}