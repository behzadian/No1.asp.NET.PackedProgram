using No1.asp.NET.Commons.Extensions;

namespace No1.FaraBank.Api.Config;

public record HealthConfig(bool Enabled, string[] RequiredHosts, string SecurityExpression, string Url = "/app/health/status")
{
	public static HealthConfig Get(IConfiguration configuration) {
		var section = configuration.GetSection(nameof(HealthConfig));
		if (!section.Exists()) {
			throw new InvalidOperationException($"Configuration section '{nameof(HealthConfig)}' is missing.");
		}

		// Read primitive values
		var enabled = section.GetValue<bool>(nameof(Enabled));
		var securityExpression = section.GetValue<string>(nameof(SecurityExpression)) ?? string.Empty;
		var url = section.GetValue<string>(nameof(Url)) ?? "/app/health/status";

		var requiredHostsSection = section.GetSection(nameof(RequiredHosts));
		string[] requiredHosts = requiredHostsSection.ReadStringArray();

		return new HealthConfig(enabled, requiredHosts, securityExpression, url);
	}
}