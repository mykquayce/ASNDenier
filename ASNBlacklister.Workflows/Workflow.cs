using System;
using WorkflowCore.Interface;

namespace ASNBlacklister.Workflows
{
	public class Workflow : IWorkflow<Models.PersistenceData>
	{
		public string Id => nameof(Workflow);
		public int Version => 1;

		public void Build(IWorkflowBuilder<Models.PersistenceData> builder)
		{
			builder
				.StartWith<Steps.ClearBlacklistStep>()
				.Then<Steps.GetASNNumbersStep>()
					.Output(data => data.ASNNumbers, step => step.ASNNumbers)
				.ForEach(data => data.ASNNumbers)
					.Do(each => each
						.StartWith<Steps.GetSubnetsStep>()
							.Input(step => step.ASNNumber, (_, context) => context.Item as int? ?? 0)
							.Output(data => data.SubnetAddresses, step => step.Subnets)
						.Then<Steps.BlacklistSubnetsStep>()
							.Input(step => step.SubnetAddresses, data => data.SubnetAddresses)
					)
				.EndWorkflow();
		}
	}
}
