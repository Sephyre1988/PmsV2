using System;
using Microsoft.Extensions.DependencyInjection;
using UoW;

namespace Business.IoC.Factories
{
	public class PmsUnitOfWorkFactory : IPmsUnitOfWorkFactory
	{
		private readonly IServiceScopeFactory _scopeFactory;

		public PmsUnitOfWorkFactory(IServiceScopeFactory scopeFactory)
		{
			_scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
		}

		public IPmsUnitOfWorkFactoryScope GetScope() => new PmsUnitOfWorkFactoryScope(_scopeFactory.CreateScope());

		private class PmsUnitOfWorkFactoryScope : IPmsUnitOfWorkFactoryScope
		{
			private IServiceScope _scope;

			public PmsUnitOfWorkFactoryScope(IServiceScope scope)
			{
				_scope = scope ?? throw new ArgumentNullException(nameof(scope));
				UnitOfWork = scope.ServiceProvider.GetRequiredService<IPmsUnitOfWork>();
			}

			~PmsUnitOfWorkFactoryScope()
			{
				Dispose(false);
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			private void Dispose(bool disposing)
			{
				if (disposing)
				{
					_scope?.Dispose();
				}

				UnitOfWork = null;
				_scope = null;
			}

			public IPmsUnitOfWork UnitOfWork { get; private set; }
		}
	}

	
}