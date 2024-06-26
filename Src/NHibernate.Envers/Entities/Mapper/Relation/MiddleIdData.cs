﻿using System;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Tools;
using NHibernate.Mapping;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	/// <summary>
	///  A class holding information about ids, which form a virtual "relation" from a middle-table. Middle-tables are used
	///  when mapping collections.
	/// </summary>
	[Serializable]
	public sealed class MiddleIdData
	{
		public MiddleIdData(AuditEntitiesConfiguration verEntCfg, IdMappingData mappingData, Property mappedByProperty, string entityName, bool audited)
		{
			OriginalMapper = mappingData.IdMapper;
			if (mappedByProperty.Type.IsAssociationType)
			{
				PrefixedMapper = mappingData.IdMapper.PrefixMappedProperties(mappedByProperty.Name + MappingTools.RelationCharacter);
			}
			else
			{
				var singleIdMapper = mappingData.IdMapper as SingleIdMapper;
				PrefixedMapper = singleIdMapper.SetName("LegOrSectorID");	// HACK
			}
			EntityName = entityName;
			AuditEntityName = audited ? verEntCfg.GetAuditEntityName(entityName) : null;
		}

		public MiddleIdData(AuditEntitiesConfiguration verEntCfg, IdMappingData mappingData, string prefix,
							string entityName, bool audited)
		{
			OriginalMapper = mappingData.IdMapper;
			PrefixedMapper = mappingData.IdMapper.PrefixMappedProperties(prefix);
			EntityName = entityName;
			AuditEntityName = audited ? verEntCfg.GetAuditEntityName(entityName) : null;
		}

		public IIdMapper OriginalMapper { get; }
		public IIdMapper PrefixedMapper { get; }
		public string EntityName { get; }
		public string AuditEntityName { get; }

		/// <summary>
		/// Is the entity, to which this middle id data correspond, audited.
		/// </summary>
		/// <returns></returns>
		public bool IsAudited()
		{
			return AuditEntityName != null;
		}
	}
}
