using Dawn;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace ASNBlacklister.Workflows.Steps;

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
		var (organization, asns) = Guard.Argument(ASNNumbers).NotNull().Value;

		_logger.LogInformation("{organization} has {count} asn(s) : {asns}", organization, asns.Length, string.Join(", ", asns));

		foreach (var asn in asns)
		{
			var count = 0;
			var prefixes = _whoIsClient.GetIpsAsync(asn);

			await foreach (var prefix in prefixes)
			{
				count++;
				Prefixes.Add(prefix);
			}

			_logger?.LogInformation("{asn} has {count} prefix(es)", asn, count);
		}

		return ExecutionResult.Next();
	}
}
