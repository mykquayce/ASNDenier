using Dawn;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace ASNDenier.Workflows.Steps
{
	public class BlacklistSubnetsStep : IStepBody
	{
		private readonly Helpers.SSH.Services.ISSHService _sshService;

		public IEnumerable<Helpers.Networking.Models.SubnetAddress>? SubnetAddresses { get; set; }

		public BlacklistSubnetsStep(Helpers.SSH.Services.ISSHService sshService)
		{
			_sshService = Guard.Argument(() => sshService).NotNull().Value;
		}

		public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
		{
			Guard.Argument(() => SubnetAddresses!).NotNull().NotEmpty().DoesNotContainNull();

			await _sshService.AddBlackholesAsync(SubnetAddresses!);

			return ExecutionResult.Next();
		}
	}
}
