namespace Business.Contracts.Managers.Email
{
	public class EmailDestination
	{
		public long Id { get; set; }

		public string Address { get; set; }

		public EmailDestinationType EmailDestinationType { get; set; }
	}
}