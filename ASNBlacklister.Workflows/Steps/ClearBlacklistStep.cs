using Dawn;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace ASNBlacklister.Workflows.Steps;

public class ClearBlacklistStep : IStepBody
{
	private readonly Helpers.SSH.IService _sshService;

	public ClearBlacklistStep(Helpers.SSH.IService sshService)
	{
		_sshService = Guard.Argument(sshService).NotNull().Value;
	}

	public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
	{
		await _sshService.DeleteBlackholesAsync();

		return ExecutionResult.Next();
	}
}
