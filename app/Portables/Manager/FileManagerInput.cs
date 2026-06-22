using System.ComponentModel.DataAnnotations;

namespace No1.FaraBank.Api.Portables.Manager;

public record FileManagerInput(Guid? GlobalID, IFormFile? File, [Required] bool Delete);