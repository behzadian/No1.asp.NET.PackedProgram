namespace No1.FaraBank.Api.Config;

using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using No1.FaraBank.Api.Data.Conventions;
using No1.NHibernateNodaTime;

/// <summary>
/// Adds nhibernate to application.
/// </summary>
public static class NHibernateServiceExtensions
{
	public static IServiceCollection AddNHibernate(this IServiceCollection services, IConfiguration configuration) {
		var databaseSettings = NHibernateExtendedConfig.Get(configuration);
		services.AddSingleton(databaseSettings);

		var nhibernateConfig = NHibernateConfiguration(databaseSettings);

		// Optionally print generated DDL to the application output without executing it.
		// Enable by setting configuration key "NHibernate:PrintSchema" = true (e.g., in appsettings.Development.json)
		if (databaseSettings.PrintSchema) {
			try {
				var schemaExport = new SchemaExport(nhibernateConfig);
				schemaExport.SetDelimiter(";");

				// script: write SQL to stdout, export: don't execute against DB, justDrop: false
				schemaExport.Execute(useStdOut: true, execute: false, justDrop: false);
			} catch (Exception ex) {
				Console.WriteLine($"Failed to generate schema DDL: {ex}");
			}
		}

		var sessionFactory = nhibernateConfig.BuildSessionFactory();
		services.AddSingleton(sessionFactory);
		services.AddScoped(provider => provider.GetRequiredService<ISessionFactory>().OpenSession());

		return services;
	}

	public static Configuration NHibernateConfiguration(NHibernateExtendedConfig databaseConfig) {
		var configuration = new Configuration();

		// Base configuration
		configuration.DataBaseIntegration(db => {
			db.ConnectionString = databaseConfig.ConnectionString;
			db.Dialect<PostgreSQL83Dialect>();
			db.Driver<NpgsqlDriver>();
			db.LogFormattedSql = true;
			db.LogSqlInConsole = true;
		});

		FluentConfiguration fluentConfig = Fluently
			.Configure(configuration)
			.Mappings(m => AutoMapModel(m, databaseConfig.MappingsExportFolder));

		return fluentConfig.BuildConfiguration();
	}

	public static void AutoMapModel(MappingConfiguration m, string? mappingsExportFolder = null) {
		var autoPersistenceModel = AutoMap
			.AssemblyOf<Program>()
			.Where(t => t.Name.EndsWith("Entity"))
			.UseOverridesFromAssemblyOf<Program>()
			.Conventions.Add<GuidTypeConvention>()
			.Conventions.Add<TableNameConvention>()
			.Conventions.Add<SnakeCaseColumnNameConvention>()
			.Conventions.Add<SnakeCaseForeignKeyConvention>()
			.Conventions.Add<SnakeCaseIdConvention>()
			.Conventions.EnableNodaTime()
		;
		var autoMappingsContainer = m.AutoMappings.Add(autoPersistenceModel);
		if (!string.IsNullOrWhiteSpace(mappingsExportFolder)) {
			Directory.CreateDirectory(mappingsExportFolder);
			autoMappingsContainer.ExportTo(mappingsExportFolder);
		}
	}
}