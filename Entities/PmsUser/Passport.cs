using System;
using SimplePersistence.Model;

namespace Entities.PmsUser
{
	public class Passport : EntityWithAllMetaAndVersionAsLong<long>
	{
		public string PassportNumber { get; set; }
		public string EmittingCountry { get; set; }
		public DateTimeOffset EmittingDate { get; set; }
		public DateTimeOffset ValidityDate { get; set; }

	}
}
