using System.Threading;
using System.Threading.Tasks;

namespace Business.Contracts.Managers.Configurations
{
	public interface IConfigurationManager : IManager
	{
		Task<SecuredHttpEndpointConfigurations> GetPartnershipServiceConfigurationsAsync(CancellationToken ct);

	}
}