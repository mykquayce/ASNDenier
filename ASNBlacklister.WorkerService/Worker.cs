using Dawn;
using Microsoft.Extensions.Options;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace ASNDenier.WorkerService;

public class Worker : BackgroundService
{
	private readonly ILogger<Worker> _logger;
	private readonly IWorkflowHost _workflowHost;
	private readonly IReadOnlyDictionary<string, int[]> _asnNumbers;

	public Worker(ILogger<Worker> logger, IWorkflowHost workflowHost, IOptions<Models.ASNNumbers> options)
	{
		_logger = logger
			?? throw new ArgumentNullException(nameof(logger));

		_workflowHost = workflowHost
			?? throw new ArgumentNullException(nameof(workflowHost));
		_workflowHost.OnStepError += WorkflowHost_OnStepError;

		_workflowHost.RegisterWorkflow<Workflows.Workflow, Models.PersistenceData>();

		_asnNumbers = Guard.Argument(() => options).NotNull().Wrap(o => o.Value)
			.NotNull().NotEmpty().Value;
	}

	private void WorkflowHost_OnStepError(
		WorkflowInstance workflow,
		WorkflowStep step,
		Exception exception)
	{
#if DEBUG
		System.Diagnostics.Debugger.Break();
#endif

		_logger?.LogError(exception, "step error");
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await _workflowHost.StartAsync(stoppingToken);

		while (!stoppingToken.IsCancellationRequested)
		{
			var data = new Models.PersistenceData
			{
				ASNNumbers = _asnNumbers,
			};
			await _workflowHost.StartWorkflow(nameof(Workflows.Workflow), data);
			await Task.Delay(86_400_000, stoppingToken);
		}

		await _workflowHost.StopAsync(stoppingToken);
	}
}
