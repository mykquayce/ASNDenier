using Dawn;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace ASNBlacklister.Workflows.Steps
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

		public KeyValuePair<string, int[]>? ASNNumbers { get; set; }
		public ICollection<Helpers.Networking.Models.AddressPrefix> Prefixes { get; } = new List<Helpers.Networking.Models.AddressPrefix>();

		public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
		{
			var (organization, asns) = Guard.Argument(() => ASNNumbers).NotNull().Value;

			_logger.LogInformation("getting prefix(es) for {organization} {count} asn(s)", organization, asns.Length);

			foreach (var asn in asns)
			{
				var prefixes = _whoIsClient.GetIpsAsync(asn);

				await foreach (var prefix in prefixes)
				{
					Prefixes.Add(prefix);
				}
			}

			_logger?.LogInformation("Found {count} prefix(es) for {organization}.", Prefixes.Count, organization);

			return ExecutionResult.Next();
		}
	}
}
