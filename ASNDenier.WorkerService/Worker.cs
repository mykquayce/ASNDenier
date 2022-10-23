using Dawn;
using Helpers.Timing;
using Microsoft.Extensions.Options;

namespace ASNDenier.WorkerService;

public class Worker : BackgroundService
{
	private readonly ILogger<Worker> _logger;
	private readonly IReadOnlyDictionary<string, int[]> _asnNumbers;
	private readonly IInterval _interval;
	private readonly Helpers.SSH.IService _sshService;
	private readonly Helpers.Networking.Clients.IWhoIsClient _whoIsClient;

	public Worker(
		ILogger<Worker> logger,
		IOptions<Models.ASNNumbers> asnNumbersOptions,
		IOptions<Interval> intervalOptions,
		Helpers.SSH.IService sshService,
		Helpers.Networking.Clients.IWhoIsClient whoIsClient)
	{
		_logger = Guard.Argument(logger).NotNull().Value;
		_asnNumbers = Guard.Argument(asnNumbersOptions).NotNull().Wrap(o => o.Value)
			.NotNull().NotEmpty().Value;
		_interval = Guard.Argument(intervalOptions).NotNull().Wrap(o => o.Value)
			.NotNull().Value;
		_sshService = Guard.Argument(sshService).NotNull().Value;
		_whoIsClient = Guard.Argument(whoIsClient).NotNull().Value;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await _sshService.DeleteBlackholesAsync();
			_logger.LogInformation("deleted blackholes");

			foreach (var (organization, asns) in _asnNumbers)
			{
				_logger.LogInformation("{organization} has {count} asn(s) : {asns}", organization, asns.Length, string.Join(", ", asns));

				foreach (var asn in asns)
				{
					var prefixes = await _whoIsClient.GetIpsAsync(asn, stoppingToken).ToListAsync(stoppingToken);

					_logger.LogInformation("Applying {Count} prefix(es)", prefixes.Count);

					await _sshService.AddBlackholesAsync(prefixes);
				}
			}

			var delay = _interval.Next() - DateTime.UtcNow;
			var millisecondInterval = (int)delay.TotalMilliseconds;
			_logger.LogInformation(@"sleeping for {minutes:hh\:mm\:ss}", delay);
			await Task.Delay(millisecondInterval, stoppingToken);
		}
	}
}
