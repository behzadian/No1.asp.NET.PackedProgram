namespace No1.FaraBank.Api.Portables.Manager;

public record BankEnterpriseManagerInput(Guid? GlobalID, string Name, FileManagerInput Logo);