using Entities.PmsUser;
using System.Linq;
using UoW.Implementation.Mapping;
using UoW.Repository.PmsUser;

namespace UoW.Implementation.Repository.PmsUser
{
	public class AddressRepository : NHQueryableRepository<Address, long>, IAddressRepository
	{
		public AddressRepository(IPmsDatabaseSession databaseSession) : base(databaseSession) { }

		public override IQueryable<Address> QueryById(long id)
		{
			return Query().Where(e => e.Id == id);

		}
	}
}