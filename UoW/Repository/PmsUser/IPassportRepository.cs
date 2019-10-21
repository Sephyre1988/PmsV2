using Entities.PmsUser;
using SimplePersistence.UoW;

namespace UoW.Repository.PmsUser
{
	public interface IPassportRepository : IAsyncRepository<Passport, long>
	{
		
	}
}