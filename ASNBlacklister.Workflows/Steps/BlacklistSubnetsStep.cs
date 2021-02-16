using Dawn;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace ASNDenier.Workflows.Steps
{
	public class BlacklistSubnetsStep : IStepBody
	{
		private readonly Helpers.OpenWrt.Services.IOpenWrtService _openWrtService;

		public IEnumerable<Helpers.Networking.Models.SubnetAddress>? SubnetAddresses { get; set; }

		public BlacklistSubnetsStep(Helpers.OpenWrt.Services.IOpenWrtService openWrtService)
		{
			_openWrtService = Guard.Argument(() => openWrtService).NotNull().Value;
		}

		public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
		{
			Guard.Argument(() => SubnetAddresses!).NotNull().NotEmpty().DoesNotContainNull();

			await _openWrtService.AddBlackholesAsync(SubnetAddresses!);

			return ExecutionResult.Next();
		}
	}
}
