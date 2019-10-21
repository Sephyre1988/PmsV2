namespace Business.Contracts
{
	public abstract class ActionResult<TId, TVersion> : IActionResult
	{
		public TId StateId { get; set; }

		public TVersion StateVersion { get; set; }
	}
}