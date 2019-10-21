using System;

namespace Business.Contracts.Exceptions
{
	public class UnauthorizedException : Exception
	{
		public UnauthorizedException(string message) : base(message)
		{

		}
	}
}
