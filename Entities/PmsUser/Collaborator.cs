using SimplePersistence.Model;

namespace Entities.PmsUser
{
	public class Collaborator : EntityWithAllMetaAndVersionAsLong<long>
	{
		public string Name { get; set; }
		public string Role { get; set; }
		public string Email { get; set; }
		public Address Address { get; set; }
		public long MobilePhone { get; set; }
		public bool IsActive { get; set; }
		public bool IsAdmin { get; set; }
	}
}
