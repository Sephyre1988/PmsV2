using Entities.PmsUser;
using System.Linq;
using UoW.Implementation.Mapping;
using UoW.Repository.PmsUser;

namespace UoW.Implementation.Repository.PmsUser
{
	public class IndividualGuestRepository : NHQueryableRepository<IndividualGuest, long>, IIndividualGuestRepository
	{
		public IndividualGuestRepository(IPmsDatabaseSession databaseSession) : base(databaseSession)
		{
		}

		public override IQueryable<IndividualGuest> QueryById(long id)
		{
			return Query().Where(e => e.Id == id);
		}
	}
}