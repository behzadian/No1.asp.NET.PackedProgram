using System.Text.RegularExpressions;

namespace No1.FaraBank.Api.Repos.Conventions;

public static partial class NamingConventionsConvertor
{
	public static string SnakeCase(this string name) {
		return WordPattern().Replace(name, "$1_$2").ToLower();
	}

	[GeneratedRegex(@"([a-z\d])([A-Z])")]
	private static partial Regex WordPattern();
}