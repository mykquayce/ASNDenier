IHostBuilder builder = Host.CreateDefaultBuilder(args);

builder
	.ConfigureServices((hostContext, services) =>
	{
		services
			.Configure<ASNDenier.Models.Interval>(hostContext.Configuration.GetSection(nameof(ASNDenier.Models.Interval)))
			.Configure<ASNDenier.Models.ASNNumbers>(hostContext.Configuration.GetSection(nameof(ASNDenier.Models.ASNNumbers)))
			.Configure<Helpers.SSH.Config>(hostContext.Configuration.GetSection("Router"));

		services.AddHostedService<ASNDenier.WorkerService.Worker>();

		services
			.AddTransient<Helpers.Networking.Clients.IWhoIsClient, Helpers.Networking.Clients.Concrete.WhoIsClient>()
			.AddTransient<Helpers.SSH.IClient, Helpers.SSH.Concrete.Client>()
			.AddTransient<Helpers.SSH.IService, Helpers.SSH.Concrete.Service>();
	});

IHost host = builder.Build();

host.Run();
