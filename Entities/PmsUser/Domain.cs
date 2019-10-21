using SimplePersistence.Model;

namespace Entities.PmsUser
{
	public class Domain : EntityWithSoftDelete<long>
	{
		public virtual string Name { get; set; }

		public virtual string Value { get; set; }

		public virtual string Meaning { get; set; }

		public virtual long? Ts { get; set; }

		public virtual string Description { get; set; }

		public virtual long? NextId { get; set; }
	}
}