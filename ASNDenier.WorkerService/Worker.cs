using Dawn;
using Microsoft.Extensions.Options;

namespace ASNDenier.WorkerService;

public class Worker : BackgroundService
{
	private readonly ILogger<Worker> _logger;
	private readonly IReadOnlyDictionary<string, int[]> _asnNumbers;
	private readonly Helpers.SSH.IService _sshService;
	private readonly Helpers.Networking.Clients.IWhoIsClient _whoIsClient;

	public Worker(
		ILogger<Worker> logger,
		IOptions<Models.ASNNumbers> options,
		Helpers.SSH.IService sshService,
		Helpers.Networking.Clients.IWhoIsClient whoIsClient)
	{
		_logger = Guard.Argument(logger).NotNull().Value;
		_asnNumbers = Guard.Argument(options).NotNull().Wrap(o => o.Value)
			.NotNull().NotEmpty().Value;
		_sshService = Guard.Argument(sshService).NotNull().Value;
		_whoIsClient = Guard.Argument(whoIsClient).NotNull().Value;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await _sshService.DeleteBlackholesAsync();

			foreach (var (organization, asns) in _asnNumbers)
			{
				_logger.LogInformation("{organization} has {count} asn(s) : {asns}", organization, asns.Length, string.Join(", ", asns));

				foreach (var asn in asns)
				{
					var prefixes = await _whoIsClient.GetIpsAsync(asn).ToListAsync(stoppingToken);

					_logger?.LogInformation("Applying {Count} prefix(es)", prefixes.Count);

					await _sshService.AddBlackholesAsync(prefixes);
				}
			}

			await Task.Delay(86_400_000, stoppingToken);
		}
	}
}
