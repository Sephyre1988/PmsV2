using NHibernate;
using SimplePersistence.UoW.NH;

namespace UoW.Implementation.Mapping
{
	public class PmsDatabaseSession : DatabaseSession, IPmsDatabaseSession
	{
		public PmsDatabaseSession(ISession session) : base(session)
		{
		}
	}
}