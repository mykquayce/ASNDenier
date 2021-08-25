using Dawn;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace ASNDenier.Workflows.Steps
{
	public class GetSubnetsStep : IStepBody
	{
		private readonly Helpers.Networking.Clients.IWhoIsClient _whoIsClient;
		private readonly ILogger<GetSubnetsStep> _logger;

		public GetSubnetsStep(Helpers.Networking.Clients.IWhoIsClient whoIsClient, ILogger<GetSubnetsStep> logger)
		{
			_whoIsClient = whoIsClient;
			_logger = logger;
		}

		public int? ASNNumber { get; set; }
		public ICollection<Helpers.Networking.Models.AddressPrefix> Prefixes { get; } = new List<Helpers.Networking.Models.AddressPrefix>();

		public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
		{
			Guard.Argument(() => ASNNumber).NotNull().Positive();

			await foreach (var prefix in _whoIsClient.GetIpsAsync(ASNNumber!.Value))
			{
				Prefixes.Add(prefix);
			}

			_logger?.LogInformation("Found {Count} prefix(es) for ASN {ASNNumber}.", Prefixes.Count, ASNNumber);

			return ExecutionResult.Next();
		}
	}
}
