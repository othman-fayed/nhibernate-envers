using System.Collections.Generic;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Tools;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration
{
	/// <summary>
	/// A helper class holding auditing meta-data for all persistent classes.
	/// </summary>
	public class ClassesAuditingData
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(ClassesAuditingData));

		private readonly IDictionary<string, ClassAuditingData> entityNameToAuditingData = new Dictionary<string, ClassAuditingData>();
		private readonly IDictionary<PersistentClass, ClassAuditingData> persistentClassToAuditingData = new Dictionary<PersistentClass, ClassAuditingData>();

		/// <summary>
		/// Stores information about auditing meta-data for the given class.
		/// </summary>
		/// <param name="pc">Persistent class.</param>
		/// <param name="cad">Auditing meta-data for the given class.</param>
		public void AddClassAuditingData(PersistentClass pc, ClassAuditingData cad)
		{
			entityNameToAuditingData.Add(pc.EntityName, cad);
			persistentClassToAuditingData.Add(pc, cad);
		}

		/// <summary>
		/// A collection of all auditing meta-data for persistent classes.
		/// </summary>
		public IEnumerable<KeyValuePair<PersistentClass, ClassAuditingData>> AllClassAuditedData => persistentClassToAuditingData;

		/// <summary>
		/// After all meta-data is read, updates calculated fields. This includes:
		/// <ul>
		/// <li>setting {@code forceInsertable} to {@code true} for properties specified by {@code @AuditMappedBy}</li> 
		/// </ul>
		/// </summary>
		public void UpdateCalculatedFields()
		{
			foreach (var classAuditingDataEntry in persistentClassToAuditingData)
			{
				var pc = classAuditingDataEntry.Key;
				var classAuditingData = classAuditingDataEntry.Value;
				foreach (var propertyName in classAuditingData.PropertyNames)
				{
					// Replaces below statments
					//updateCalculatedProperty(pc, classAuditingData, propertyName);

					string referencedEntityName = null;
					ClassAuditingData referencedClassAuditingData = null;
					var property = pc.GetProperty(propertyName);

					var propertyAuditingData = classAuditingData.GetPropertyAuditingData(propertyName);
					// If a property had the @AuditMappedBy annotation, setting the referenced fields to be always insertable.
					if (propertyAuditingData.ForceInsertable)
					{
						referencedEntityName = MappingTools.ReferencedEntityName(property.Value);
						referencedClassAuditingData = entityNameToAuditingData[referencedEntityName];
						forcePropertyInsertable(referencedClassAuditingData, propertyAuditingData.MappedBy,
								pc.EntityName, referencedEntityName);

						forcePropertyInsertable(referencedClassAuditingData, propertyAuditingData.PositionMappedBy,
								pc.EntityName, referencedEntityName);
					}

					//if (property.Value is List propertyValueAsList
					//   && propertyAuditingData.MappedBy != null
					//   && propertyAuditingData.PositionMappedBy == null)
					//{
					//	referencedEntityName = referencedEntityName ?? MappingTools.ReferencedEntityName(property.Value);

					//	// If a property has mappedBy= and @Indexed and isn't @AuditMappedBy, add synthetic support.
					//	addSyntheticIndexProperty(propertyValueAsList, property.PropertyAccessorName, entityNameToAuditingData[referencedEntityName]);
					//}
				}
			}
		}
		private static void forcePropertyInsertable(ClassAuditingData classAuditingData, string propertyName,
											 string entityName, string referencedEntityName)
		{
			if (propertyName != null)
			{
				if (classAuditingData.GetPropertyAuditingData(propertyName) == null)
				{
					throw new MappingException("@AuditMappedBy points to a property that doesn't exist: " +
						referencedEntityName + "." + propertyName);
				}
				log.Debug("Non-insertable property {0}, {1} will be made insertable because a matching @AuditMappedBy was found in the {2} entity.",
					referencedEntityName, propertyName, entityName);

				classAuditingData
						.GetPropertyAuditingData(propertyName)
						.ForceInsertable = true;
			}
		}

		private static void addSyntheticIndexProperty(List value, string propertyAccessorName, ClassAuditingData classAuditingData)
		{
			var indexValue = value.Index;
			if (indexValue != null)
			{
				foreach (var column in indexValue.ColumnIterator)
				{
					var indexColumnName = column.Text;
					if (indexColumnName != null)
					{
						var auditingData = new PropertyAuditingData(indexColumnName, propertyAccessorName, true, indexValue);
						classAuditingData.AddPropertyAuditingData(indexColumnName, auditingData);
					}
				}
			}
		}

	}
}
