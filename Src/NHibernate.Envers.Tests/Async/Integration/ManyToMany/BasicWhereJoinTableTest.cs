﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.ManyToMany;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.ManyToMany
{
	using System.Threading.Tasks;
	public partial class BasicWhereJoinTableTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 4 }, await (AuditReader().GetRevisionsAsync(typeof(WhereJoinTableEntity), wjte1_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1, 3, 4 }, await (AuditReader().GetRevisionsAsync(typeof(WhereJoinTableEntity), wjte2_id)).ConfigureAwait(false));

			CollectionAssert.AreEquivalent(new[] { 1 }, await (AuditReader().GetRevisionsAsync(typeof(IntNoAutoIdTestEntity), ite1_1_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1 }, await (AuditReader().GetRevisionsAsync(typeof(IntNoAutoIdTestEntity), ite1_2_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1 }, await (AuditReader().GetRevisionsAsync(typeof(IntNoAutoIdTestEntity), ite2_1_id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 1 }, await (AuditReader().GetRevisionsAsync(typeof(IntNoAutoIdTestEntity), ite2_2_id)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOfWjte1Async()
		{
			var ite1_1 = await (Session.GetAsync<IntNoAutoIdTestEntity>(ite1_1_id)).ConfigureAwait(false);
			var ite2_1 = await (Session.GetAsync<IntNoAutoIdTestEntity>(ite2_1_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<WhereJoinTableEntity>(wjte1_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<WhereJoinTableEntity>(wjte1_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<WhereJoinTableEntity>(wjte1_id, 3)).ConfigureAwait(false);
			var rev4 = await (AuditReader().FindAsync<WhereJoinTableEntity>(wjte1_id, 4)).ConfigureAwait(false);

			// Checking 1st list
			CollectionAssert.IsEmpty(rev1.References1);
			CollectionAssert.AreEquivalent(new[] { ite1_1 }, rev2.References1);
			CollectionAssert.AreEquivalent(new[] { ite1_1 }, rev3.References1);
			CollectionAssert.IsEmpty(rev4.References1);

			// Checking 2nd list
			CollectionAssert.IsEmpty(rev1.References2);
			CollectionAssert.AreEquivalent(new[] { ite2_1 }, rev2.References2);
			CollectionAssert.AreEquivalent(new[] { ite2_1 }, rev3.References2);
			CollectionAssert.AreEquivalent(new[] { ite2_1 }, rev4.References2);
		}

		[Test]
		public async Task VerifyHistoryOfWjte2Async()
		{
			var ite1_1 = await (Session.GetAsync<IntNoAutoIdTestEntity>(ite1_1_id)).ConfigureAwait(false);
			var ite1_2 = await (Session.GetAsync<IntNoAutoIdTestEntity>(ite1_2_id)).ConfigureAwait(false);
			var ite2_2 = await (Session.GetAsync<IntNoAutoIdTestEntity>(ite2_2_id)).ConfigureAwait(false);

			var rev1 = await (AuditReader().FindAsync<WhereJoinTableEntity>(wjte2_id, 1)).ConfigureAwait(false);
			var rev2 = await (AuditReader().FindAsync<WhereJoinTableEntity>(wjte2_id, 2)).ConfigureAwait(false);
			var rev3 = await (AuditReader().FindAsync<WhereJoinTableEntity>(wjte2_id, 3)).ConfigureAwait(false);
			var rev4 = await (AuditReader().FindAsync<WhereJoinTableEntity>(wjte2_id, 4)).ConfigureAwait(false);

			// Checking 1st list
			CollectionAssert.IsEmpty(rev1.References1);
			CollectionAssert.IsEmpty(rev2.References1);
			CollectionAssert.AreEquivalent(new[] { ite1_1, ite1_2 }, rev3.References1);
			CollectionAssert.AreEquivalent(new[] { ite1_1, ite1_2 }, rev4.References1);

			// Checking 2nd list
			CollectionAssert.IsEmpty(rev1.References2);
			CollectionAssert.IsEmpty(rev2.References2);
			CollectionAssert.IsEmpty(rev3.References2);
			CollectionAssert.AreEquivalent(new[] { ite2_2 }, rev4.References2);
		}
	}
}