namespace No1.FaraBank.Api.Data.Shared;

public record class Pagination<TEntity>(int Index, int Size, IList<Sort<TEntity>> Sorts)
{
	public int StartRowIndex => Math.Max(this.Index - 1, 0) * this.Size;
}