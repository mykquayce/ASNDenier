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
				.ForEach(data => data.ASNNumbers, runParallel: _ => false)
					.Do(each => each
						.StartWith<Steps.GetSubnetsStep>()
							.Input(step => step.ASNNumber, (_, context) => context.Item as int? ?? 0)
							.Output(data => data.Prefixes, step => step.Prefixes)
						.Then<Steps.BlacklistSubnetsStep>()
							.Input(step => step.Prefixes, data => data.Prefixes)
					)
				.EndWorkflow();
		}
	}
}
