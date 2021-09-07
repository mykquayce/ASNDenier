using Dawn;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace ASNBlacklister.Workflows.Steps
{
	public class BlacklistSubnetsStep : IStepBody
	{
		private readonly Helpers.SSH.IService _sshService;
		private readonly ILogger<BlacklistSubnetsStep> _logger;

		public ICollection<Helpers.Networking.Models.AddressPrefix>? Prefixes { get; set; }

		public BlacklistSubnetsStep(Helpers.SSH.IService sshService, ILogger<BlacklistSubnetsStep> logger)
		{
			_sshService = Guard.Argument(() => sshService).NotNull().Value;
			_logger = logger;
		}

		public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
		{
			Guard.Argument(() => Prefixes!).NotNull().DoesNotContainNull();

			if (Prefixes!.Any())
			{
				_logger?.LogInformation("Applying prefixes {Prefixes}", string.Join(", ", Prefixes!));
				await _sshService.AddBlackholesAsync(Prefixes!);
			}

			return ExecutionResult.Next();
		}
	}
}
