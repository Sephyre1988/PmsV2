using SimplePersistence.Model;

namespace Entities.PmsUser
{
	public class Address : EntityWithAllMetaAndVersionAsLong<long>
	{
		public string Street { get; set; }
		public int PostalCode { get; set; }
		public string District { get; set; }
		public string Parish { get; set; }
		public string Door { get; set; }
		public string County { get; set; }
		public string Country { get; set; }
	}
}
