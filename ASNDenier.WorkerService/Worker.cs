using Helpers.Timing;
using Microsoft.Extensions.Options;

namespace ASNDenier.WorkerService;

public class Worker(
	ILogger<Worker> logger,
	IOptions<Models.ASNNumbers> asnNumbersOptions,
	IOptions<Interval> intervalOptions,
	Helpers.SSH.IService sshService,
	Helpers.Networking.Clients.IWhoIsClient whoIsClient) : BackgroundService
{
	private readonly IReadOnlyDictionary<string, int[]> _asnNumbers = asnNumbersOptions.Value;
	private readonly IInterval _interval = intervalOptions.Value;

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

			var delay = new TimeSpan(long.Max(
				TimeSpan.FromMinutes(5).Ticks,
				(_interval.Next() - DateTime.UtcNow).Ticks));
			logger.LogInformation(@"sleeping for {minutes:hh\:mm\:ss}", delay);
			await Task.Delay(delay, stoppingToken);
		}
	}
}
