namespace Business.Contracts
{
	public abstract class Action<TId, TVersion> : IAction
	{
		public TId StateId { get; set; }

		public TVersion StateVersion { get; set; }
	}
}