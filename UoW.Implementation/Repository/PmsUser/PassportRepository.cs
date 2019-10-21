using Entities.PmsUser;
using System.Linq;
using UoW.Implementation.Mapping;
using UoW.Repository.PmsUser;

namespace UoW.Implementation.Repository.PmsUser
{
	public class PassportRepository : NHQueryableRepository<Passport, long>, IPassportRepository
	{
		public PassportRepository(IPmsDatabaseSession databaseSession) : base(databaseSession)
		{
		}

		public override IQueryable<Passport> QueryById(long id)
		{
			return Query().Where(e => e.Id == id);
		}
	}
}