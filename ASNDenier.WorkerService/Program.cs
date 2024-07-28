IHostBuilder builder = Host.CreateDefaultBuilder(args);

builder
	.ConfigureServices((hostContext, services) =>
	{
		services
			.Configure<Helpers.Timing.Interval>(hostContext.Configuration.GetSection(nameof(Helpers.Timing.Interval)))
			.Configure<ASNDenier.Models.ASNNumbers>(hostContext.Configuration.GetSection(nameof(ASNDenier.Models.ASNNumbers)));

		services.AddHostedService<ASNDenier.WorkerService.Worker>();

		services
			.AddTransient<Helpers.Networking.Clients.IWhoIsClient, Helpers.Networking.Clients.Concrete.WhoIsClient>()
			.AddSshClient(b =>
			{
				var config = hostContext.Configuration.GetSection("router").Get<Config>();

				if (string.IsNullOrEmpty(config.Host) || string.IsNullOrEmpty(config.Username))
				{
					throw new KeyNotFoundException("router config missing");
				}

				b.Host = config.Host;
				b.Port = config.Port;
				b.Username = config.Username;
				b.Password = config.Password;
				b.PathToPrivateKey = config.PathToPrivateKey;
			});
	});

IHost host = builder.Build();

host.Run();

internal readonly record struct Config(string Host, ushort Port, string Username, string? Password, string? PathToPrivateKey);
