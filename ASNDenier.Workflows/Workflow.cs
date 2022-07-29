﻿using WorkflowCore.Interface;

namespace ASNDenier.Workflows;

public class Workflow : IWorkflow<Models.PersistenceData>
{
	public string Id => nameof(Workflow);
	public int Version => 1;

	public void Build(IWorkflowBuilder<Models.PersistenceData> builder)
	{
		builder
			.StartWith<Steps.ClearDenylistStep>()
			.ForEach(data => data.ASNNumbers, runParallel: _ => false)
				.Do(each => each
					.StartWith<Steps.GetSubnetsStep>()
						.Input(step => step.ASNNumbers, (_, context) => context.Item as KeyValuePair<string, int[]>?)
						.Output(data => data.Prefixes, step => step.Prefixes)
					.Then<Steps.DenySubnetsStep>()
						.Input(step => step.Prefixes, data => data.Prefixes)
				)
			.EndWorkflow();
	}
}