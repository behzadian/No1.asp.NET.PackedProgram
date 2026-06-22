using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Json;
using No1.asp.NET.Commons.Extensions;
using No1.EnvBasedEndpoints;
using No1.FaraBank.Api.Config;
using No1.FaraBank.Api.Exceptions.Core;
using No1.FaraBank.Api.Logging;
using No1.OpenSearchCommons;
using OpenTelemetry.Trace;
using Serilog;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace No1.FaraBank.Api;

public class Program
{
	protected Program() {
	}

	public static void Main(string[] args) {
		var builder = WebApplication.CreateBuilder(args);
		ConfigurationManager configuration = builder.Configuration;

		Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine("SERILOG ERROR: " + msg));
		Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).Enrich.With<ActivityEnricher>().CreateLogger();
		builder.Host.UseSerilog();

		builder.Services.AddOpenTelemetry().WithTracing(builder => builder
				.AddAspNetCoreInstrumentation()
				.AddHttpClientInstrumentation()
		);

		var environment = builder.Environment;
		var thisAssembly = Assembly.GetExecutingAssembly();
		var buildInstant = thisAssembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(x => x.Key.Equals("BuildInstant"))?.Value?.Otherwise("Unavailable") ?? "Undefined";
		Console.WriteLine($"Starting in {environment.EnvironmentName} environment. Built at: {buildInstant} UTC");
		Log.Logger.Information("Starting in {Environment} environment. Built at: {BuildInstant} UTC", environment.EnvironmentName, buildInstant);

		var devConfig = configuration.GetNeededConfig<DevelopmentConfig>();
		var authConfig = configuration.GetNeededConfig<OAuth2Config>();
		var swaggerConfig = configuration.GetNeededConfig<SwaggerConfig>();

		builder.RegisterOpenSearchCertificateSignedHttpClient();

		// Health check.
		var healthConfig = HealthConfig.Get(configuration);
		var databaseSettings = NHibernateExtendedConfig.Get(configuration);
		if (healthConfig.Enabled) {
			builder.Services
				.AddHealthChecks()
				.AddNpgSql(databaseSettings.ConnectionString, name: "PostgreSQL")
				.AddUrlGroup(new Uri(authConfig.HealthUrl), name: "KeyCloak")
				.AddCheck<OpenSearchHealthCheck>("OpenSearch")
				.AddApplicationInsightsPublisher();
		}

		// Atofac
		builder.ConfigureAutofac();

		// Exception handling
		builder.Services.AddProblemDetails(options => {
			options.CustomizeProblemDetails = context => {
				context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
				context.ProblemDetails.Extensions.TryAdd("traceId", context.HttpContext.TraceIdentifier);
			};
		});
		builder.Services.Configure<JsonOptions>(options => ConfigJsonOptions(options.SerializerOptions, devConfig));
		builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

		// Authentication
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddControllers()
			.AddJsonOptions(options => ConfigJsonOptions(options.JsonSerializerOptions, devConfig))
			.ConfigureApplicationPartManager(x => EnvironmentControllerFeatureProvider.Register(x, environment));
		builder.EnableSwagger(authConfig);

		builder.Services.AddCors(options => options.AddDefaultPolicy(CorsConfig.Get(builder.Configuration).Config));
		builder.Services.ConfigAuthentication(authConfig);
		builder.Services.AddNHibernate(builder.Configuration);

		// Allow services to access the current HttpContext and User
		builder.Services.AddHttpContextAccessor();

		var app = builder.Build();

		app.UseSerilogRequestLogging();
		app.UseExceptionHandler();

		app.EnableSwagger(authConfig, swaggerConfig);

		if (healthConfig.Enabled) {
			app.UseHealthChecks(healthConfig.Url, new HealthCheckOptions() { Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
			app.MapHealthChecks(healthConfig.Url).RequireHost(healthConfig.RequiredHosts).Secure(healthConfig.SecurityExpression);
		}

		// CORS must run before authentication/authorization so preflight requests
		// are handled without requiring an authenticated user.
		app.UseCors();

		app.UseAuthentication();
		app.UseAuthorization();
		app.UseHttpsRedirection();
		app.MapControllers();

		app.Run();
	}

	private static void ConfigJsonOptions(JsonSerializerOptions jsonSerializerOptions, DevelopmentConfig developmentConfig) {
		jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
		jsonSerializerOptions.WriteIndented = developmentConfig.WriteIndented;
	}
}