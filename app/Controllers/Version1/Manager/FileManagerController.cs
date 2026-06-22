using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using No1.FaraBank.Api.Controllers.Core;
using No1.FaraBank.Api.Portables.Manager;

namespace No1.FaraBank.Api.Controllers.Version1.Manager;

[Route($"{VersionedAudiencedEndpoint.Version1Manager.Prefix}/Files")]
[ApiController]
[Authorize(Roles = "manager")]
public class FileManagerController : ControllerBase
{
	[HttpPost]
	[Consumes("multipart/form-data")]
	[RequestSizeLimit(1024 * 1024 * 10)]
	public FileManagerPortable Persist(FileManagerInput input, string entityName, Guid entityId) {
		throw new NotImplementedException();
	}
}