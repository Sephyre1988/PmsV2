using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.PmsUser;
using SimplePersistence.UoW.NH;
using UoW.Implementation.Mapping;
using UoW.Repository.PmsUser;

namespace UoW.Implementation.Repository.PmsUser
{
	public class DomainRepository : NHQueryableRepository<Domain, long>, IDomainRepository
	{
		public DomainRepository(IPmsDatabaseSession databaseSession) : base(databaseSession)
		{
		}

		public override IQueryable<Domain> QueryById(long id)
		{
			return Query().Where(e => e.Id == id);
		}

		public async Task<IReadOnlyCollection<Domain>> GetAllByNameAsync(string name, CancellationToken ct)
		{
			return await Task.Run(() =>
			{
				return Query().Where(e => e.Name == name).ToList();

			}, ct);
		}

		public async Task<Domain> GetByDomainAndValueAsync(string domain, string value, CancellationToken ct)
		{
			return await Task.Run(() =>
			{
				// ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
				return Query().Where(e => e.Value == value && e.Name == domain).FirstOrDefault();
			}, ct);
		}
		public async Task<IReadOnlyCollection<Domain>> GetByNameListAsync(IEnumerable<string> names, CancellationToken ct)
		{
			return await Task.Run(() =>
			{
				return Query().Where(d => names.Contains(d.Name)).ToArray();
			}, ct);
		}

		public async Task<Domain> GetByNameAsync(string name, CancellationToken ct)
		{
			return await Task.Run(() =>
			{
				return Query().SingleOrDefault(d => d.Name == name);
			}, ct);
		}
	}
}