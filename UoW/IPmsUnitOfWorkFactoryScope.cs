using System;

namespace UoW
{
	public interface IPmsUnitOfWorkFactoryScope : IDisposable
	{
		IPmsUnitOfWork UnitOfWork { get; }
	}
}