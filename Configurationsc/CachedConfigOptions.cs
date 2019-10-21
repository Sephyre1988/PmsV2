using System;

namespace Configurations
{
	public class CachedConfigOptions
	{
		public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

		public TimeSpan? SlidingExpiration { get; set; }
	}
}