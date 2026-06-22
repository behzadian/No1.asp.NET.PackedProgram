using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using No1.Commons.Exceptions;
using No1.EnvBasedEndpoints;
using No1.FaraBank.Api.Contracts;
using NodaTime;

namespace No1.FaraBank.Api.LocalOnly;

[Route("LocalOnly/Test")]
[ApiController]
[NonProduction]
[AllowAnonymous]
public class TestController(ILogger<TestController> logger, IClockContract clock) : ControllerBase
{
	[HttpGet(nameof(ThrowException))]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S112:General or reserved exceptions should never be thrown", Justification = "Keep")]
	public void ThrowException() => throw new Exception("Exception Message");

	[HttpPost(nameof(InvalidModel))]
	public SampleModel InvalidModel(SampleModel input) => input;

	[HttpGet(nameof(Log))]
	public SampleModel Log(LogLevel level, string message = "") {
		Instant now = clock.Now();
		switch (level) {
			case LogLevel.Trace:
				TestLogTemplates.LogTrace(logger, now, message);
				break;
			case LogLevel.Debug:
				TestLogTemplates.LogDebug(logger, now, message);
				break;
			case LogLevel.Information:
				TestLogTemplates.LogInformation(logger, now, message);
				break;
			case LogLevel.Warning:
				TestLogTemplates.LogWarning(logger, now, message);
				break;
			case LogLevel.Error:
				TestLogTemplates.LogError(logger, now, message);
				break;
			case LogLevel.Critical:
				TestLogTemplates.LogCritical(logger, now, message);
				break;
			case LogLevel.None:
			default:
				throw new UnsupportedEnumException<LogLevel>(level);
		}

		return new SampleModel(0, Guid.Empty, "Text", level);
	}
}