using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Configurations.Contracts;
using UoW;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Configurations
{
	public abstract class CachedConfig<T> : IConfig<T> where T : class
	{
		protected const string CachePrefix = "Config->Cached->";

		private readonly IPmsUnitOfWorkFactory _uowFactory;
		private readonly IMemoryCache _cache;
		private readonly string _key;
		private readonly CachedConfigOptions _options;

		protected CachedConfig(
			IPmsUnitOfWorkFactory uowFactory, IMemoryCache cache,
			CachedConfigOptions options, ILogger<CachedConfig<T>> logger)
		{
			if (uowFactory == null) throw new ArgumentNullException(nameof(uowFactory));
			if (cache == null) throw new ArgumentNullException(nameof(cache));
			if (options == null) throw new ArgumentNullException(nameof(options));
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			_uowFactory = uowFactory;
			_cache = cache;
			_key = CachePrefix + GetType().Name;
			_options = options;
			Logger = logger;
		}

		protected ILogger Logger { get; }

		public virtual async Task<T> GetAsync()
		{
			using (Logger.BeginScope("Key:'{key}'", _key))
			{
				Logger.LogDebug("Getting configuration from cache");

				T item;
				if (_cache.TryGetValue(_key, out item))
					return item;

				Logger.LogWarning(
					"Failed cache hit... Creating new item [AbsoluteExpirationRelativeToNow:'{absoluteExpirationRelativeToNow}' SlidingExpiration:'{slidingExpiration}']",
					_options.AbsoluteExpirationRelativeToNow, _options.SlidingExpiration);

				using (var uowScope = _uowFactory.GetScope())
				{
					item = await CreateAsync(uowScope.UnitOfWork, CancellationToken.None);
				}

				using (var k = _cache.CreateEntry(_key))
				{
					k.AbsoluteExpirationRelativeToNow = _options.AbsoluteExpirationRelativeToNow;
					k.SlidingExpiration = _options.SlidingExpiration;

					k.Value = item;
				}

				return item;
			}
		}

		public void Reload() => _cache.Remove(_key);

		public TaskAwaiter<T> GetAwaiter() => GetAsync().GetAwaiter();

		protected abstract Task<T> CreateAsync(IPmsUnitOfWork uow, CancellationToken ct);
	}

	
}