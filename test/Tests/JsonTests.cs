using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using No1.FaraBank.Api.LocalOnly;
using test.Core;

namespace test.Tests;

public class JsonTests(IntegrationTestFactory factory) : IClassFixture<IntegrationTestFactory>
{
	private readonly HttpClient client = factory.WithWebHostBuilder(static builder => builder.UseEnvironment("test")).CreateClient();

	[Fact]
	public async Task WhenEndpointCalledWithValidTextualEnumValueThenReturnsValidTextualEnumValue() {
		// Arrange
		using var request = new HttpRequestMessage(HttpMethod.Get, $"/LocalOnly/Test/{nameof(TestController.Log)}/?level={LogLevel.Trace}");

		// Act
		var response = await this.client.SendAsync(request);
		var content = await response.Content.ReadAsStringAsync();

		// Assert
		Assert.Contains("\"logLevel\": \"Trace\"", content, StringComparison.InvariantCultureIgnoreCase);
	}

	[Fact]
	public async Task WhenEndpointCalledWithInvalidTextualEnumValueThenReturnsInputValidationError() {
		// Arrange
		using var request = new HttpRequestMessage(HttpMethod.Get, $"/LocalOnly/Test/{nameof(TestController.Log)}/?level=Invalid");

		// Act
		var response = await this.client.SendAsync(request);

		// Assert
		Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task WhenEndpointCalledWithValidNumericalEnumValueThenReturnsValidTextualEnumValue() {
		// Arrange
		using var request = new HttpRequestMessage(HttpMethod.Get, $"/LocalOnly/Test/{nameof(TestController.Log)}/?level=2");

		// Act
		var response = await this.client.SendAsync(request);
		var content = await response.Content.ReadAsStringAsync();

		// Assert
		Assert.Contains("\"logLevel\": \"Information\"", content, StringComparison.InvariantCultureIgnoreCase);
	}

	[Fact]
	public async Task WhenEndpointCalledWithInvalidNumericalEnumValueThenReturnsInputValidationError() {
		// Arrange
		using var request = new HttpRequestMessage(HttpMethod.Get, $"/LocalOnly/Test/{nameof(TestController.Log)}/?level=10");

		// Act
		var response = await this.client.SendAsync(request);

		// Assert
		Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
	}
}