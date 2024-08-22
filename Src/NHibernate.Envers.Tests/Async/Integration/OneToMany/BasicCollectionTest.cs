﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.OneToMany;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany
{
	using System.Threading.Tasks;
	public partial class BasicCollectionTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, await (AuditReader().GetRevisionsAsync(typeof(CollectionRefEdEntity), ed1_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, await (AuditReader().GetRevisionsAsync(typeof(CollectionRefEdEntity), ed2_id)).ConfigureAwait(false));

			CollectionAssert.AreEquivalent(new[] { 1, 2 }, await (AuditReader().GetRevisionsAsync(typeof(CollectionRefIngEntity), ing1_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1, 3 }, await (AuditReader().GetRevisionsAsync(typeof(CollectionRefIngEntity), ing2_id)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOfEd1Async()
		{
			var ing1 = await (Session.GetAsync<CollectionRefIngEntity>(ing1_id)).ConfigureAwait(false);
			var ing2 = await (Session.GetAsync<CollectionRefIngEntity>(ing2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<CollectionRefEdEntity>(ed1_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<CollectionRefEdEntity>(ed1_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<CollectionRefEdEntity>(ed1_id, 3)).ConfigureAwait(false);

			CollectionAssert.AreEquivalent(new[] {ing1, ing2}, rev1.Reffering);
			CollectionAssert.AreEquivalent(new[] { ing2 }, rev2.Reffering);
			CollectionAssert.IsEmpty(rev3.Reffering);
		}

		[Test]
		public async Task VerifyHistoryOfEd2Async()
		{
			var ing1 = await (Session.GetAsync<CollectionRefIngEntity>(ing1_id)).ConfigureAwait(false);
			var ing2 = await (Session.GetAsync<CollectionRefIngEntity>(ing2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<CollectionRefEdEntity>(ed2_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<CollectionRefEdEntity>(ed2_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<CollectionRefEdEntity>(ed2_id, 3)).ConfigureAwait(false);

			CollectionAssert.IsEmpty(rev1.Reffering);
			CollectionAssert.AreEquivalent(new[] { ing1 }, rev2.Reffering);
			CollectionAssert.AreEquivalent(new[] { ing2, ing1 }, rev3.Reffering);
		}

		[Test]
		public async Task VerifyHistoryOfIng1Async()
		{
			var ed1 = await (Session.GetAsync<CollectionRefEdEntity>(ed1_id)).ConfigureAwait(false);
			var ed2 = await (Session.GetAsync<CollectionRefEdEntity>(ed2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<CollectionRefIngEntity>(ing1_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<CollectionRefIngEntity>(ing1_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<CollectionRefIngEntity>(ing1_id, 3)).ConfigureAwait(false);

			Assert.AreEqual(ed1, rev1.Reference);
			Assert.AreEqual(ed2, rev2.Reference);
			Assert.AreEqual(ed2, rev3.Reference);
		}

		[Test]
		public async Task VerifyHistoryOfIng2Async()
		{
			var ed1 = await (Session.GetAsync<CollectionRefEdEntity>(ed1_id)).ConfigureAwait(false);
			var ed2 = await (Session.GetAsync<CollectionRefEdEntity>(ed2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<CollectionRefIngEntity>(ing2_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<CollectionRefIngEntity>(ing2_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<CollectionRefIngEntity>(ing2_id, 3)).ConfigureAwait(false);

			Assert.AreEqual(ed1, rev1.Reference);
			Assert.AreEqual(ed1, rev2.Reference);
			Assert.AreEqual(ed2, rev3.Reference);
		}
	}
}