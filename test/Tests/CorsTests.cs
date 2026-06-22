using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Net;
using test.Core;

namespace test.Tests;

public class CorsTests(IntegrationTestFactory factory) : IClassFixture<IntegrationTestFactory>
{
	[Fact]
	public async Task WhenRequestFromAllowedOriginThenGetAllowedResponse() {
		// Create a customized client for this test only
		var client = factory.WithWebHostBuilder(builder => {
			builder.UseEnvironment("test");

			// Override specific configuration keys for this test
			builder.ConfigureAppConfiguration((context, config) => {
				config.AddInMemoryCollection(new Dictionary<string, string?> {
					["CorsConfig:Origins:0"] = "https://the-allowed-origin.com",
				});
			});
		}).CreateClient();

		// Now test with the origin you just allowed
		using var request = new HttpRequestMessage(HttpMethod.Options, "/Client/Profile");
		request.Headers.Add("Origin", "https://the-allowed-origin.com");
		request.Headers.Add("Access-Control-Request-Method", "GET");

		var response = await client.SendAsync(request);

		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		Assert.Contains("https://the-allowed-origin.com", response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault() ?? string.Empty, StringComparison.InvariantCultureIgnoreCase);
	}

	[Fact]
	public async Task WhenRequestFromDisallowedOriginThenGetDisallowedResponse() {
		// Create a customized client for this test only
		var client = factory.WithWebHostBuilder(builder => {
			builder.UseEnvironment("test");

			// Override specific configuration keys for this test
			builder.ConfigureAppConfiguration((context, config) => {
				config.AddInMemoryCollection(new Dictionary<string, string?> {
					["CorsConfig:Origins:0"] = "https://the-allowed-origin.com",
				});
			});
		}).CreateClient();

		// Now test with the origin you just allowed
		using var request = new HttpRequestMessage(HttpMethod.Options, "/Client/Profile");
		request.Headers.Add("Origin", "https://not-allowed-origin.com");
		request.Headers.Add("Access-Control-Request-Method", "GET");

		var response = await client.SendAsync(request);

		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
	}
}