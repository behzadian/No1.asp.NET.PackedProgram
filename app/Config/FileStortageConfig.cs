namespace No1.FaraBank.Api.Config;

public record FileStortageConfig(string PhysicalLocation, int DatabaseStorageMaxBytes = 100 * 1024)
{
	public static FileStortageConfig Get(IConfiguration configuration) {
		return configuration.GetSection(nameof(FileStortageConfig)).Get<FileStortageConfig>()!;
	}
}