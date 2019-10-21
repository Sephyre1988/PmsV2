using Entities.PmsUser;
using System.Linq;
using UoW.Implementation.Mapping;
using UoW.Repository.PmsUser;

namespace UoW.Implementation.Repository.PmsUser
{
	public class CommercialGuestRepository : NHQueryableRepository<CommercialGuest, long>, ICommercialGuestRepository
	{
		public CommercialGuestRepository(IPmsDatabaseSession databaseSession) : base(databaseSession)
		{
		}

		public override IQueryable<CommercialGuest> QueryById(long id)
		{
			return Query().Where(e => e.Id == id);
		}
	}
}