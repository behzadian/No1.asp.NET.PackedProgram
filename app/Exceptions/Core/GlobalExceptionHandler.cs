using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using No1.Commons.Exceptions;
using No1.FaraBank.Api.Config;

namespace No1.FaraBank.Api.Exceptions.Core;

public class GlobalExceptionHandler(
	ILogger<GlobalExceptionHandler> logger,
	IProblemDetailsService problemDetailsService,
	IHostEnvironment environment
) : IExceptionHandler
{
	async ValueTask<bool> IExceptionHandler.TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken) {
		logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

		var statusCode = exception switch {
			BadHttpRequestException => StatusCodes.Status400BadRequest,
			UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
			HttpExceptionBase httpException => (int)httpException.Status,
			_ => StatusCodes.Status500InternalServerError,
		};

		httpContext.Response.StatusCode = statusCode;

		var problemDetails = new ProblemDetails {
			Status = statusCode,

			// Title & details are left null → .NET will fill them with default
			Type = $"https://httpstatuses.com/{statusCode}",
		};

		var configuration = NullExpressionException.Exec(() => httpContext.RequestServices).GetRequiredService<IConfiguration>();
		var config = DevelopmentConfig.Get(configuration);
		if (config.PrintDetailsPublicly && !environment.IsProduction()) {
			problemDetails.Extensions["stackTrace"] = exception.ToString()
				.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
				.Select((value, index) => KeyValuePair.Create($"{index + 1:D3}", value))
				.ToDictionary();
			problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
		}

		await problemDetailsService.TryWriteAsync(new ProblemDetailsContext {
			HttpContext = httpContext,
			ProblemDetails = problemDetails,
			Exception = exception,
		});

		return true;
	}
}