using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using No1.FaraBank.Api.Contracts;
using No1.FaraBank.Api.Controllers.Core;
using No1.FaraBank.Api.Shared;

namespace No1.FaraBank.Api.Controllers.Version1.Public;

[Route($"{VersionedAudiencedEndpoint.Version1Public.Prefix}/Current")]
[AllowAnonymous]
[ApiController]
public class CurrentPublicController(ILoggedInUserContract loggedInUserProvider) : ControllerBase
{
	[HttpGet("Profile")]
	public LoggedInUser? Profile() {
		return loggedInUserProvider.GetLoggedInUser();
	}
}