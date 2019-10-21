namespace Business.Contracts.Managers.Configurations
{
	public class DomainConfiguration
	{
		public DomainConfiguration(long id, string value, string description)
		{
			Id = id;
			Value = value;
			Description = description;
		}

		public long Id { get; }

		public string Value { get; set; }

		public string Description { get; }
	}
}