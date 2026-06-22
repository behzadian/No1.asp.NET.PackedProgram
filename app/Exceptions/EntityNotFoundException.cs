using No1.FaraBank.Api.Exceptions.Core;
using System.Net;

namespace No1.FaraBank.Api.Exceptions;

[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1128:Put constructor initializers on their own line", Justification = "Will be fixed in .editorconfig")]
public class EntityNotFoundException<TEntity> : HttpExceptionBase
{
	public EntityNotFoundException(Guid globalId) : base(HttpStatusCode.BadRequest, $"Entity of type {typeof(TEntity).Name} with GlobalID: {globalId} could not be found") {
	}

	public EntityNotFoundException(long id) : base(HttpStatusCode.BadRequest, $"Entity of type {typeof(TEntity).Name} with ID: {id} could not be found") {
	}
}