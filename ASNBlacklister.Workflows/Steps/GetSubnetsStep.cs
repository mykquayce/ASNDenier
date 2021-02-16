using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace ASNDenier.Workflows.Steps
{
	public class GetSubnetsStep : IStepBody
	{
		private readonly Helpers.Networking.Clients.IWhoIsClient _whoIsClient;

		public GetSubnetsStep(Helpers.Networking.Clients.IWhoIsClient whoIsClient)
		{
			_whoIsClient = whoIsClient;
		}

		public int? ASNNumber { get; set; }
		public ICollection<Helpers.Networking.Models.SubnetAddress> Subnets { get; } = new List<Helpers.Networking.Models.SubnetAddress>();

		public async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
		{
			await foreach (var (ip, mask) in _whoIsClient.GetIpsAsync(ASNNumber!.Value))
			{
				var subnet = new Helpers.Networking.Models.SubnetAddress(ip, mask);
				Subnets.Add(subnet);
			}

			return ExecutionResult.Next();
		}
	}
}
