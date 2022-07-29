using Dawn;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace ASNDenier.Workflows.Steps;

public class ClearDenylistStep : IStepBody
{
	private readonly Helpers.SSH.IService _sshService;

	public ClearDenylistStep(Helpers.SSH.IService sshService)
	{
		_sshService = Guard.Argument(sshService).NotNull().Value;
	}

	public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
	{
		await _sshService.DeleteBlackholesAsync();

		return ExecutionResult.Next();
	}
}
