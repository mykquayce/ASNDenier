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
				.StartWith<Steps.EchoStep>()
					.Input(step => step.Message, _ => $"Worker running at: {DateTimeOffset.Now}")
				.Then<Steps.GetASNNumbersStep>()
					.Output(data => data.ASNNumbers, step => step.ASNNumbers)
				.Then<Steps.EchoStep>()
					.Input(step => step.Message, data => string.Join(',', data.ASNNumbers!))
				.EndWorkflow();
		}
	}
}
