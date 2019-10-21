namespace Business.Contracts.Managers.Email
{
	public class EmailParameter
	{
		public string Key { get; private set; }

		public string Value { get; private set; }

		public EmailParameter(string key, string value)
		{
			Key = key;
			Value = !string.IsNullOrEmpty(value) ? value : string.Empty;
		}
	}
}