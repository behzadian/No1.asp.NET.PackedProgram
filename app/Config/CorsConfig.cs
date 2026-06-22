using Microsoft.AspNetCore.Cors.Infrastructure;

namespace No1.FaraBank.Api.Config;

public class CorsConfig
{
	public TimeSpan PreflightMaxAge { get; set; }

	public bool Credentials { get; set; }

	public bool WildcardSubdomains { get; set; }

	public string[] Origins { get; set; } = [];

	public string[] Methods { get; set; } = [];

	public string[] Headers { get; set; } = [];

	public string[] ExposedHeaders { get; set; } = [];

	public static CorsConfig Get(IConfiguration configuration) {
		var cfg = new CorsConfig();
		configuration.GetSection(nameof(CorsConfig)).Bind(cfg);
		return cfg;
	}

	public void Config(CorsPolicyBuilder policy) {
		policy.SetPreflightMaxAge(this.PreflightMaxAge);

		if (this.Origins.IsUsable()) {
			policy.WithOrigins(this.Origins);
		}

		if (this.Methods.IsUsable()) {
			policy.WithMethods(this.Methods);
		}

		if (this.Headers.IsUsable()) {
			policy.WithHeaders(this.Headers);
		}

		if (this.ExposedHeaders.IsUsable()) {
			policy.WithExposedHeaders(this.ExposedHeaders);
		}

		if (this.Credentials) {
			policy.AllowCredentials();
		} else {
			policy.DisallowCredentials();
		}

		if (this.WildcardSubdomains) {
			policy.SetIsOriginAllowedToAllowWildcardSubdomains();
		}
	}
}