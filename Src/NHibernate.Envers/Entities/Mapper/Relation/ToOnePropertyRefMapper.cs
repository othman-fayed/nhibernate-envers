using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Query;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Query;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Properties;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	[Serializable]
	public partial class ToOnePropertyRefMapper : AbstractToOneMapper
	{
		private readonly IIdMapper _propertyMapper;
		private readonly IPropertyMapper _propertyRefMapping;
		private readonly string _referencedEntityName;
		private readonly bool _nonInsertableFake;
		private readonly string _referencedPropertyName;

		public ToOnePropertyRefMapper(
							IIdMapper propertyMapper,
							IPropertyMapper referencedPropertyMapper,
							PropertyData propertyData,
							string referencedEntityName,
							string referencedPropertyName,
							bool nonInsertableFake
			)
			: base(propertyData)
		{
			_propertyMapper = propertyMapper;
			_propertyRefMapping = referencedPropertyMapper;
			_referencedEntityName = referencedEntityName;
			_nonInsertableFake = nonInsertableFake;
			_referencedPropertyName = referencedPropertyName;
		}

		public override bool MapToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj)
		{
			var newData = new Dictionary<string, object>();

			// If this property is originally non-insertable, but made insertable because it is in a many-to-one "fake"
			// bi-directional relation, we always store the "old", unchaged data, to prevent storing changes made
			// to this field. It is the responsibility of the collection to properly update it if it really changed.
			//_delegat.MapToMapFromEntity(newData, _nonInsertableFake ? oldObj : newObj);

			_propertyRefMapping.MapToMapFromEntity(
				session,
				newData,
				newObj,
				_nonInsertableFake ? oldObj : newObj);

			// Whatever we add here will be used when we insert a new revision record
			foreach (var entry in newData)
			{
				data[entry.Key] = entry.Value;
			}

			return checkModified(session, newObj, oldObj);

		}

		public override void MapModifiedFlagsToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj)
		{
			if (PropertyData.UsingModifiedFlag)
			{
				data[PropertyData.ModifiedFlagPropertyName] = checkModified(session, newObj, oldObj);
			}
		}

		public override void MapModifiedFlagsToMapForCollectionChange(string collectionPropertyName, IDictionary<string, object> data)
		{
			if (PropertyData.UsingModifiedFlag)
			{
				data[PropertyData.ModifiedFlagPropertyName] = collectionPropertyName.Equals(PropertyData.Name);
			}
		}

		private bool checkModified(ISessionImplementor session, object newObj, object oldObj)
		{
			return !_nonInsertableFake && !Toolz.EntitiesEqual(session, newObj, oldObj);
		}

		protected override void NullSafeMapToEntityFromMap(
			AuditConfiguration verCfg,
			object obj,
			IDictionary data,
			object primaryKey,
			IAuditReaderImplementor versionsReader,
			long revision)
		{
			//_propertyRefMapping.MapToEntityFromMap(
			//	verCfg,
			//	obj,
			//	data,
			//	primaryKey,
			//	versionsReader,
			//	revision
			//	);

			//return versionsReader.CreateQuery().ForEntitiesAtRevision(referencedEntity.EntityClass, revision)
			//	.Add(AuditEntity.RelatedId(_owningReferencePropertyName).Eq(primaryKey))
			//	.GetSingleResult();
			var entityPropertyReference = _propertyMapper.MapToIdFromMap(data);
			//PropertyData.Name;
			object value = null;
			if (entityPropertyReference != null)
			{
				//if (!versionsReader.FirstLevelCache.(_referencedEntityName, revision, entityPropertyReference, out value))
				//{
				var referencedEntity = GetEntityInfo(verCfg, _referencedEntityName);
				//var ignoreNotFound = false;
				//if (!referencedEntity.IsAudited)
				//{
				//	var referencingEntityName = verCfg.EntCfg.GetEntityNameForVersionsEntityName((string)data["$type$"]);
				//	var relation = verCfg.EntCfg.GetRelationDescription(referencingEntityName, PropertyData.Name);
				//	ignoreNotFound = relation != null && relation.IsIgnoreNotFound;
				//}
				//var removed = RevisionType.Deleted.Equals(data[verCfg.AuditEntCfg.RevisionTypePropName]);

				//value = ignoreNotFound ?
				//	ToOneEntityLoader.LoadImmediate(versionsReader, _referencedEntityName, entityPropertyReference, revision, removed, verCfg) :
				//	ToOneEntityLoader.CreateProxyOrLoadImmediate(versionsReader, _referencedEntityName, entityPropertyReference, revision, removed, verCfg);
				//}

				value = versionsReader
					.CreateQuery()
					.ForEntitiesAtRevision(referencedEntity.EntityClass, revision)
					.Add(AuditEntity.Property(_referencedPropertyName).Eq(entityPropertyReference))
					.GetSingleResult();

				if (value is null)
				{
					value = versionsReader
					.CreateQuery()
					.ForEntitiesAtRevision(referencedEntity.EntityClass, revision)
					.Add(AuditEntity.Property(_referencedPropertyName).Eq(entityPropertyReference))
					.GetSingleResult();

					//if (value is null)
					//{

					//	var revision1 = versionsReader
					//		.CreateQuery()
					//	 .ForRevisionsOfEntity(referencedEntity.EntityClass, false, true)
					//	 .AddProjection(AuditEntity.RevisionNumber().Max())
					//	 .Add(AuditEntity.Property(_referencedPropertyName).Eq(entityPropertyReference))
					//	 .Add(AuditEntity.RevisionNumber().Le(revision))
					//	 .GetSingleResult();

					//	if (revision1 != null)
					//	{
					//		var revNo = Convert.ToInt64(revision1);
					//		value = versionsReader
					//		.CreateQuery()
					//		.ForEntitiesAtRevision(referencedEntity.EntityClass, revNo)
					//		.Add(AuditEntity.Property(_referencedPropertyName).Eq(entityPropertyReference))
					//		.GetSingleResult();
					//	}
					//}

				}
			}
			SetPropertyValue(obj, value);
		}

		public void AddMiddleEqualToQuery(Parameters parameters, string prefix1, string prefix2)
		{
			//_delegat.AddIdsEqualToQuery(parameters, prefix1, _delegat, prefix2);
		}

		protected override Task NullSafeMapToEntityFromMapAsync(AuditConfiguration verCfg, object obj, IDictionary data, object primaryKey, IAuditReaderImplementor versionsReader, long revision, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
	}
}
