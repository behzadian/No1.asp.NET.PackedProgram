namespace No1.FaraBank.Api.Config;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AuthorityNeededAttribute(params string[] authorities) : Attribute, IAuthorizationFilter
{
	public void OnAuthorization(AuthorizationFilterContext context) {
		var user = context.HttpContext.User;

		if (!user.Identity?.IsAuthenticated ?? true) {
			context.Result = new UnauthorizedResult();
			return;
		}

		// Check authorities from resource_access claim
		var hasAuthority = this.CheckAuthorities(user);

		// Check roles from realm_access claim
		var hasRole = this.CheckRoles(user);

		// If user has EITHER authority OR role, allow access
		if (!hasAuthority && !hasRole) {
			context.Result = new ForbidResult();
		}
	}

	private bool CheckAuthorities(System.Security.Claims.ClaimsPrincipal user) {
		// Get authorities from custom claim added during token validation
		var userAuthorities = user.FindAll("authority")
			.Select(c => c.Value)
			.ToList();

		return Array.Exists(authorities, userAuthorities.Contains);
	}

	private bool CheckRoles(System.Security.Claims.ClaimsPrincipal user) {
		var userRoles = user.FindAll(System.Security.Claims.ClaimTypes.Role)
			.Select(c => c.Value)
			.ToList();

		return Array.Exists(authorities, userRoles.Contains);
	}
}