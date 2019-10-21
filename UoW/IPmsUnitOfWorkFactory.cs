namespace UoW
{
	public interface IPmsUnitOfWorkFactory
	{
		IPmsUnitOfWorkFactoryScope GetScope();
	}
}