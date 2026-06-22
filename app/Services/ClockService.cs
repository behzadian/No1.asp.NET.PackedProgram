using No1.FaraBank.Api.Contracts;
using NodaTime;

namespace No1.FaraBank.Api.Services;

public class ClockService : IClockContract
{
	Instant IClockContract.Now(string? usage) => SystemClock.Instance.GetCurrentInstant();
}