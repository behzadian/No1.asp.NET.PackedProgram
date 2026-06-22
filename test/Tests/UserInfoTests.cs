using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using No1.FaraBank.Api.Shared;
using System.Net;
using System.Text.Json;
using test.Core;

namespace test.Tests;

public class UserInfoTests(IntegrationTestFactory factory) : IClassFixture<IntegrationTestFactory>
{
	[Fact]
	public async Task WhenAuthenticatedWithValidTokenThenReturnDetails() {
		// Arrange
		var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("test")).CreateClient();
		using var request = new HttpRequestMessage(HttpMethod.Get, "/v1/Public/Current/Profile");
		var token = await KeyCloakUtility.Token(factory, "id@fb.ir", "password");
		request.Headers.Add("Authorization", $"Bearer {token}");
		var jsonOptions = factory.Services.GetRequiredService<IOptions<Microsoft.AspNetCore.Mvc.JsonOptions>>().Value.JsonSerializerOptions;

		// Act
		var response = await client.SendAsync(request);
		var body = await response.Content.ReadAsStringAsync();
		var result = JsonSerializer.Deserialize<LoggedInUser>(body, jsonOptions);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("bcf62c5d-8cf2-4f9f-9d0e-4b27adfee035", result.GlobalID.ToString());
		Assert.Equal("id@fb.ir", result.Email);
		Assert.Equal("Fara Bank", result.Name);
		Assert.Null(result.Phone);
	}

	[Fact]
	public async Task WhenAnonymousRequestThenReturnNoContent() {
		// Arrange
		var client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("test")).CreateClient();
		using var request = new HttpRequestMessage(HttpMethod.Get, "/v1/Public/Current/Profile");

		// Act
		var response = await client.SendAsync(request);
		var body = await response.Content.ReadAsStringAsync();

		// Assert
		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		Assert.Empty(body);
	}
}