using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using Humanizer;
using No1.FaraBank.Api.Repos.Conventions;

namespace No1.FaraBank.Api.Data.Conventions;

public class TableNameConvention : IClassConvention
{
	public void Apply(IClassInstance instance) {
		string entityName = instance.EntityType.Name[..^"Entity".Length];
		string entityPlural = entityName.Pluralize();
		string tableName = entityPlural.SnakeCase();
		instance.Table(tableName);
	}
}