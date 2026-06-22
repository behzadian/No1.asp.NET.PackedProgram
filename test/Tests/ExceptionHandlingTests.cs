using Microsoft.AspNetCore.Hosting;
using No1.Commons.Extensions;
using No1.FaraBank.Api.Config;
using No1.FaraBank.Api.LocalOnly;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using test.Core;

namespace test.Tests;

public class ExceptionHandlingTests(IntegrationTestFactory factory) : IClassFixture<IntegrationTestFactory>
{
	[Fact]
	public async Task WhenRequestAnEndpointThatThrowsExceptionThenExpectDetailsOnNonProductionAndFormattedJson() {
		// Create a customized client for this test only
		var client = factory.WithWebHostBuilder(builder => {
			builder.UseEnvironment("test");

			// Override specific configuration keys for this test
			builder.UseSetting($"{nameof(DevelopmentConfig)}:{nameof(DevelopmentConfig.PrintDetailsPublicly)}", "true");
			builder.UseSetting($"{nameof(DevelopmentConfig)}:{nameof(DevelopmentConfig.WriteIndented)}", "true");
		}).CreateClient();

		// Now test with the origin you just allowed
		using var request = new HttpRequestMessage(HttpMethod.Get, "/LocalOnly/Test/ThrowException");

		var response = await client.SendAsync(request);
		var json = await response.Content.ReadAsStringAsync();
		var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!;

		Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
		Assert.NotEmpty(json);
		Assert.True(json.Count('\n') > 0);
		Assert.Equal("https://httpstatuses.com/500", result["type"].GetString());
		Assert.Equal("An error occurred while processing your request.", result["title"].GetString());
		Assert.Equal(500, result["status"].GetInt32());
		Assert.Equal("GET /LocalOnly/Test/ThrowException", result["instance"].GetString());
		Assert.Equal("Exception", result["exceptionType"].GetString());
		Assert.True(result["traceId"].GetString().IsUsable());
		Assert.Equal("System.Exception: Exception Message", result["stackTrace"].GetProperty("001").GetString());
		Assert.False(result.ContainsKey("detail"));
		Assert.Equal(7, result.Count);
	}

	[Fact]
	public async Task WhenRequestAnEndpointThatThrowsExceptionThenExpectNoDetailsOnProductionAndSingleLineJson() {
		// Create a customized client for this test only
		var client = factory.WithWebHostBuilder(builder => {
			builder.UseEnvironment("test");

			// Override specific configuration keys for this test
			builder.UseSetting($"{nameof(DevelopmentConfig)}:{nameof(DevelopmentConfig.PrintDetailsPublicly)}", "false");
			builder.UseSetting($"{nameof(DevelopmentConfig)}:{nameof(DevelopmentConfig.WriteIndented)}", "false");
		}).CreateClient();

		// Now test with the origin you just allowed
		using var request = new HttpRequestMessage(HttpMethod.Get, "/LocalOnly/Test/ThrowException");

		var response = await client.SendAsync(request);
		var json = await response.Content.ReadAsStringAsync();
		var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!;

		Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
		Assert.NotEmpty(json);
		Assert.True(json.Count('\n') == 0);
		Assert.Equal("https://httpstatuses.com/500", result["type"].GetString());
		Assert.Equal("An error occurred while processing your request.", result["title"].GetString());
		Assert.Equal(500, result["status"].GetInt32());
		Assert.Equal("GET /LocalOnly/Test/ThrowException", result["instance"].GetString());
		Assert.True(result["traceId"].GetString().IsUsable());
		Assert.False(result.ContainsKey("detail"));
		Assert.False(result.ContainsKey("stackTrace"));
		Assert.False(result.ContainsKey("exceptionType"));
		Assert.Equal(5, result.Count);
	}

