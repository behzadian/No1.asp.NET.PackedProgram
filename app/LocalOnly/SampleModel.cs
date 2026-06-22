using System.ComponentModel.DataAnnotations;

namespace No1.FaraBank.Api.LocalOnly;

public record SampleModel([Range(0, 9)] long ID, Guid GlobalID, string Text, LogLevel LogLevel);