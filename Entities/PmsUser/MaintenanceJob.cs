using System;
using Entities.PmsUser.Types;
using SimplePersistence.Model;

namespace Entities.PmsUser
{
	public class MaintenanceJob : EntityWithAllMetaAndVersionAsLong<long>
	{
		public long CommodityId { get; set; }
		public CommodityType CommodityType { get; set; }
		public ItemType ItemType { get; set; }
		public bool IsResolved { get; set; }
		public string Description { get; set; }
		public DateTimeOffset StarTime { get; set; }
		public Collaborator Collaborator { get; set; }
	}
}
