using Dawn;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace ASNDenier.Workflows.Steps
{
	public class ClearBlacklistStep : IStepBody
	{
		private readonly Helpers.SSH.Services.ISSHService _sshService;

		public ClearBlacklistStep(Helpers.SSH.Services.ISSHService sshService)
		{
			_sshService = Guard.Argument(() => sshService).NotNull().Value;
		}

		public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
		{
			await _sshService.DeleteBlackholesAsync();

			return ExecutionResult.Next();
		}
	}
}
