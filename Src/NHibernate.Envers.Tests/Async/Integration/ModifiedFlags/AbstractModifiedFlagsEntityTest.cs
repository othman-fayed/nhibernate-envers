﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	using System.Threading.Tasks;
	public abstract partial class AbstractModifiedFlagsEntityTest : TestBase
	{

		protected Task<IList> QueryForPropertyHasChangedAsync(System.Type type, object id, params string[] propertyNames)
		{
			try
			{
				var query = createForRevisionQuery(type, id, false);
				addHasChangedProperties(query, propertyNames);
				return query.GetResultListAsync();
			}
			catch (System.Exception ex)
			{
				return Task.FromException<IList>(ex);
			}
		}

		protected Task<IList> QueryForPropertyHasChangedWithDeletedAsync(System.Type type, object id, params string[] propertyNames)
		{
			try
			{
				var query = createForRevisionQuery(type, id, true);
				addHasChangedProperties(query, propertyNames);
				return query.GetResultListAsync();
			}
			catch (System.Exception ex)
			{
				return Task.FromException<IList>(ex);
			}
		}

		protected Task<IList> QueryForPropertyHasNotChangedAsync(System.Type type, object id, params string[] propertyNames)
		{
			try
			{
				var query = createForRevisionQuery(type, id, false);
				addHasNotChangedProperties(query, propertyNames);
				return query.GetResultListAsync();
			}
			catch (System.Exception ex)
			{
				return Task.FromException<IList>(ex);
			}
		}

		protected Task<IList> QueryForPropertyHasNotChangedWithDeletedAsync(System.Type type, object id, params string[] propertyNames)
		{
			try
			{
				var query = createForRevisionQuery(type, id, true);
				addHasNotChangedProperties(query, propertyNames);
				return query.GetResultListAsync();
			}
			catch (System.Exception ex)
			{
				return Task.FromException<IList>(ex);
			}
		}
	}
}