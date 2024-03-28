using NHibernate.Criterion;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy;
using NHibernate.Envers.Query;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	public static partial class ToOneEntityLoader
	{
		public static object LoadImmediate(IAuditReaderImplementor versionsReader, string entityName,
			object entityId, long revision, bool removed, AuditConfiguration verCfg)
		{
			if (verCfg.EntCfg.GetNotVersionEntityConfiguration(entityName) == null)
			{
				// Audited relation, look up entity with Envers.
				// When user traverses removed entities graph, do not restrict revision type of referencing objects
				// to ADD or MOD (DEL possible). See HHH-5845.
				return versionsReader.Find(entityName, entityId, revision, removed);
			}
			// Not audited relation, look up entity with Hibernate.
			return versionsReader.SessionImplementor.ImmediateLoad(entityName, entityId);
		}

		public static object CreateProxyOrLoadImmediate(IAuditReaderImplementor versionsReader, string entityName,
			object entityId, long revision, bool removed, AuditConfiguration verCfg)
		{
			var persister = versionsReader.SessionImplementor.Factory.GetEntityPersister(entityName);
			if (persister.HasProxy)
			{
				return persister.CreateProxy(entityId,
					new ToOneDelegateSessionImplementor(versionsReader, entityName, entityId, revision, removed, verCfg));
			}

			return LoadImmediate(versionsReader, entityName, entityId, revision, removed, verCfg);
		}

		public static object LoadImmediate(
			IAuditReaderImplementor versionsReader,
			string entityName,
			string entityPropertyRef,
			object propertyValue,
			long revision,
			bool removed,
			AuditConfiguration verCfg)
		{
			if (verCfg.EntCfg.GetNotVersionEntityConfiguration(entityName) == null)
			{
				// Audited relation, look up entity with Envers.
				// When user traverses removed entities graph, do not restrict revision type of referencing objects
				// to ADD or MOD (DEL possible). See HHH-5845.
				return versionsReader
					.CreateQuery()
					.ForEntitiesAtRevision(entityName, revision)
					.Add(AuditEntity.Property(entityPropertyRef).Eq(propertyValue))
					.GetSingleResult();
			}
			// Not audited relation, look up entity with Hibernate.
			var query = versionsReader.Session.CreateCriteria(entityName);
			query.Add(Restrictions.Eq(Projections.Property(entityPropertyRef), propertyValue));

			return query.UniqueResult();
		}

		public static object CreateProxyOrLoadImmediate(
			IAuditReaderImplementor versionsReader,
			string entityName,
			string entityPropertyRef,
			object propertyValue,
			long revision,
			bool removed,
			AuditConfiguration verCfg)
		{
			var persister = versionsReader.SessionImplementor.Factory.GetEntityPersister(entityName);
			if (persister.HasProxy)
			{
				// Get the entity persister
				//IEntityPersister persister = session.SessionFactory.GetClassMetadata(typeof(MyEntity)) as IEntityPersister;

				// Create the proxy manually
				//MyEntity proxy = per.GetProxy(typeof(MyEntity), new LazyInitializer(persister, entity.Id, session));

				// TODO

				//return persister.CreateProxy(entityId,
				//	new ToOneDelegateSessionImplementor(versionsReader, entityName, entityId, revision, removed, verCfg));
			}

			return LoadImmediate(versionsReader, entityName, entityPropertyRef, propertyValue, revision, removed, verCfg);
		}

	}
}