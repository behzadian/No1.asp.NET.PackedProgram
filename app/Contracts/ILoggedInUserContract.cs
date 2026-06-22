using No1.FaraBank.Api.Shared;

namespace No1.FaraBank.Api.Contracts;

public interface ILoggedInUserContract
{
	LoggedInUser? GetLoggedInUser();
}