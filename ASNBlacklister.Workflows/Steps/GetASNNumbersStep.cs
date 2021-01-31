using Microsoft.Extensions.Options;
using System.Collections.Generic;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace ASNDenier.Workflows.Steps
{
	public class GetASNNumbersStep : StepBody
	{
		public GetASNNumbersStep(IOptions<Models.ASNNumbers> options)
		{
			ASNNumbers = options.Value;
		}

		public ICollection<int> ASNNumbers { get; }

		public override ExecutionResult Run(IStepExecutionContext context) => ExecutionResult.Next();
	}
}
