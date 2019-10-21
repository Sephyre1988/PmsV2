using Entities.PmsUser;
using SimplePersistence.UoW;

namespace UoW.Repository.PmsUser
{
	public interface IIndividualGuestRepository : IAsyncRepository<IndividualGuest,long>
	{
		
	}
}