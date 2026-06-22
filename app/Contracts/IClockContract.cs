using NodaTime;

namespace No1.FaraBank.Api.Contracts;

public interface IClockContract
{
	Instant Now(string? usage = null);
}