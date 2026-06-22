using System.Net;

namespace No1.FaraBank.Api.Config;

public record SwaggerConfig(IEnumerable<string> AllowedCIDRs)
{
	public IEnumerable<IPNetwork> AllowedNetworks { get; set; } = AllowedCIDRs.Where(x => x.IsUsable()).Select(IPNetwork.Parse);
}