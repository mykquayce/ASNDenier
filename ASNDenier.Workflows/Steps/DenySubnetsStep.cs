using Dawn;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace ASNDenier.Workflows.Steps;

public class DenySubnetsStep : IStepBody
{
	private readonly Helpers.SSH.IService _sshService;
	private readonly ILogger<DenySubnetsStep> _logger;

	public ICollection<Helpers.Networking.Models.AddressPrefix>? Prefixes { get; set; }

	public DenySubnetsStep(Helpers.SSH.IService sshService, ILogger<DenySubnetsStep> logger)
	{
		_sshService = Guard.Argument(sshService).NotNull().Value;
		_logger = logger;
	}

	public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
	{
		Guard.Argument(Prefixes!).NotNull().DoesNotContainNull();
		_logger?.LogInformation("Applying {Count} prefix(es)", Prefixes!.Count);

		if (Prefixes!.Any())
		{
			await _sshService.AddBlackholesAsync(Prefixes!);
		}

		return ExecutionResult.Next();
	}
}
