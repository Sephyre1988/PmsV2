using Entities.PmsUser;
using SimplePersistence.UoW;

namespace UoW.Repository.PmsUser
{
	public interface IMaintenanceJobRepository : IAsyncRepository<MaintenanceJob, long>
	{
		
	}
}