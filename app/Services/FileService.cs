using No1.FaraBank.Api.Config;
using No1.FaraBank.Api.Contracts;
using No1.FaraBank.Api.Data.Repos;
using No1.FaraBank.Api.Models;
using No1.FaraBank.Api.Portables.Manager;

namespace No1.FaraBank.Api.Services;

public class FileService(FileStortageConfig fileStortageConfig, FileRepo repo, IClockContract clock) : IFileContract
{
	async Task<FileEntity?> IFileContract.ProcessAsync<TOwnerEntity>(FileEntity? current, FileManagerInput input, Guid ownerEntityGlobalId, string name, CancellationToken cancellationToken) {
		if (input.File.HasValue() && input.Delete) {
			throw new ArgumentException("While input.Delete is true, input.File is not null.");
		}

		if (input.Delete) {
			if (current != null) {
				await repo.DeleteAsync(current, cancellationToken);
			}

			return null;
		} else if (input.File == null) {
			return current;
		}

		current ??= new FileEntity() { Created = clock.Now() };

		var pathParts = new List<int>(10);
		var hashCode = Math.Abs(current.GlobalID.GetHashCode());
		while (hashCode > 0) {
			pathParts.Add(hashCode % 10);
			hashCode /= 10;
		}

		var pathId = string.Join("/", pathParts);

		current.Updated = clock.Now();
		current.Path = $"{fileStortageConfig.PhysicalLocation}/{typeof(TOwnerEntity).Name.GetHashCode()}/{pathId}/{ownerEntityGlobalId}-{name}{Path.GetExtension(input.File.FileName)}";
		await repo.Persist(current, cancellationToken);

		return current;
	}
}