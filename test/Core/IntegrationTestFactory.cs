using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using No1.FaraBank.Api;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;

namespace test.Core;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "_")]
public sealed class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
	private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder("postgres:17")
			.WithDatabase("testdb")
			.WithUsername("postgres")
			.WithPassword("postgres")
			.WithCleanUp(true)
			.Build();

	private readonly KeycloakContainer kcContainer = new KeycloakBuilder("quay.io/keycloak/keycloak:23.0")
			.WithRealm("../../../../kc/import/fb-realm.json")
			.Build();

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1055:URI-like return values should not be strings", Justification = "_")]
	public string KcUrl() => this.kcContainer.GetBaseAddress();

	async Task IAsyncLifetime.InitializeAsync() {
		await this.kcContainer.StartAsync().ConfigureAwait(true);
		await this.dbContainer.StartAsync().ConfigureAwait(true);
		await RunFlywayMigrationsAsync(this.dbContainer.GetConnectionString()).ConfigureAwait(true);
	}

	async Task IAsyncLifetime.DisposeAsync() {
		await this.dbContainer.StopAsync().ConfigureAwait(true);
		await this.kcContainer.StopAsync().ConfigureAwait(true);
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder) {
		ArgumentNullException.ThrowIfNull(builder);
		builder.UseEnvironment("test");
		builder.UseSetting("NHibernateExtendedConfig:ConnectionString", this.dbContainer.GetConnectionString());
		builder.UseSetting("OAuth2Config:PublicUrl", $"{this.kcContainer.GetBaseAddress()}realms/fb");
		builder.UseSetting("OAuth2Config:PrivateUrl", $"{this.kcContainer.GetBaseAddress()}realms/fb");
		builder.UseSetting("OAuth2Config:HealthUrl", $"{this.kcContainer.GetBaseAddress()}health/ready");
	}

	private static async Task RunFlywayMigrationsAsync(string connectionString) {
		// Path to your Flyway migration scripts (adjust accordingly)
		var migrationPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "db", "flyway", "migrations"));
		if (Path.Exists(migrationPath)) {
			Console.WriteLine($"Flyway migration path: `{migrationPath}`");
			foreach (var migration in Directory.GetFiles(migrationPath)) {
				Console.WriteLine($"Flyway migration `{migration}` found");
			}
		} else {
			throw new TestSetupException($"Flyway migration path `{migrationPath}` can't be found.");
		}

		var flywayContainer = new ContainerBuilder("flyway/flyway:11-alpine")
			.WithBindMount(migrationPath, "/flyway/sql")
			.WithCommand("migrate")
			.WithEnvironment("FLYWAY_URL", connectionString)
			.WithEnvironment("FLYWAY_USER", "postgres")
			.WithEnvironment("FLYWAY_PASSWORD", "postgres")
			.WithEnvironment("FLYWAY_SCHEMAS", "public")
			.WithEnvironment("FLYWAY_BASELINE_ON_MIGRATE", "true")
			.WithCleanUp(true)
			.Build();

		await flywayContainer.StartAsync().ConfigureAwait(true);
		await flywayContainer.DisposeAsync().ConfigureAwait(true);
	}
}