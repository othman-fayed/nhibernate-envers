using System.Linq;
using System.Xml.Linq;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.Tools;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Metadata
{
	/// <summary>
	/// Generates metadata for to-one relations (reference-valued properties).
	/// </summary>
	public sealed class ToOneRelationMetadataGenerator
	{
		private readonly AuditMetadataGenerator _mainGenerator;

		public ToOneRelationMetadataGenerator(AuditMetadataGenerator auditMetadataGenerator)
		{
			_mainGenerator = auditMetadataGenerator;
		}

		public void AddToOne(XElement parent, PropertyAuditingData propertyAuditingData, IValue value,
					  ICompositeMapperBuilder mapper, string entityName, bool insertable)
		{
			var referencedEntityName = ((ToOne)value).ReferencedEntityName;

			var idMapping = _mainGenerator.GetReferencedIdMappingData(entityName, referencedEntityName,
					propertyAuditingData, true);

			var lastPropertyPrefix = MappingTools.CreateToOneRelationPrefix(propertyAuditingData.Name);

			// Generating the id mapper for the relation
			var relMapper = idMapping.IdMapper.PrefixMappedProperties(lastPropertyPrefix);

			// Storing information about this relation
			_mainGenerator.EntitiesConfigurations[entityName].AddToOneRelation(
					propertyAuditingData.Name, referencedEntityName, relMapper, insertable, MappingTools.IgnoreNotFound(value));

			// If the property isn't insertable, checking if this is not a "fake" bidirectional many-to-one relationship,
			// that is, when the one side owns the relation (and is a collection), and the many side is non insertable.
			// When that's the case and the user specified to store this relation without a middle table (using
			// @AuditMappedBy), we have to make the property insertable for the purposes of Envers. In case of changes to
			// the entity that didn't involve the relation, it's value will then be stored properly. In case of changes
			// to the entity that did involve the relation, it's the responsibility of the collection side to store the
			// proper data.
			bool nonInsertableFake;
			if (!insertable && propertyAuditingData.ForceInsertable)
			{
				nonInsertableFake = true;
				insertable = true;
			}
			else
			{
				nonInsertableFake = false;
			}


			// Adding an element to the mapping corresponding to the references entity id's
			var properties = new XElement(idMapping.XmlRelationMapping);
			properties.Add(new XAttribute("name", propertyAuditingData.Name));

			MetadataTools.PrefixNamesInPropertyElement(properties, lastPropertyPrefix,
						MetadataTools.GetColumnNameEnumerator(value.ColumnIterator),
						false, insertable, propertyAuditingData.AccessType);

			// Extracting related id properties from properties tag
			var firstJoin = firstJoinElement(parent);
			foreach (var element in properties.Elements())
			{
				if (firstJoin == null)
				{
					parent.Add(element);
				}
				else
				{
					firstJoin.AddBeforeSelf(element);
				}
			}

			// Adding mapper for the id
			var propertyData = propertyAuditingData.GetPropertyData();
			mapper.AddComposite(propertyData, new ToOneIdMapper(relMapper, propertyData, referencedEntityName, nonInsertableFake));
		}

		/// <summary>
		/// Mapping for a not-owning to-one relation throught property-ref
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="propertyAuditingData"></param>
		/// <param name="value"></param>
		/// <param name="mapper"></param>
		/// <param name="entityName"></param>
		/// <param name="insertable"></param>
		/// <exception cref="MappingException"></exception>
		public void AddToOnePropertyRef(
			XElement parent,
			PropertyAuditingData propertyAuditingData,
			IValue value,
			ICompositeMapperBuilder mapper,
			string entityName,
			bool insertable)
		{
			var propertyValue = (ToOne)value;
			var referencedEntityName = ((ToOne)value).ReferencedEntityName;
			var referencePropertyName = propertyValue.ReferencedPropertyName; // mappedBy

			//var idMapping = _mainGenerator.GetReferencedIdMappingData(entityName, referencedEntityName, propertyAuditingData, true);
			var configuration = _mainGenerator.EntitiesConfigurations[referencedEntityName];
			if (configuration == null)
			{
				throw new MappingException("An audited relation to a non-audited entity " + referencedEntityName + "!");
			}

			var relationDescription = configuration.GetRelationDescription(referencePropertyName);
			var key = configuration.PropertyMapper.Properties.Keys.SingleOrDefault(f => f.Name == referencePropertyName);
			if (key == null)
			{
				throw new MappingException("An audited relation to a non-audited entity " + entityName + "!");
			}

			var ownedIdMapping = configuration.IdMappingData;

			if (ownedIdMapping == null)
			{
				throw new MappingException("An audited relation to a non-audited entity " + entityName + "!");
			}

			var propertyRefMapping = configuration.PropertyMapper.Properties[key];

			if (propertyRefMapping == null)
			{
				throw new MappingException("An audited relation to a non-audited entity " + entityName + "!");
			}

			var lastPropertyPrefix = MappingTools.CreateToOneRelationPrefix(propertyAuditingData.Name);
			//var lastPropertyPrefix = MappingTools.CreateToOneRelationPrefix(referencePropertyName);
			//var referencedEntityName = propertyValue.ReferencedEntityName;

			// Generating the id mapper for the relation
			//var relMapper = propertyRefMapping.PrefixMappedProperties(lastPropertyPrefix);

			var propertyData = propertyAuditingData.GetPropertyData();
			//if (relMapper is SingleIdMapper singleIdMapper)
			//{
			//	relMapper = new SingleIdMapper(new Entities.PropertyData(
			//		propertyData.Name,
			//		referencePropertyName,
			//		propertyData.AccessType,
			//		propertyData.UsingModifiedFlag,
			//		propertyData.ModifiedFlagPropertyName
			//		));
			//}

			//var ownedIdMapper = propertyRefMapping.IdMapper.PrefixMappedProperties(lastPropertyPrefix);

			// Storing information about this relation
			//_mainGenerator.EntitiesConfigurations[entityName].AddToManyNotOwningRelation(
			//		propertyAuditingData.Name,
			//		referencePropertyName,
			//		referencedEntityName,
			//		ownedIdMapping.IdMapper,
			//		propertyRefMapping, //relMapper,
			//		null
			//		);
			_mainGenerator.EntitiesConfigurations[entityName].AddToOneNotOwningRelation(
					propertyAuditingData.Name,
					referencePropertyName,
					referencedEntityName,
					//ownedIdMapping,
					propertyRefMapping, //relMapper,
					insertable,
					MappingTools.IgnoreNotFound(value)
					);

			// If the property isn't insertable, checking if this is not a "fake" bidirectional many-to-one relationship,
			// that is, when the one side owns the relation (and is a collection), and the many side is non insertable.
			// When that's the case and the user specified to store this relation without a middle table (using
			// @AuditMappedBy), we have to make the property insertable for the purposes of Envers. In case of changes to
			// the entity that didn't involve the relation, it's value will then be stored properly. In case of changes
			// to the entity that did involve the relation, it's the responsibility of the collection side to store the
			// proper data.
			bool nonInsertableFake;
			if (!insertable && propertyAuditingData.ForceInsertable)
			{
				nonInsertableFake = true;
				insertable = true;
			}
			else
			{
				nonInsertableFake = false;
			}

			// Adding an element to the mapping corresponding to the references entity id's
			//var properties = new XElement(null!);
			////var properties = new XElement(idMapping.XmlRelationMapping);
			////var properties = new XElement(propertyRefMapping.XmlRelationMapping);
			//properties.Add(new XAttribute("name", propertyAuditingData.Name));

			//MetadataTools.PrefixNamesInPropertyElement(properties, lastPropertyPrefix,
			//			MetadataTools.GetColumnNameEnumerator(value.ColumnIterator),
			//			false, insertable, propertyAuditingData.AccessType);

			//// Extracting related id properties from properties tag
			//var firstJoin = firstJoinElement(parent);
			//foreach (var element in properties.Elements())
			//{
			//	if (firstJoin == null)
			//	{
			//		parent.Add(element);
			//	}
			//	else
			//	{
			//		firstJoin.AddBeforeSelf(element);
			//	}
			//}

			// Adding mapper for the id
			//var propertyData = propertyAuditingData.GetPropertyData();

			//mapper.AddComposite(propertyData, new ToOneIdMapper(null, propertyData, referencedEntityName, nonInsertableFake));

			mapper.AddComposite(propertyData, new ToOnePropertyRefMapper(propertyRefMapping, propertyData, referencedEntityName, referencePropertyName, nonInsertableFake));
			//mapper.AddComposite(propertyData, new OneToOneNotOwningMapper(entityName, referencedEntityName, owningReferencePropertyName, propertyData));
		}


		private static XElement firstJoinElement(XElement classElement)
		{
			//do we have to check for other elements than join here?
			return classElement.Element(MetadataTools.CreateElementName("join"));
		}

		public void AddOneToOneNotOwning(PropertyAuditingData propertyAuditingData, IValue value, ICompositeMapperBuilder mapper, string entityName)
		{
			var propertyValue = (OneToOne)value;
			var owningReferencePropertyName = propertyValue.ReferencedPropertyName; // mappedBy

			var configuration = _mainGenerator.EntitiesConfigurations[entityName];
			if (configuration == null)
			{
				throw new MappingException("An audited relation to a non-audited entity " + entityName + "!");
			}

			var ownedIdMapping = configuration.IdMappingData;

			if (ownedIdMapping == null)
			{
				throw new MappingException("An audited relation to a non-audited entity " + entityName + "!");
			}

			var lastPropertyPrefix = MappingTools.CreateToOneRelationPrefix(owningReferencePropertyName);
			var referencedEntityName = propertyValue.ReferencedEntityName;

			// Generating the id mapper for the relation
			var ownedIdMapper = ownedIdMapping.IdMapper.PrefixMappedProperties(lastPropertyPrefix);

			// Storing information about this relation
			_mainGenerator.EntitiesConfigurations[entityName].AddToOneNotOwningRelation(
					propertyAuditingData.Name, owningReferencePropertyName,
					referencedEntityName, ownedIdMapper, MappingTools.IgnoreNotFound(value));

			// Adding mapper for the id
			var propertyData = propertyAuditingData.GetPropertyData();
			mapper.AddComposite(propertyData, new OneToOneNotOwningMapper(entityName, referencedEntityName, owningReferencePropertyName, propertyData));
		}

		public void AddOneToOnePrimaryKeyJoinColumn(PropertyAuditingData propertyAuditingData, IValue value, ICompositeMapperBuilder mapper, string entityName, bool insertable)
		{
			var referencedEntityName = ((ToOne)value).ReferencedEntityName;
			var idMapping = _mainGenerator.GetReferencedIdMappingData(entityName, referencedEntityName, propertyAuditingData, true);
			var lastPropertyPrefix = MappingTools.CreateToOneRelationPrefix(propertyAuditingData.Name);

			// Generating the id mapper for the relation
			var relMapper = idMapping.IdMapper.PrefixMappedProperties(lastPropertyPrefix);

			// Storing information about this relation
			_mainGenerator.EntitiesConfigurations[entityName].AddToOneRelation(propertyAuditingData.Name, referencedEntityName, relMapper, insertable, MappingTools.IgnoreNotFound(value));

			// Adding mapper for the id
			var propertyData = propertyAuditingData.GetPropertyData();
			mapper.AddComposite(propertyData, new OneToOnePrimaryKeyJoinColumnMapper(entityName, referencedEntityName, propertyData));
		}
	}
}