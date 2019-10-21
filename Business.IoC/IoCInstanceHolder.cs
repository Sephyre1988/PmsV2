using System;

namespace Business.IoC
{
	public class IoCInstanceHolder<TService> : IDisposable
		where TService : class
	{
		public TService Service { get; private set; }
		public IoCInstanceHolder(TService service)
		{
			Service = service;
		}

		~IoCInstanceHolder()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				(Service as IDisposable)?.Dispose();
			}
			Service = null;
		}
	}

	public class IoCInstanceHolder<TService, TService2> : IDisposable
		where TService : class
		where TService2 : class
	{
		public TService Service { get; private set; }
		public TService2 Service2 { get; private set; }
		public IoCInstanceHolder(TService service, TService2 service2)
		{
			Service = service;
			Service2 = service2;
		}

		~IoCInstanceHolder()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				(Service as IDisposable)?.Dispose();
				(Service2 as IDisposable)?.Dispose();
			}
			Service = null;
			Service2 = null;
		}
	}
}