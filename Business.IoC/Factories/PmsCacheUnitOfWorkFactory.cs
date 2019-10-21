using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Business.IoC.Factories
{
	public class PmsCacheUnitOfWorkFactory : IPmsCacheUnitOfWorkFactory
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public PmsCacheUnitOfWorkFactory(IServiceScopeFactory serviceScopeFactory)
		{
			_serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
		}

		public void Using(Action<IPmsCacheScopedUnitOfWork> handler)
		{
			using (var scope = _serviceScopeFactory.CreateScope())
			{
				var uow = scope.ServiceProvider.GetRequiredService<IPmsCacheScopedUnitOfWork>();
				handler(uow);
			}
		}

		public T Using<T>(Func<IPmsCacheScopedUnitOfWork, T> handler)
		{
			using (var scope = _serviceScopeFactory.CreateScope())
			{
				var uow = scope.ServiceProvider.GetRequiredService<IPmsCacheScopedUnitOfWork>();
				return handler(uow);
			}
		}

		public async Task UsingAsync(Func<IPmsCacheScopedUnitOfWork, Task> handler)
		{
			using (var scope = _serviceScopeFactory.CreateScope())
			{
				var uow = scope.ServiceProvider.GetRequiredService<IPmsCacheScopedUnitOfWork>();
				await handler(uow);
			}
		}

		public async Task<T> UsingAsync<T>(Func<IPmsCacheScopedUnitOfWork, Task<T>> handler)
		{
			using (var scope = _serviceScopeFactory.CreateScope())
			{
				var uow = scope.ServiceProvider.GetRequiredService<IPmsCacheScopedUnitOfWork>();
				return await handler(uow);
			}
		}
	}
}