	[Fact]
	public async Task WhenRequestWithInvalidDataAndTrueFormatReponseJsonThenReturn400AndFormattedJsonProblemDetails() {
		// Create a customized client for this test only
		var client = factory.WithWebHostBuilder(builder => {
			builder.UseEnvironment("test");

			// Override specific configuration keys for this test
			builder.UseSetting($"{nameof(DevelopmentConfig)}:{nameof(DevelopmentConfig.PrintDetailsPublicly)}", "true");
			builder.UseSetting($"{nameof(DevelopmentConfig)}:{nameof(DevelopmentConfig.WriteIndented)}", "true");
		}).CreateClient();

		// Now test with the origin you just allowed
		using var request = new HttpRequestMessage(HttpMethod.Post, "/LocalOnly/Test/InvalidModel") {
			Content = JsonContent.Create(new SampleModel(10, Guid.Empty, string.Empty, Microsoft.Extensions.Logging.LogLevel.Information)),
		};

		var response = await client.SendAsync(request);
		var json = await response.Content.ReadAsStringAsync();
		var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!;

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		Assert.NotEmpty(json);
		Assert.True(json.Count('\n') > 0);
		Assert.Equal("https://tools.ietf.org/html/rfc9110#section-15.5.1", result["type"].GetString());
		Assert.Equal("One or more validation errors occurred.", result["title"].GetString());
		Assert.Equal(400, result["status"].GetInt32());
		Assert.Equal("POST /LocalOnly/Test/InvalidModel", result["instance"].GetString());
		Assert.True(result["traceId"].GetString().IsUsable());
		Assert.Equal(1, result["errors"].GetPropertyCount());
		Assert.Equal(JsonValueKind.Object, result["errors"].ValueKind);
		Assert.True(result["errors"].TryGetProperty("ID", out _));
		Assert.Equal(1, result["errors"].GetProperty("ID").GetArrayLength());
		Assert.Equal("The field ID must be between 0 and 9.", result["errors"].GetProperty("ID")[0].GetString());
		Assert.Equal(6, result.Count);
	}

	[Fact]
	public async Task WhenRequestWithInvalidModelAndFalseFormatReponseJsonThenExpect400AndSingleLineJsonOutput() {
		// Create a customized client for this test only
		var client = factory.WithWebHostBuilder(builder => {
			builder.UseEnvironment("test");

			// Override specific configuration keys for this test
			builder.UseSetting($"{nameof(DevelopmentConfig)}:{nameof(DevelopmentConfig.PrintDetailsPublicly)}", "false");
			builder.UseSetting($"{nameof(DevelopmentConfig)}:{nameof(DevelopmentConfig.WriteIndented)}", "false");
		}).CreateClient();

		// Now test with the origin you just allowed
		using var request = new HttpRequestMessage(HttpMethod.Post, "/LocalOnly/Test/InvalidModel") {
			Content = new StringContent("{'ID': '10', GlobalID: '', Text: 12)}", MediaTypeHeaderValue.Parse("text/json")),
		};

		var response = await client.SendAsync(request);
		var json = await response.Content.ReadAsStringAsync();
		var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!;

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		Assert.NotEmpty(json);
		Assert.True(json.Count('\n') == 0);
		Assert.Equal("https://tools.ietf.org/html/rfc9110#section-15.5.1", result["type"].GetString());
		Assert.Equal("One or more validation errors occurred.", result["title"].GetString());
		Assert.Equal(400, result["status"].GetInt32());
		Assert.Equal("POST /LocalOnly/Test/InvalidModel", result["instance"].GetString());
		Assert.True(result["traceId"].GetString().IsUsable());
		Assert.Equal(2, result["errors"].GetPropertyCount());
		Assert.Equal(1, result["errors"].GetProperty("$").GetArrayLength());
		Assert.Equal("''' is an invalid start of a property name. Expected a '\"'. Path: $ | LineNumber: 0 | BytePositionInLine: 1.", result["errors"].GetProperty("$")[0].GetString());
		Assert.Equal(1, result["errors"].GetProperty("input").GetArrayLength());
		Assert.Equal("The input field is required.", result["errors"].GetProperty("input")[0].GetString());
		Assert.Equal(6, result.Count);
	}
}