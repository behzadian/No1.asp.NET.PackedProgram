using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi;
using No1.Commons.Exceptions;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Configuration;
using System.Reflection;
using static No1.FaraBank.Api.Controllers.Core.VersionedAudiencedEndpoint;

namespace No1.FaraBank.Api.Config;

internal static class ProgramSwaggerExtensions
{
	private static readonly string[] OAuth2Scopes = ["openid", "profile", "email"];

	internal static void EnableSwagger(this WebApplicationBuilder builder, OAuth2Config authConfig) {
		var thisAssembly = Assembly.GetExecutingAssembly();

		var buildInstant = thisAssembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(x => x.Key.Equals("BuildInstant"))?.Value?.Otherwise("Unavailable") ?? "Undefined";
		var buildHash = thisAssembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(x => x.Key.Equals("BuildHash"))?.Value?.Otherwise("Unavailable") ?? "Undefined";
		var commitHash = thisAssembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(x => x.Key.Equals("CommitHash"))?.Value?.Otherwise("Unavailable") ?? "Undefined";

		builder.Services.AddSwaggerGen(options => {
			var desc = $"Built at [**{buildInstant} (UTC)**]. Build hash: [***{buildHash.Trim()}***], Commit hash: [***{commitHash.Trim()}***].";
			options.SwaggerDoc("_", new OpenApiInfo {
				Title = "Default",
				Version = "_",
				Description = desc,
			});

			foreach (var version in Versions) {
				options.SwaggerDoc(version.Name, new OpenApiInfo {
					Title = $"FaraBank Version {version.Version} for {version.Audience}",
					Version = version.UrlPrefix,
					Description = desc,
				});
			}

			options.DocInclusionPredicate((docName, apiDesc) => {
				var descriptor = NullExpressionException.Exec(() => apiDesc.ActionDescriptor as ControllerActionDescriptor);
				string @namespace = descriptor.ControllerTypeInfo.Namespace!;
				string controller = descriptor.ControllerTypeInfo.Name.StripEnd("Controller");

				var version = Versions.FirstOrDefault(v => v.Name == docName);

				if (version.HasValue()) {
					bool namespaceMatched = @namespace.Contains(version.NamespaceKeyPeriodSurrounded);
					bool controllerMatched = @controller.EndsWith(version.Audience.ToString());

					return namespaceMatched && controllerMatched;
				} else {
					// Default. When there is no related version.
					return !Versions.Any(x => @namespace.Contains(x.NamespaceKeyPeriodSurrounded));
				}
			});

			options.IncludeXmlComments(thisAssembly);
			options.OperationFilter<AddResponseHeadersFilter>(); // [SwaggerResponseHeader]
			options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
			options.OperationFilter<SecurityRequirementsOperationFilter>();
			var securityScheme = new OpenApiSecurityScheme {
				Type = SecuritySchemeType.OAuth2,
				Flows = new OpenApiOAuthFlows {
					AuthorizationCode = new OpenApiOAuthFlow {
						AuthorizationUrl = new Uri($"{authConfig.PublicUrl}/protocol/openid-connect/auth"),
						TokenUrl = new Uri($"{authConfig.PublicUrl}/protocol/openid-connect/token"),
						Scopes = new Dictionary<string, string> {
							{ "openid", "OpenID Connect" },
							{ "profile", "User Profile" },
							{ "email", "Email Address" },
						},
					},
				},
			};
			options.AddSecurityDefinition("oauth2", securityScheme);
			options.AddSecurityRequirement(document => new OpenApiSecurityRequirement {
				{
					new OpenApiSecuritySchemeReference("oauth2", document),
					OAuth2Scopes.ToList()
				},
			});
		});
	}

	internal static void EnableSwagger(this WebApplication app, OAuth2Config authConfig, SwaggerConfig swaggerConfig) {
		app.Use(async (context, next) => {
			if (context.Request.Path.StartsWithSegments("/swagger")) {
				if (swaggerConfig.AllowedNetworks.IsUsable()) {
					var remoteIp = context.Connection.RemoteIpAddress;

					if (remoteIp is null || !swaggerConfig.AllowedNetworks.Any(ip => ip.Contains(remoteIp))) {
						context.Response.StatusCode = StatusCodes.Status403Forbidden;
						await context.Response.WriteAsync("Forbidden");
						return;
					}
				} else {
					if (app.Environment.IsProduction()) {
						throw new ConfigurationErrorsException("No allowed IP is specified for accessing swagger on production.");
					}

					Log.Warning("No allowed IP is specified for accessing swagger.");
				}
			}

			await next();
		});
		app.UseSwagger();
		app.UseSwaggerUI(options => {
			options.OAuthClientId(authConfig.ClientID);
			options.OAuthClientSecret(authConfig.ClientSecret); // Optional
			options.OAuthRealm(authConfig.Realm);
			options.OAuthAppName("SwaggerUI");
			options.OAuthUsePkce();
			options.SwaggerEndpoint($"/swagger/_/swagger.json", "Default");
			foreach (var versionName in Versions.Select(v => v.Name)) {
				options.SwaggerEndpoint($"/swagger/{versionName}/swagger.json", versionName);
			}
		});
	}
}