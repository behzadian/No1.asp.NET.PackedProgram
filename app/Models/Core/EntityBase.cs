using NodaTime;

namespace No1.FaraBank.Api.Models.Core;

public abstract class EntityBase
{
	public virtual long ID { get; set; }

	public virtual Guid GlobalID { get; set; } = Guid.NewGuid();

	public virtual bool SyncForced { get; set; } = false;

	public virtual Instant Created { get; set; }

	public virtual Instant Updated { get; set; }

	public virtual Instant? Deleted { get; set; }
}