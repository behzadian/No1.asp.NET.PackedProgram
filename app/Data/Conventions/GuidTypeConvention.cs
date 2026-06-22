using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using JetBrains.Annotations;

namespace No1.FaraBank.Api.Data.Conventions;

[UsedImplicitly]
public class GuidTypeConvention : IUserTypeConvention
{
	public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria) {
		criteria.Expect(x => x.Property.PropertyType == typeof(Guid) || x.Property.PropertyType == typeof(Guid?));
	}

	public void Apply(IPropertyInstance instance) {
		instance.CustomSqlType("uuid");
	}
}