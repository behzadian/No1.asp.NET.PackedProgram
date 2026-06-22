using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace No1.FaraBank.Api.Logging;

public class ActivityEnricher : ILogEventEnricher
{
	public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) {
		if (Activity.Current is null) {
			return;
		}

		logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", Activity.Current.TraceId.ToString()));

		logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", Activity.Current.SpanId.ToString()));
	}
}