using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Configurations.Contracts;

namespace Configurations
{
	public static class Config
	{
		public static IConfig<T> Create<T>(T value) where T : class => new SingletonConfig<T>(value);

		private class SingletonConfig<T> : IConfig<T> where T : class
		{
			private readonly Task<T> _task;

			public SingletonConfig(T value)
			{
				if (value == null) throw new ArgumentNullException(nameof(value));

				_task = Task.FromResult(value);
			}

			public Task<T> GetAsync() => _task;

			public void Reload()
			{

			}

			public TaskAwaiter<T> GetAwaiter() => _task.GetAwaiter();
		}
	}
}