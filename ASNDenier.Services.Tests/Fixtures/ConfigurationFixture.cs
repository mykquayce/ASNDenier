using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASNDenier.Services.Tests.Fixtures;

public class ConfigurationFixture
{
	public ConfigurationFixture()
	{
		var configuration = new ConfigurationBuilder()
			.AddUserSecrets<WorkerService.Worker>()
			.Build();

		var provider = new ServiceCollection()
			.Configure<Helpers.SSH.Config>(configuration.GetSection("Router"))
			.BuildServiceProvider();

		var options = provider.GetService<IOptions<Helpers.SSH.Config>>()
			?? throw new KeyNotFoundException();

		Config = options.Value;
	}

	public Helpers.SSH.Config Config { get; }
}
