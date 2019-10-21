using Entities.PmsUser;
using System.Linq;
using UoW.Implementation.Mapping;
using UoW.Repository.PmsUser;

namespace UoW.Implementation.Repository.PmsUser
{
	public class RoomRepository : NHQueryableRepository<Room, long>, IRoomRepository
	{
		public RoomRepository(IPmsDatabaseSession databaseSession) : base(databaseSession)
		{
		}

		public override IQueryable<Room> QueryById(long id)
		{
			return Query().Where(e => e.Id == id);
		}
	}
}