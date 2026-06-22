using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using No1.FaraBank.Api.LocalOnly;
using No1.SerilogTestSink;
using Serilog.Events;
using test.Core;

namespace test.Tests;

public class LoggingTests : IClassFixture<IntegrationTestFactory>
{
	private readonly HttpClient client;

	public LoggingTests(IntegrationTestFactory factory) {
		ArgumentNullException.ThrowIfNull(factory);
		TestSerilogSink.LogEvents.Clear();
		this.client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("test")).CreateClient();
	}

	[Fact]
	public async Task WhenTraceMessageLogPostedThenLoggerSkips() {
		// Arrange
		using var request = new HttpRequestMessage(HttpMethod.Get, $"/LocalOnly/Test/{nameof(TestController.Log)}/?level={LogLevel.Trace}");

		// Act
		_ = await this.client.SendAsync(request);

		// Assert
		Assert.False(TestSerilogSink.LogEvents.IsEmpty);
		Assert.DoesNotContain(TestSerilogSink.LogEvents, x => x.Match<TestLogTemplates>(nameof(TestLogTemplates.LogTrace)));
	}

	[Fact]
	public async Task WhenDebugMessageLogPostedThenLoggerSkips() {
		// Arrange
		using var request = new HttpRequestMessage(HttpMethod.Get, $"/LocalOnly/Test/{nameof(TestController.Log)}/?level={LogLevel.Debug}");

		// Act
		_ = await this.client.SendAsync(request);

		// Assert
		Assert.False(TestSerilogSink.LogEvents.IsEmpty);
		Assert.DoesNotContain(TestSerilogSink.LogEvents, x => x.Match<TestLogTemplates>(nameof(TestLogTemplates.LogDebug)));
	}

	[Fact]
	public async Task WhenInformationMessageLogPostedThenLoggerPersist() {
		// Arrange
		using var request = new HttpRequestMessage(HttpMethod.Get, $"/LocalOnly/Test/{nameof(TestController.Log)}/?level={LogLevel.Information}");

		// Act
		_ = await this.client.SendAsync(request);

		// Assert
		Assert.Contains(TestSerilogSink.LogEvents, x => x.Match<TestLogTemplates>(nameof(TestLogTemplates.LogInformation)));
	}

	[Fact]
	public async Task WhenWarningMessageLogPostedThenLoggerPersist() {
		// Arrange
		using var request = new HttpRequestMessage(HttpMethod.Get, $"/LocalOnly/Test/{nameof(TestController.Log)}/?level={LogLevel.Warning}");

		// Act
		_ = await this.client.SendAsync(request);

		// Assert
		Assert.Contains(TestSerilogSink.LogEvents, x => x.Match<TestLogTemplates>(nameof(TestLogTemplates.LogWarning)));
	}

	[Fact]
	public async Task WhenErrorMessageLogPostedThenLoggerPersist() {
		// Arrange
		using var request = new HttpRequestMessage(HttpMethod.Get, $"/LocalOnly/Test/{nameof(TestController.Log)}/?level={LogLevel.Error}");

		// Act
		_ = await this.client.SendAsync(request);

		// Assert
		Assert.Contains(TestSerilogSink.LogEvents, x => x.Match<TestLogTemplates>(nameof(TestLogTemplates.LogError)));
	}

	[Fact]
	public async Task WhenCriticalMessageLogPostedThenLoggerPersist() {
		// Arrange
		using var request = new HttpRequestMessage(HttpMethod.Get, $"/LocalOnly/Test/{nameof(TestController.Log)}/?level={LogLevel.Critical}");

		// Act
		_ = await this.client.SendAsync(request);

		// Assert
		Assert.Contains(TestSerilogSink.LogEvents, x => x.Match<TestLogTemplates>(nameof(TestLogTemplates.LogCritical)));
	}

	[Fact]
	public async Task WhenAnEndpointGetCalledThenRequestWillBeLogged() {
		// Arrange
		using var request = new HttpRequestMessage(HttpMethod.Get, $"/LocalOnly/Test/{nameof(TestController.Log)}/?level={LogLevel.Information}");

		// Act
		_ = await this.client.SendAsync(request);

		// Assert
		Assert.Contains(TestSerilogSink.LogEvents, x => x.MessageTemplate.Text == "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms");
	}

	[Fact]
	public async Task WhenAnEndpointThrowsExceptionThenExceptionWillBeLogged() {
		// Arrange
		using var request = new HttpRequestMessage(HttpMethod.Get, "/LocalOnly/Test/" + nameof(TestController.ThrowException));

		// Act
		_ = await this.client.SendAsync(request);

		// Assert
		Assert.Contains(TestSerilogSink.LogEvents, x => x.Level == LogEventLevel.Error && x.Exception?.GetType() == typeof(Exception));
	}
}