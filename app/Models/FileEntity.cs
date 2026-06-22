using No1.FaraBank.Api.Models.Core;

namespace No1.FaraBank.Api.Models;

public class FileEntity : EntityBase
{
	public virtual string Path { get; set; } = string.Empty;
}