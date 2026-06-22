namespace No1.FaraBank.Api.Data.Shared;

public record class Sort<TEntity>(Func<TEntity> Property, SortDirection SortDirection);