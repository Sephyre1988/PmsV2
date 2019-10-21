using System.Collections.Generic;
using SimplePersistence.Model;

namespace Entities.PmsUser
{
	public class CommercialGuest : EntityWithAllMetaAndVersionAsLong<long>
	{
		public string Name { get; set; }
		public List<IndividualGuest> GuestList { get; set; }
		public Address Address { get; set; }
		public List<long> PhoneNumbers { get; set; }

	}
}
