namespace ASNDenier.WorkerService;

public class Program
{
	public static Task Main(string[] args) => CreateHostBuilder(args).RunConsoleAsync();

	public static IHostBuilder CreateHostBuilder(params string[] args)
	{
		var hostBuilder = Host.CreateDefaultBuilder(args);

		hostBuilder.ConfigureHostConfiguration(config =>
		{
			config.AddUserSecrets<Program>(optional: true, reloadOnChange: true);
		});

		hostBuilder
			.ConfigureServices((hostContext, services) =>
			{
				services
					.Configure<Models.ASNNumbers>(hostContext.Configuration.GetSection(nameof(Models.ASNNumbers)))
					.Configure<Helpers.SSH.Config>(hostContext.Configuration.GetSection("Router"));

				services.AddHostedService<Worker>();

				services
					.AddTransient<Helpers.Networking.Clients.IWhoIsClient, Helpers.Networking.Clients.Concrete.WhoIsClient>()
					.AddTransient<Helpers.SSH.IClient, Helpers.SSH.Concrete.Client>()
					.AddTransient<Helpers.SSH.IService, Helpers.SSH.Concrete.Service>();
			});

		return hostBuilder;
	}
}
