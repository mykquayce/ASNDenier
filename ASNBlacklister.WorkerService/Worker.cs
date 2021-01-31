using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace ASNBlacklister.WorkerService
{
	public class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;
		private readonly IWorkflowHost _workflowHost;

		public Worker(ILogger<Worker> logger, IWorkflowHost workflowHost)
		{
			_logger = logger
				?? throw new ArgumentNullException(nameof(logger));

			_workflowHost = workflowHost
				?? throw new ArgumentNullException(nameof(workflowHost));

			_workflowHost.RegisterWorkflow<Workflows.Workflow>();
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await _workflowHost.StartAsync(stoppingToken);

			while (!stoppingToken.IsCancellationRequested)
			{
				await _workflowHost.StartWorkflow(nameof(Workflows.Workflow));
				await Task.Delay(1000, stoppingToken);
			}

			await _workflowHost.StopAsync(stoppingToken);
		}
	}
}
