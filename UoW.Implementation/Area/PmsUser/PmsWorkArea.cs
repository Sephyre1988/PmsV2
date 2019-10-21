using SimplePersistence.UoW.NH;
using UoW.Area.PmsUser;
using UoW.Repository.PmsUser;

namespace UoW.Implementation.Area.PmsUser
{
	public class PmsWorkArea : NHWorkArea, IPmsWorkArea
	{
		public PmsWorkArea(
			IDatabaseSession databaseSession,
			IAddressRepository addresses,
			ICollaboratorRepository collaborators,
			ICommercialGuestRepository commercialGuests,
			IIndividualGuestRepository individualGuests,
			IMaintenanceJobRepository maintenanceJobs,
			IPassportRepository passports,
			IRoomRepository rooms) : base(databaseSession)
		{
			Addresses = addresses;
			Collaborators = collaborators;
			CommercialGuests = commercialGuests;
			IndividualGuests = individualGuests;
			MaintenanceJobs = maintenanceJobs;
			Passports = passports;
			Rooms = rooms;
		}

		public IAddressRepository Addresses { get; }
		public ICollaboratorRepository Collaborators { get; }
		public ICommercialGuestRepository CommercialGuests { get; }
		public IIndividualGuestRepository IndividualGuests { get; }
		public IMaintenanceJobRepository MaintenanceJobs { get; }
		public IPassportRepository Passports { get; }
		public IRoomRepository Rooms { get; }
	}
}