using SimplePersistence.UoW.NH;
using UoW.Area.PmsUser;
using UoW.Implementation.Mapping;

namespace UoW.Implementation
{
	public class PmsUnitOfWork : NHUnitOfWork, IPmsUnitOfWork
	{
		public PmsUnitOfWork(IPmsDatabaseSession session, IPmsWorkArea pmsUser) : base(session)
		{
			PmsUser = pmsUser;
		}

		public IPmsWorkArea PmsUser { get; }
	}
}