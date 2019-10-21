using System;

namespace Business.Contracts.Managers.Configurations
{
	public class HttpEndpointConfigurations
	{
		public HttpEndpointConfigurations(string url)
		{
			if (url == null)
				throw new ArgumentNullException(nameof(url));
			if (string.IsNullOrWhiteSpace(url))
				throw new ArgumentException("Value cannot be whitespace.", nameof(url));

			Url = url;
		}

		public string Url { get; }
	}
}
