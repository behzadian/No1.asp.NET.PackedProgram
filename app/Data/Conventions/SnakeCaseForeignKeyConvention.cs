using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using JetBrains.Annotations;
using No1.FaraBank.Api.Repos.Conventions;

namespace No1.FaraBank.Api.Data.Conventions;

[UsedImplicitly]
public class SnakeCaseForeignKeyConvention : IReferenceConvention
{
	public void Apply(IManyToOneInstance instance) {
		instance.Column(instance.Property.Name.SnakeCase() + "_id");
	}
}