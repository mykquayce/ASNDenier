using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ASNDenier.Services.Tests;

public sealed class Fixture : IDisposable
{
	private readonly IServiceProvider _serviceProvider;

	public Fixture()
	{
		var configuration = new ConfigurationBuilder()
			.AddUserSecrets<WorkerService.Worker>()
			.Build();

		_serviceProvider = new ServiceCollection()
			.AddSshClient(b =>
			{
				b.Host = configuration["router:host"]!;
				b.Port = ushort.Parse(configuration["router:port"]!);
				b.Username = configuration["router:username"]!;
				b.Password = configuration["router:password"];
				b.PathToPrivateKey = configuration["router:pathtoprivatekey"];
			})
			.AddTransient<Helpers.Networking.Clients.IWhoIsClient, Helpers.Networking.Clients.Concrete.WhoIsClient>()
			.BuildServiceProvider();

		Client = _serviceProvider.GetRequiredService<Helpers.SSH.IClient>();
		Service = _serviceProvider.GetRequiredService<Helpers.SSH.IService>();
		WhoIsClient = _serviceProvider.GetRequiredService<Helpers.Networking.Clients.IWhoIsClient>();
	}

	public Helpers.SSH.IClient Client { get; }
	public Helpers.SSH.IService Service { get; }
	public Helpers.Networking.Clients.IWhoIsClient WhoIsClient { get; }

	public void Dispose() => (_serviceProvider as ServiceProvider)?.Dispose();
}
