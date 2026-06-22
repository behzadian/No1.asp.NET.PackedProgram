namespace No1.FaraBank.Api.Data.Shared;

public class Paged<T>
{
	public IList<T> Items { get; set; } = [];

	public int Index { get; set; }

	public int Size { get; set; }

	public int? Count { get; set; }
}