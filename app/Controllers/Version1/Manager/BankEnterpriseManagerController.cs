using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using No1.FaraBank.Api.Contracts;
using No1.FaraBank.Api.Controllers.Core;
using No1.FaraBank.Api.Data.Shared;
using No1.FaraBank.Api.Models;
using No1.FaraBank.Api.Portables.Manager;

namespace No1.FaraBank.Api.Controllers.Version1.Manager;

[ApiController]
[Route($"{VersionedAudiencedEndpoint.Version1Manager.Prefix}/BankEnterprises")]
[Authorize(Roles = "manager")]
public class BankEnterpriseManagerController(IBankEnterpriseContract bankOfficialContract) : ControllerBase
{
	[HttpGet]
	[Authorize(Roles = "manager")]
	public async Task<Paged<BankEnterpriseManagerPortable>> Get(Pagination<BankEnterpriseEntity> pagination) {
		ArgumentNullException.ThrowIfNull(pagination);
		return await bankOfficialContract.PagedAsync(pagination);
	}

	[HttpPost]
	[Authorize(Roles = "manager")]
	public async Task<BankEnterpriseManagerPortable> Post(BankEnterpriseManagerInput input, CancellationToken cancellationToken = default) {
		return await bankOfficialContract.PersistAsync(input, cancellationToken);
	}
}