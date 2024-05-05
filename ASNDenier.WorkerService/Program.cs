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
				b.Host = hostContext.Configuration["router:host"] ?? throw new Exception();
				b.Port = ushort.TryParse(hostContext.Configuration["router:port"], out var u) ? u : throw new Exception();
				b.Username = hostContext.Configuration["router:username"] ?? throw new Exception();
				b.Password = hostContext.Configuration["router:password"];
				b.PathToPrivateKey = hostContext.Configuration["router:pathtoprivatekey"];
			});
	});

IHost host = builder.Build();

host.Run();
