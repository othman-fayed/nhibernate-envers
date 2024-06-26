﻿using System;

namespace NHibernate.Envers.Entities
{
	[Serializable]
	public class PropertyData
	{
		/// <summary>
		/// Copies the given property data, except the name.
		/// </summary>
		/// <param name="newName">New name</param>
		/// <param name="propertyData">Property data to copy the rest of properties from.</param>
		public PropertyData(string newName, PropertyData propertyData)
		{
			Name = newName;
			BeanName = propertyData.BeanName;
			AccessType = propertyData.AccessType;
		}

		/// <summary>
		/// </summary>
		/// <param name="name">Name of the property.</param>
		/// <param name="beanName">Name of the property in the bean.</param>
		/// <param name="accessType">Accessor type for this property.</param>
		public PropertyData(string name, string beanName, string accessType)
		{
			Name = name;
			BeanName = beanName;
			AccessType = accessType;
		}

		/// <param name="name">Name of the property.</param>
		/// <param name="beanName">Name of the property in the bean.</param>
		/// <param name="accessType">Accessor type for this property.</param>
		/// <param name="usingModifiedFlag">Defines if field changes should be tracked</param>
		/// <param name="modifiedFlagName"></param>
		public PropertyData(string name, string beanName, string accessType, bool usingModifiedFlag, string modifiedFlagName, bool isSynthentic = false)
			: this(name, beanName, accessType)
		{
			UsingModifiedFlag = usingModifiedFlag;
			ModifiedFlagPropertyName = modifiedFlagName;
			IsSynthentic = isSynthentic;
		}

		/// <summary>
		/// Name of the property.
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Name of the property in the bean.
		/// </summary>
		public string BeanName { get; }
		/// <summary>
		/// Accessor type for this property.
		/// </summary>
		public string AccessType { get; }
		/// <summary>
		/// Defines if field changes should be tracked
		/// </summary>
		public bool UsingModifiedFlag { get; }
		public string ModifiedFlagPropertyName { get; }
		public bool IsSynthentic { get; }

		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			if (obj == null || GetType() != obj.GetType()) return false;

			var that = (PropertyData)obj;

			if (!AccessType?.Equals(that.AccessType) ?? that.AccessType != null) return false;
			if (!BeanName?.Equals(that.BeanName) ?? that.BeanName != null) return false;
			if (!Name?.Equals(that.Name) ?? that.Name != null) return false;
			if (UsingModifiedFlag != that.UsingModifiedFlag) return false;
			if (IsSynthentic != that.IsSynthentic) return false;
			return true;
		}

		public override int GetHashCode()
		{
			var result = Name?.GetHashCode() ?? 0;
			result = 31 * result + (BeanName?.GetHashCode() ?? 0);
			result = 31 * result + (AccessType?.GetHashCode() ?? 0);
			result = 31 * result + (UsingModifiedFlag ? 1 : 0);
			result = 31 * result + (IsSynthentic ? 1 : 0);
			return result;
		}
	}
}
