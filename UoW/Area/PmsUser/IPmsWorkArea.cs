using SimplePersistence.UoW;
using UoW.Repository.PmsUser;

namespace UoW.Area.PmsUser
{
	public interface IPmsWorkArea : IWorkArea
	{
		IAddressRepository Addresses { get; }
		ICollaboratorRepository Collaborators { get; }
		ICommercialGuestRepository CommercialGuests { get; }
		IIndividualGuestRepository IndividualGuests { get; }
		IMaintenanceJobRepository MaintenanceJobs { get; }
		IPassportRepository Passports { get; }
		IRoomRepository Rooms { get; }
	}
}