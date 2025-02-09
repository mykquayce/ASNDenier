using Cronos;
using Microsoft.Extensions.Options;

namespace ASNDenier.WorkerService;

public class Worker(
	ILogger<Worker> logger,
	IOptions<Models.ASNNumbers> asnNumbersOptions,
	IOptions<CronExpression> schedule,
	Helpers.SSH.IService sshService,
	Helpers.Networking.Clients.IWhoIsClient whoIsClient) : BackgroundService
{
	private readonly IReadOnlyDictionary<string, int[]> _asnNumbers = asnNumbersOptions.Value;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await sshService.DeleteBlackholesAsync(stoppingToken);
			logger.LogInformation("deleted blackholes");

			foreach (var (organization, asns) in _asnNumbers)
			{
				logger.LogInformation("{organization} has {count} asn(s) : {asns}", organization, asns.Length, string.Join(", ", asns));

				foreach (var asn in asns)
				{
					var prefixes = await whoIsClient.GetIpsAsync(asn, stoppingToken).ToArrayAsync(stoppingToken);

					logger.LogInformation("Applying {Count} prefix(es)", prefixes.Length);

					var idx = 1;
					foreach (var batch in prefixes.Chunk(size: 100))
					{
						logger.LogInformation("batch {idx}", idx++);
						await sshService.AddBlackholesAsync(batch, stoppingToken);
					}
				}
			}

			var delay = getdelay();
			logger.LogInformation(@"sleeping for {minutes:hh\:mm\:ss}", delay);
			try { await Task.Delay(delay, stoppingToken); }
			catch (TaskCanceledException) { break; }
		}

		TimeSpan getdelay()
		{
			var min = TimeSpan.FromMinutes(5);
			var next = schedule.Value.GetNextOccurrence(DateTime.UtcNow)!.Value - DateTime.UtcNow;
			return next < min ? min : next;
		}
	}
}
