using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Configurations.Contracts
{
	public interface IConfig<T> where T : class
	{
		Task<T> GetAsync();

		void Reload();

		TaskAwaiter<T> GetAwaiter();
	}
}