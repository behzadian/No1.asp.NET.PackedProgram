using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using JetBrains.Annotations;

namespace No1.FaraBank.Api.Data.Conventions;

[UsedImplicitly]
public class SnakeCaseIdConvention : IIdConvention
{
	public void Apply(IIdentityInstance instance) {
		instance.Column("id"); // or instance.Property.Name.Underscore()
	}
}