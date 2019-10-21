using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Contracts.Managers.Configurations
{
	public class SecuredHttpEndpointConfigurations : HttpEndpointConfigurations
	{
		public SecuredHttpEndpointConfigurations(string url, string userName, string password) : base(url)
		{
			if (url == null)
				throw new ArgumentNullException(nameof(url));
			if (userName == null)
				throw new ArgumentNullException(nameof(userName));
			if (password == null)
				throw new ArgumentNullException(nameof(password));
			if (string.IsNullOrWhiteSpace(url))
				throw new ArgumentException("Value cannot be whitespace.", nameof(url));
			if (string.IsNullOrWhiteSpace(userName))
				throw new ArgumentException("Value cannot be whitespace.", nameof(userName));
			if (string.IsNullOrWhiteSpace(password))
				throw new ArgumentException("Value cannot be whitespace.", nameof(password));

			UserName = userName;
			Password = password;
		}

		public string UserName { get; }

		public string Password { get; }
	}
}
