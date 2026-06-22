using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using No1.FaraBank.Api.Models;
using No1.FaraBank.Api.Repos.Conventions;
using No1.NHibernateNodaTime;

namespace No1.FaraBank.Api.Data.Overrides;

public class Overrides : IAutoMappingOverride<BankAccountEntity>, IAutoMappingOverride<BankEnterpriseEntity>, IAutoMappingOverride<FileEntity>
{
	void IAutoMappingOverride<BankAccountEntity>.Override(AutoMapping<BankAccountEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, NamingConventionsConvertor.SnakeCase);
	}

	void IAutoMappingOverride<BankEnterpriseEntity>.Override(AutoMapping<BankEnterpriseEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, NamingConventionsConvertor.SnakeCase);
	}

	void IAutoMappingOverride<FileEntity>.Override(AutoMapping<FileEntity> mapping) {
		NodaTimeUtility.OverrideEntity(mapping, NamingConventionsConvertor.SnakeCase);
	}
}