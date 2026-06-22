namespace No1.FaraBank.Api.Config;

public record OAuth2Config(
	string Realm,
	string ClientID,
	string ClientSecret,
	string PublicUrl,
	string PrivateUrl,
	string HealthUrl,
	bool RequiresSecureMetadata,
	IEnumerable<string> Audiences
)
{
	public static OAuth2Config Get(IConfiguration configuration) => configuration.GetSection(nameof(OAuth2Config)).Get<OAuth2Config>()!;
}