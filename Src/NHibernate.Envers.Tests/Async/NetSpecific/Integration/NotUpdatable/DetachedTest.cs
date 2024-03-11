﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NHibernate.Envers.Tests.Integration.NotUpdatable;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.NotUpdatable
{
	using System.Threading.Tasks;
	public partial class DetachedUpdateTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			(await (AuditReader().GetRevisionsAsync(typeof(PropertyNotUpdatableEntity), id)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(1, 2);
		}
	}
}