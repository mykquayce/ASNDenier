using Dawn;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace ASNBlacklister.Workflows.Steps
{
	public class GetSubnetsStep : IStepBody
	{
		private readonly Helpers.Networking.Clients.IWhoIsClient _whoIsClient;

		public GetSubnetsStep(Helpers.Networking.Clients.IWhoIsClient whoIsClient)
		{
			_whoIsClient = whoIsClient;
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

			return ExecutionResult.Next();
		}
	}
}
