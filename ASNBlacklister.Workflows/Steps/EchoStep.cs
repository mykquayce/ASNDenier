using Microsoft.Extensions.Logging;
using System;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace ASNBlacklister.Workflows.Steps
{
	public class EchoStep : StepBody
	{
		private readonly ILogger<EchoStep> _logger;

		public EchoStep(ILogger<EchoStep> logger)
		{
			_logger = logger
				?? throw new ArgumentNullException(nameof(logger));
		}

		public string? Message { get; set; }

		public override ExecutionResult Run(IStepExecutionContext context)
		{
			_logger.LogInformation(Message);
			return ExecutionResult.Next();
		}
	}
}
