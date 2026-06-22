using JetBrains.Annotations;
using No1.FaraBank.Api.Repos.Conventions; // Install-Package Humanizer.Core

namespace No1.FaraBank.Api.Data.Conventions;

using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

[UsedImplicitly]
public class SnakeCaseColumnNameConvention : IPropertyConvention
{
	public void Apply(IPropertyInstance instance) {
		instance.Column(instance.Property.Name.SnakeCase());
	}
}