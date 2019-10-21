using Entities.PmsUser;
using System.Linq;
using UoW.Implementation.Mapping;
using UoW.Repository.PmsUser;

namespace UoW.Implementation.Repository.PmsUser
{
	public class CollaboratorRepository : NHQueryableRepository<Collaborator, long>, ICollaboratorRepository
	{
		public CollaboratorRepository(IPmsDatabaseSession databaseSession) : base(databaseSession)
		{
		}

		public override IQueryable<Collaborator> QueryById(long id)
		{
			return Query().Where(e => e.Id == id);
		}
	}
}