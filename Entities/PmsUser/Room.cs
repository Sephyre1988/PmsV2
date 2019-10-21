using System.Collections.Generic;
using SimplePersistence.Model;

namespace Entities.PmsUser
{
	public class Room : EntityWithAllMetaAndVersionAsLong<long>
	{
		public int RoomNumber { get; set; }
		public long BedCount { get; set; }
		public string RoomName { get; set; }
		public List<IndividualGuest> GuestsIn { get; set; }
		public bool NeedsClean { get; set; }
		public bool NeedsMaintenance { get; set; }
		public bool IsVacant { get; set; }
	}
}
