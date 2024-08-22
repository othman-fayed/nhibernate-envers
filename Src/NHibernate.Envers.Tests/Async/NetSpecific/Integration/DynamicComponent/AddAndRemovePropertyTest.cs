﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.DynamicComponent
{
	using System.Threading.Tasks;
	public partial class AddAndRemovePropertyTest : TestBase
	{


		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			(await (AuditReader().GetRevisionsAsync(typeof (DynamicTestEntity), id)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(1, 2, 3);
		}

		[Test]
		public async Task VerifyHistoryAsync()
		{
			var rev1 = await (AuditReader().FindAsync<DynamicTestEntity>(id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<DynamicTestEntity>(id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<DynamicTestEntity>(id, 3)).ConfigureAwait(false);

			rev1.Properties.Count.Should().Be.EqualTo(1);
			rev2.Properties.Count.Should().Be.EqualTo(1);
			rev3.Properties.Should().Be.Null();

			rev1.Properties["Name"].Should().Be.EqualTo("1");
			rev2.Properties["Name"].Should().Be.EqualTo("2");
		}
	}
}