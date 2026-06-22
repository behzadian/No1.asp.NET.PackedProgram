using System.Net.Http.Json;

namespace test.Core;

internal static partial class KeyCloakUtility
{
	private static readonly HttpClient HttpClient = new();

	internal static async Task<string> Token(IntegrationTestFactory factory, string user, string pass) {
		return await Token(factory.KcUrl(), "fb", "SwaggerUI", "UBZJJQiHzv8L5dk1X5mEAK8nFoFrWD41", user, pass).ConfigureAwait(true);
	}

	internal static async Task<string> Token(string kcUrl, string realm, string clientId, string clientSecret, string user, string pass) {
		using var content = new FormUrlEncodedContent(new Dictionary<string, string> {
			["grant_type"] = "password",
			["client_id"] = clientId,
			["client_secret"] = string.Empty,
			["username"] = user,
			["password"] = pass,
		});

		var response = await HttpClient.PostAsync(
			new Uri($"{kcUrl}/realms/{realm}/protocol/openid-connect/token"),
			content
		).ConfigureAwait(true);

		response.EnsureSuccessStatusCode();

		var json = await response.Content.ReadFromJsonAsync<TokenResponse>().ConfigureAwait(true);

		return json!.AccessToken;
	}
}