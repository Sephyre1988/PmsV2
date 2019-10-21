using Entities.PmsUser;
using SimplePersistence.UoW;

namespace UoW.Repository.PmsUser
{
	public interface ICommercialGuestRepository : IAsyncRepository<CommercialGuest, long>
	{
		
	}
}