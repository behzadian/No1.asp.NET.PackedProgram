using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using No1.FaraBank.Api.Contracts;
using No1.FaraBank.Api.Controllers.Core;
using No1.FaraBank.Api.Portables.Owner;

namespace No1.FaraBank.Api.Controllers.Version1.Owner;

[ApiController]
[Route($"{VersionedAudiencedEndpoint.Version1Owner.Prefix}/BankAccounts")]
public class BankAccountsOwnerController(IBankAccountContract bankAccountService, ILoggedInUserContract loggedInUser) : ControllerBase
{
	[HttpGet]
	[Authorize(Roles = "owner")]
	public IEnumerable<BankAccountOwnerPortable> Get() {
		return [];
	}

	[HttpPost]
	[Authorize(Roles = "owner")]
	public Task<BankAccountOwnerPortable> Post(BankAccountOwnerInput input, CancellationToken cancellationToken = default) {
		return bankAccountService.PersistAsync(loggedInUser.GetLoggedInUser()!.GlobalID, input, cancellationToken);
	}
}