using System.Net;

namespace No1.FaraBank.Api.Exceptions.Core;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3376:Attribute, EventArgs, and Exception type names should end with the type being extended", Justification = "Keep")]
public abstract class HttpExceptionBase : Exception
{
	protected HttpExceptionBase(HttpStatusCode status, string? message = null) : base(message ?? string.Empty) {
		this.Status = status;
	}

	protected HttpExceptionBase(HttpStatusCode status, string? message, Exception? innerException) : base(message, innerException) {
		this.Status = status;
	}

	public HttpStatusCode Status { get; }
}