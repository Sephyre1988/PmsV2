using Entities.PmsUser;
using SimplePersistence.UoW;

namespace UoW.Repository.PmsUser
{
	public interface ICollaboratorRepository : IAsyncRepository<Collaborator, long>
	{
		
	}
}