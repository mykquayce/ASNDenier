using System;
using WorkflowCore.Interface;

namespace ASNBlacklister.Workflows
{
	public class Workflow : IWorkflow
	{
		public string Id => nameof(Workflow);
		public int Version => 1;

		public void Build(IWorkflowBuilder<object> builder)
		{
			builder
				.StartWith<Steps.EchoStep>()
					.Input(step => step.Message, _ => $"Worker running at: {DateTimeOffset.Now}")
				.EndWorkflow();
		}
	}
}
