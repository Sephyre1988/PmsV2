using System;
using System.Collections.Generic;

namespace Business.Contracts.Services
{
	public class EntityCollection<T>
	{
		public EntityCollection(IReadOnlyCollection<T> entities, int totalCount)
		{
			Entities = entities ?? throw new ArgumentNullException(nameof(entities));
			TotalCount = totalCount;
		}

		public IReadOnlyCollection<T> Entities { get; }

		public int TotalCount { get; }
	}
}