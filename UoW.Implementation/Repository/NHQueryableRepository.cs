using NHibernate;
using NHibernate.Linq;
using SimplePersistence.UoW.Exceptions;
using SimplePersistence.UoW.NH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace UoW.Implementation.Repository
{
	public abstract class NHQueryableRepository<TEntity, TKey> : INHRepository<TEntity, TKey>
	where TEntity : class
	{
		protected NHQueryableRepository(ISession session)
		{
			if (session == null) throw new ArgumentNullException(nameof(session));

			Session = session;
		}

		protected NHQueryableRepository(IDatabaseSession databaseSession)
		{
			if (databaseSession == null) throw new ArgumentNullException(nameof(databaseSession));

			Session = databaseSession.Session;
		}

		#region Implementation of IAsyncRepository<TEntity,in TKey>

		public async Task<TEntity> GetByIdAsync(TKey id)
		{
			return await GetByIdAsync(id, CancellationToken.None);
		}

		public async Task<TEntity> GetByIdAsync(TKey id, CancellationToken ct)
		{
			if (id == null) throw new ArgumentNullException(nameof(id));

			return await Task.Run(() => GetById(id), ct);
		}

		public async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = new CancellationToken())
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			Session.Save(entity);
			await FlushAsync(ct);
			return entity;
		}

		public async Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities, CancellationToken ct = new CancellationToken())
		{
			if (entities == null) throw new ArgumentNullException(nameof(entities));

			return await AddAsync(ct, entities as TEntity[] ?? entities.ToArray());
		}

		public async Task<IEnumerable<TEntity>> AddAsync(params TEntity[] entities)
		{
			return await AddAsync(CancellationToken.None, entities);
		}

		public async Task<IEnumerable<TEntity>> AddAsync(CancellationToken ct, params TEntity[] entities)
		{
			if (entities == null) throw new ArgumentNullException(nameof(entities));
			if (entities.Length == 0)
				return entities;

			foreach (var entity in entities)
				Session.Save(entity);
			await FlushAsync(ct);
			return entities;
		}

		public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct = new CancellationToken())
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			Session.Update(entity);
			await FlushAsync(ct);
			return entity;
		}

		public async Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken ct = new CancellationToken())
		{
			if (entities == null) throw new ArgumentNullException(nameof(entities));

			return await UpdateAsync(ct, entities as TEntity[] ?? entities.ToArray());
		}

		public async Task<IEnumerable<TEntity>> UpdateAsync(params TEntity[] entities)
		{
			return await UpdateAsync(CancellationToken.None, entities);
		}

		public async Task<IEnumerable<TEntity>> UpdateAsync(CancellationToken ct, params TEntity[] entities)
		{
			if (entities == null) throw new ArgumentNullException(nameof(entities));
			if (entities.Length == 0)
				return entities;

			foreach (var entity in entities)
				Session.Update(entity);
			await FlushAsync(ct);
			return entities;
		}

		public async Task<TEntity> DeleteAsync(TEntity entity, CancellationToken ct = new CancellationToken())
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			Session.Delete(entity);
			await FlushAsync(ct);
			return entity;
		}

		public async Task<IEnumerable<TEntity>> DeleteAsync(IEnumerable<TEntity> entities, CancellationToken ct = new CancellationToken())
		{
			if (entities == null) throw new ArgumentNullException(nameof(entities));

			return await DeleteAsync(ct, entities as TEntity[] ?? entities.ToArray());
		}

		public async Task<IEnumerable<TEntity>> DeleteAsync(params TEntity[] entities)
		{
			return await DeleteAsync(CancellationToken.None, entities);
		}

		public async Task<IEnumerable<TEntity>> DeleteAsync(CancellationToken ct, params TEntity[] entities)
		{
			if (entities == null) throw new ArgumentNullException(nameof(entities));
			if (entities.Length == 0)
				return entities;

			foreach (var entity in entities)
				Session.Delete(entity);
			await FlushAsync(ct);
			return entities;
		}

		public async Task<long> TotalAsync(CancellationToken ct = new CancellationToken())
		{
			return await Task.Run(() => Total(), ct);
		}

		public async Task<bool> ExistsAsync(TKey id, CancellationToken ct = new CancellationToken())
		{
			if (id == null) throw new ArgumentNullException(nameof(id));

			return await Task.Run(() => Exists(id), ct);
		}

		#endregion

		#region Implementation of ISyncRepository<TEntity,in TKey>

		public TEntity GetById(TKey id)
		{
			if (id == null) throw new ArgumentNullException(nameof(id));

			return Session.Get<TEntity>(id);
		}

		public TEntity Add(TEntity entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			Session.Save(entity);
			Flush();
			return entity;
		}

		public IEnumerable<TEntity> Add(IEnumerable<TEntity> entities)
		{
			if (entities == null) throw new ArgumentNullException(nameof(entities));

			return Add(entities as TEntity[] ?? entities.ToArray());
		}

		public IEnumerable<TEntity> Add(params TEntity[] entities)
		{
			if (entities == null) throw new ArgumentNullException(nameof(entities));
			if (entities.Length == 0)
				return entities;

			foreach (var entity in entities)
				Session.Save(entity);
			Flush();
			return entities;
		}

		public TEntity Update(TEntity entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			Session.Update(entity);
			Flush();
			return entity;
		}

		public IEnumerable<TEntity> Update(IEnumerable<TEntity> entities)
		{
			if (entities == null) throw new ArgumentNullException(nameof(entities));

			return Update(entities as TEntity[] ?? entities.ToArray());
		}

		public IEnumerable<TEntity> Update(params TEntity[] entities)
		{
			if (entities == null) throw new ArgumentNullException(nameof(entities));
			if (entities.Length == 0)
				return entities;

			foreach (var entity in entities)
				Session.Update(entity);
			Flush();
			return entities;
		}

		public TEntity Delete(TEntity entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			Session.Delete(entity);
			Flush();
			return entity;
		}

		public IEnumerable<TEntity> Delete(IEnumerable<TEntity> entities)
		{
			if (entities == null) throw new ArgumentNullException(nameof(entities));

			return Delete(entities as TEntity[] ?? entities.ToArray());
		}

		public IEnumerable<TEntity> Delete(params TEntity[] entities)
		{
			if (entities == null) throw new ArgumentNullException(nameof(entities));
			if (entities.Length == 0)
				return entities;

			foreach (var entity in entities)
				Session.Delete(entity);
			Flush();
			return entities;
		}

		public long Total()
		{
			return Session.QueryOver<TEntity>().RowCountInt64();
		}

		public bool Exists(TKey id)
		{
			if (id == null) throw new ArgumentNullException(nameof(id));

			return QueryById(id).Any();
		}

		#endregion

		#region Implementation of IExposeQueryable<TEntity,in TKey>

		public IQueryable<TEntity> Query()
		{
			return Session.Query<TEntity>();
		}

		public abstract IQueryable<TEntity> QueryById(TKey id);

		public IQueryable<TEntity> QueryFetching(params Expression<Func<TEntity, object>>[] propertiesToFetch)
		{
			if (propertiesToFetch == null) throw new ArgumentNullException(nameof(propertiesToFetch));

			return propertiesToFetch.Aggregate(Query(), (current, expression) => current.Fetch(expression));
		}

		#endregion

		#region Implementation of INHRepository<TEntity,in TKey>

		public ISession Session { get; }

		#endregion

		private async Task FlushAsync(CancellationToken ct)
		{
			await Task.Run(() => Flush(), ct);
		}

		private void Flush()
		{
			try
			{
				Session.Flush();
			}
			catch (StaleObjectStateException e)
			{
				throw new ConcurrencyException(e);
			}
		}
	}

}