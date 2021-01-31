using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace ASNDenier.Workflows.Steps
{
	public class GetSubnets : IStepBody
	{
		private readonly Helpers.Networking.Clients.IWhoIsClient _whoIsClient;

		public GetSubnets(Helpers.Networking.Clients.IWhoIsClient whoIsClient)
		{
			_whoIsClient = whoIsClient;
		}

		public int? ASNNumber { get; set; }
		public ICollection<Models.Subnet> Subnets { get; } = new List<Models.Subnet>();

		public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
		{
			await foreach (var (ip, mask) in _whoIsClient.GetIpsAsync(ASNNumber!.Value))
			{
				var subnet = new Models.Subnet(ip, mask);
				Subnets.Add(subnet);
			}

			return ExecutionResult.Next();
		}
	}
}
