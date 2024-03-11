﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.RevInfo
{
	using System.Threading.Tasks;
	public partial class ChangeListenerInstanceTest : TestBase
	{

		[Test]
		public async Task EntityShouldHaveBeenPersistedAsync()
		{
			(await (Session.CreateQuery("select count(s) from StrTestEntity s where s.Str='x'").UniqueResultAsync<long>()).ConfigureAwait(false))
				.Should().Be.EqualTo(1);
		}
	}
}