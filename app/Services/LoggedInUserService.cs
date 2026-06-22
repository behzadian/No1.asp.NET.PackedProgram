using No1.Commons.Exceptions;
using No1.FaraBank.Api.Contracts;
using No1.FaraBank.Api.Shared;
using System.Security.Claims;

namespace No1.FaraBank.Api.Services;

public class LoggedInUserService(IHttpContextAccessor httpContextAccessor) : ILoggedInUserContract
{
	private LoggedInUser? loggedInUser;

	LoggedInUser? ILoggedInUserContract.GetLoggedInUser() {
		if (this.loggedInUser.HasValue()) {
			return this.loggedInUser;
		}

		var user = httpContextAccessor.HttpContext?.User;
		if (user == null) {
			return null;
		}

		var isAuthenticated = user.Identity?.IsAuthenticated ?? false;
		if (!isAuthenticated) {
			return null;
		}

		var id = Guid.Parse(NullExpressionException.Exec(() => (user.FindFirst("sub")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value)));
		var email = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
		var name = user.FindFirst("name")?.Value;
		this.loggedInUser = new LoggedInUser(id, name, email, null);

		return this.loggedInUser;
	}
}