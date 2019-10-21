using Entities.PmsUser;
using System.Linq;
using UoW.Implementation.Mapping;
using UoW.Repository.PmsUser;

namespace UoW.Implementation.Repository.PmsUser
{
	public class MaintenanceJobRepository : NHQueryableRepository<MaintenanceJob, long>, IMaintenanceJobRepository
	{
		public MaintenanceJobRepository(IPmsDatabaseSession databaseSession) : base(databaseSession)
		{
		}

		public override IQueryable<MaintenanceJob> QueryById(long id)
		{
			return Query().Where(e => e.Id == id);
		}
	}
}