namespace No1.FaraBank.Api.Config;

public record DevelopmentConfig(bool PrintDetailsPublicly, bool WriteIndented)
{
	public static DevelopmentConfig Get(IConfiguration configuration) => configuration.GetSection(nameof(DevelopmentConfig)).Get<DevelopmentConfig>()!;
}