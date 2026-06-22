using System.Diagnostics.CodeAnalysis;

namespace No1.FaraBank.Api.Controllers.Core;

public enum Audiences
{
	/// <summary>
	/// Users who are OWNER of the application and has role `God`. They can do anything. Is not usable now.
	/// </summary>
	[SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed", Justification = "_")]
	[Obsolete("Do not use now.")]
	God = 0,

	/// <summary>
	/// Users who are manager of application and has role `Manager`. They can access to all resources.
	/// </summary>
	Manager = 10,

	/// <summary>
	/// Users who are manager of application and has role `Manager`. They can access to all resources.
	/// </summary>
	[SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed", Justification = "_")]
	[Obsolete("Do not use now.")]
	Operator = 20,

	/// <summary>
	/// Users who are customer of the application and has not specific role. They can only access to the resources they own.
	/// </summary>
	Owner = 30,

	/// <summary>
	/// .
	/// </summary>
	Public = 99,
}