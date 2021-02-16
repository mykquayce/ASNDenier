﻿using System;
using WorkflowCore.Interface;

namespace ASNDenier.Workflows
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
				.ForEach(data => data.ASNNumbers)
					.Do(each => each
						.StartWith<Steps.EchoStep>()
							.Input(step => step.Message, (_, context) => (context.Item as int? ?? 0).ToString())
						.Then<Steps.GetSubnets>()
							.Input(step => step.ASNNumber, (_, context) => context.Item as int? ?? 0)
							.Output(data => data.SubnetAddresses, step => step.Subnets)
						.Then<Steps.BlacklistSubnetsStep>()
							.Input(step => step.SubnetAddresses, data => data.SubnetAddresses)
					)
				.EndWorkflow();
		}
	}
}
