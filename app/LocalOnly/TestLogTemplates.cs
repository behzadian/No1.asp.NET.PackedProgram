using NodaTime;

namespace No1.FaraBank.Api.LocalOnly;

internal partial class TestLogTemplates
{
	protected TestLogTemplates() {
	}

	[LoggerMessage(Level = LogLevel.Trace, Message = "LogTrace is being called at {instant} with message `{message}`")]
	internal static partial void LogTrace(ILogger logger, Instant instant, string message);

	[LoggerMessage(Level = LogLevel.Debug, Message = "LogDebug is being called at {instant} with message `{message}`")]
	internal static partial void LogDebug(ILogger logger, Instant instant, string message);

	[LoggerMessage(Level = LogLevel.Information, Message = "LogInformation is being called at {instant} with message `{message}`")]
	internal static partial void LogInformation(ILogger logger, Instant instant, string message);

	[LoggerMessage(Level = LogLevel.Warning, Message = "LogWarning is being called at {instant} with message `{message}`")]
	internal static partial void LogWarning(ILogger logger, Instant instant, string message);

	[LoggerMessage(Level = LogLevel.Error, Message = "LogError is being called at {instant} with message `{message}`")]
	internal static partial void LogError(ILogger logger, Instant instant, string message);

	[LoggerMessage(Level = LogLevel.Critical, Message = "LogCritical is being called at {instant} with message `{message}`")]
	internal static partial void LogCritical(ILogger logger, Instant instant, string message);
}