using System;
using System.Collections.Generic;
using System.Text;
using Entities.PmsUser;
using SimplePersistence.UoW;

namespace UoW.Repository.PmsUser
{
	public interface IAddressRepository : IAsyncRepository<Address, long>
	{
	}
}
