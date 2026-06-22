using System.Diagnostics.CodeAnalysis;

namespace test.Core;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "_")]
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "_")]
internal sealed record TokenResponse(string access_token)
{
	public string AccessToken => this.access_token;
}