using Microsoft.AspNetCore.Authorization;

namespace No1.FaraBank.Api.Config;

public static class SecurityExpressionUtility
{
	public static void Secure(this IEndpointConventionBuilder endpointConventionBuilder, string securityExpression) {
		switch (securityExpression) {
			case "AllowAll":
				endpointConventionBuilder.AllowAnonymous();
				break;
			case "AllowAuthenticated":
				endpointConventionBuilder.RequireAuthorization();
				break;
			case "AllowAdmin":
				endpointConventionBuilder.RequireAuthorization(new AuthorizeAttribute() { Roles = "Admin" });
				break;
			default:
				throw new ArgumentException($"Value `{securityExpression}` is not supported a security expression");
		}
	}
}