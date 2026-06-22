using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using No1.Commons.Exceptions;
using Serilog;
using System.Security.Claims;

namespace No1.FaraBank.Api.Config;

public static class OAuth2ServiceExtensions
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2629:Logging templates should be constant", Justification = "<>")]
	public static IServiceCollection ConfigAuthentication(this IServiceCollection services, OAuth2Config authConfig) {
		services.AddAuthentication(options => {
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options => {
			options.Authority = authConfig.PrivateUrl;
			options.Audience = authConfig.ClientID;
			options.RequireHttpsMetadata = authConfig.RequiresSecureMetadata;
			options.MetadataAddress = $"{authConfig.PrivateUrl}/.well-known/openid-configuration";
			options.TokenValidationParameters = new TokenValidationParameters {
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidAudiences = authConfig.Audiences,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ClockSkew = TimeSpan.FromMinutes(5),
				ValidIssuer = authConfig.PublicUrl,
				NameClaimType = "preferred_username", // Use preferred_username for IIdentity.Name
			};
			options.Events = new JwtBearerEvents {
				OnAuthenticationFailed = context => {
					Log.Logger.Warning($"Authentication failed. {context.Exception}");
					return Task.CompletedTask;
				},
				OnTokenValidated = context => {
					if (context.Principal?.Identity is ClaimsIdentity claimsIdentity) {
						var token = context.SecurityToken as JsonWebToken ?? throw new NullExpressionException($"context.SecurityToken is not JsonWebToken, is {context.SecurityToken?.GetType()?.Name ?? "NULL"}");
						ExtractRealmRoles(token, claimsIdentity);
						ExtractResourceRoles(token, claimsIdentity, authConfig.ClientID);
					}

					return Task.CompletedTask;
				},
			};
		})
		.AddOpenIdConnect("KeycloakOIDC", options => {
			options.Authority = authConfig.PrivateUrl;
			options.ClientId = authConfig.ClientID;
			options.ClientSecret = authConfig.ClientSecret;
			options.MetadataAddress = $"{authConfig.PrivateUrl}/.well-known/openid-configuration";
			options.RequireHttpsMetadata = authConfig.RequiresSecureMetadata;
			options.ResponseType = "code";
			options.SaveTokens = true;
			options.Scope.Clear();
			options.Scope.Add("openid");
			options.Scope.Add("profile");
			options.Scope.Add("email");
			options.Scope.Add("roles");
			options.TokenValidationParameters = new TokenValidationParameters {
				NameClaimType = "name",
				RoleClaimType = "role",
			};
			options.CallbackPath = "/signin-oidc";
			options.SignedOutCallbackPath = "/signout-callback-oidc";
		});

		services.AddAuthorizationBuilder()
			.SetFallbackPolicy(new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
			.RequireAuthenticatedUser()
			.Build()
		);

		return services;
	}

	private static void ExtractRealmRoles(JsonWebToken token, ClaimsIdentity claimsIdentity) {
		var realmAccessClaim = token.Claims.FirstOrDefault(c => c.Type == "realm_access");
		if (realmAccessClaim != null) {
			var realmAccess = JObject.Parse(realmAccessClaim.Value);
			var roles = realmAccess["roles"]?.ToObject<List<string>>();

			if (roles != null) {
				foreach (var role in roles) {
					claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
				}
			}
		}
	}

	private static void ExtractResourceRoles(JsonWebToken token, ClaimsIdentity claimsIdentity, string clientId) {
		var resourceAccessClaim = token.Claims.FirstOrDefault(c => c.Type == "resource_access");
		if (resourceAccessClaim != null) {
			var resourceAccess = JObject.Parse(resourceAccessClaim.Value);
			var clientRoles = resourceAccess[clientId]?["roles"]?.ToObject<List<string>>();

			if (clientRoles != null) {
				foreach (var role in clientRoles) {
					// Add as both authority and role
					claimsIdentity.AddClaim(new Claim("authority", role));
					claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
				}
			}
		}
	}
}