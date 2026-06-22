using No1.asp.NET.Commons.Exceptions;

namespace No1.FaraBank.Api.Config;

public record NHibernateExtendedConfig(string ConnectionString, string MappingsExportFolder, bool PrintSchema)
{
	internal static NHibernateExtendedConfig Get(IConfiguration configuration) {
		return configuration.GetSection(nameof(NHibernateExtendedConfig)).Get<NHibernateExtendedConfig>() ?? throw new ConfigurationException<NHibernateExtendedConfig>();
	}
}