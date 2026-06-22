namespace No1.FaraBank.Api.Shared;

public record LoggedInUser(Guid GlobalID, string? Name, string? Email, string? Phone);