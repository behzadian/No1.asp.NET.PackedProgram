namespace No1.FaraBank.Api.Controllers.Core;

public abstract record VersionedAudiencedEndpoint(int Version, Audiences Audience)
{
	internal static readonly VersionedAudiencedEndpoint[] Versions = [Version1Manager.Instance, Version1Owner.Instance, Version1Public.Instance,];

	public string UrlPrefix => $"v{this.Version}/{this.Audience}";

	public string NamespaceKeyPeriodSurrounded => $".Version{this.Version}.";

	public string Name => $"Version{this.Version}{this.Audience}";

	public sealed record Version1Manager : VersionedAudiencedEndpoint
	{
		public const string Prefix = $"v1/{nameof(Audiences.Manager)}";

		public static readonly Version1Manager Instance = new();

		private Version1Manager() : base(1, Audiences.Manager) {
		}
	}

	public sealed record Version1Owner : VersionedAudiencedEndpoint
	{
		public const string Prefix = $"v1/{nameof(Audiences.Owner)}";

		public static readonly Version1Owner Instance = new();

		private Version1Owner() : base(1, Audiences.Owner) {
		}
	}

	public sealed record Version1Public : VersionedAudiencedEndpoint
	{
		public const string Prefix = $"v1/{nameof(Audiences.Public)}";

		public static readonly Version1Public Instance = new();

		private Version1Public() : base(1, Audiences.Public) {
		}
	}
}