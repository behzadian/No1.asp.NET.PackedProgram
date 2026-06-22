using NHibernate.Linq;
using No1.FaraBank.Api.Contracts;
using No1.FaraBank.Api.Models;

namespace No1.FaraBank.Api.Data.Repos;

public class FileRepo(NHibernate.ISession session, IClockContract cloak)
{
	internal async Task DeleteAsync(FileEntity file, CancellationToken cancellationToken = default) {
		file.Deleted = cloak.Now();
		await session.UpdateAsync(file, cancellationToken);
	}

	internal async Task<FileEntity?> GetBankByGlobalID(Guid globalId, CancellationToken cancellationToken = default) {
		return await session.Query<FileEntity>().Where(x => x.Deleted == null && x.GlobalID == globalId).FirstOrDefaultAsync(cancellationToken);
	}

	internal async Task Persist(FileEntity file, CancellationToken cancellationToken = default) {
		await session.SaveOrUpdateAsync(file, cancellationToken);
	}
}