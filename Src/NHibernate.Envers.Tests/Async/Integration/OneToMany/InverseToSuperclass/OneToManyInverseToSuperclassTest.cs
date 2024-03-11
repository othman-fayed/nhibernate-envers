﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.OneToMany.InverseToSuperclass
{
	using System.Threading.Tasks;
	public partial class OneToManyInverseToSuperclassTest : TestBase
	{

		[Test]
		public async Task HistoryShouldExistAsync()
		{
			var rev1 = await (AuditReader().FindAsync<Master>(masterId, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<Master>(masterId, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<Master>(masterId, 3)).ConfigureAwait(false);
			var rev4 = await (AuditReader().FindAsync<Master>(masterId, 4)).ConfigureAwait(false);

			rev1.Should().Not.Be.Null();
			rev2.Should().Not.Be.Null();
			rev3.Should().Not.Be.Null();
			rev4.Should().Not.Be.Null();
		}
	}
}