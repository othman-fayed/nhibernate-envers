﻿using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
	public class ClassAuditingData : IAuditedPropertiesHolder
	{
		private readonly IDictionary<string, PropertyAuditingData> _properties;

		public ClassAuditingData()
		{
			_properties = new Dictionary<string, PropertyAuditingData>();
			JoinTableDictionary = new Dictionary<string, string>();
		}

		//called SecondaryTableDictionary in Hibernate Envers
		public IDictionary<string, string> JoinTableDictionary { get; }
		public AuditTableAttribute AuditTable { get; set; }
		public AuditFactoryAttribute Factory { get; set; }

		/// <summary>
		///  True if the class is audited globally (this helps to cover the cases when there are no fields in the class,
		///  but it's still audited).
		/// </summary>
		private bool _defaultAudited;

		public void AddPropertyAuditingData(string propertyName, PropertyAuditingData auditingData)
		{
			_properties.Add(propertyName, auditingData);
		}

		public PropertyAuditingData GetPropertyAuditingData(string propertyName)
		{
			return _properties.TryGetValue(propertyName, out var ret) ? ret : null;
		}

		public bool IsEmpty()
		{
			return _properties.Count == 0;
		}

		public bool Contains(string propertyName)
		{
			return _properties.ContainsKey(propertyName);
		}

		public IEnumerable<string> PropertyNames => _properties.Keys;

		public void SetDefaultAudited(bool defaultAudited)
		{
			_defaultAudited = defaultAudited;
		}

		public bool IsAudited()
		{
			return _defaultAudited || _properties.Count > 0;
		}

		public IEnumerable<PropertyAuditingData> SynthenticProperties()
		{
			return _properties.Values.Where(x => x.IsSynthetic);
		}
	}
}
