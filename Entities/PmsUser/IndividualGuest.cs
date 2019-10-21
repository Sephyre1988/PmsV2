using System;
using System.Collections.Generic;
using Entities.PmsUser.Types;
using SimplePersistence.Model;

namespace Entities.PmsUser
{
	public class IndividualGuest : EntityWithAllMetaAndVersionAsLong<long>
	{
		public TreatmentType Treatment { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public StayType StayType { get; set; }
		public DateTimeOffset Birthdate { get; set; }
		public string Nationality { get; set; }
		public string Profession { get; set; }
		public bool NeedAssistance { get; set; }
		public string IdentityCard { get; set; }
		public string FiscalNumber { get; set; }
		public Passport Passport { get; set; }
		public Address Address { get; set; }
		public string Email { get; set; }
		public List<long> PhoneNumbers { get; set; }
		public double FidelityPoints { get; set; }

	}
}
