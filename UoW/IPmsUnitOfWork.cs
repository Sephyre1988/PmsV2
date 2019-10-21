using SimplePersistence.UoW;
using UoW.Area.PmsUser;

namespace UoW
{
	public interface IPmsUnitOfWork : IUnitOfWork

	{
		IPmsWorkArea PmsUser { get; }
	}
}
