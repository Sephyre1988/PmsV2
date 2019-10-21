using Entities.PmsUser;
using SimplePersistence.UoW;

namespace UoW.Repository.PmsUser
{
	public interface IDomainRepository : IAsyncRepository<Domain, long>
	{
		
	}
}