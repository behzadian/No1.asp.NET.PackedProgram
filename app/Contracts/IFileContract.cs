using No1.FaraBank.Api.Models;
using No1.FaraBank.Api.Portables.Manager;

namespace No1.FaraBank.Api.Contracts;

public interface IFileContract
{
	Task<FileEntity?> ProcessAsync<TOwnerEntity>(FileEntity? current, FileManagerInput input, Guid ownerEntityGlobalId, string name, CancellationToken cancellationToken = default);
}