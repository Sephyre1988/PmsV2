using Entities.PmsUser;
using SimplePersistence.UoW;

namespace UoW.Repository.PmsUser
{
	public interface IRoomRepository : IAsyncRepository<Room, long>
	{
		
	}
}