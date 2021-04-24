using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace ASNDenier.WorkerService
{
	public class Program
	{
		public static Task Main(string[] args) => CreateHostBuilder(args).RunConsoleAsync();

		public static IHostBuilder CreateHostBuilder(params string[] args)
		{
			var hostBuilder = Host.CreateDefaultBuilder(args);

			hostBuilder.ConfigureHostConfiguration(config =>
			{
				var assembly = typeof(Program).Assembly;
				config.AddUserSecrets(assembly, optional: true, reloadOnChange: true);
			});

			hostBuilder
				.ConfigureServices((hostContext, services) =>
				{
					services
						.Configure<Models.ASNNumbers>(hostContext.Configuration.GetSection(nameof(Models.ASNNumbers)))
						.Configure<Helpers.SSH.Services.Concrete.SSHService.Config>(hostContext.Configuration.GetSection("Router"));

					services.AddHostedService<Worker>();

					services
						.AddTransient<Helpers.Networking.Clients.IWhoIsClient, Helpers.Networking.Clients.Concrete.WhoIsClient>()
						.AddTransient<Helpers.SSH.Services.ISSHService, Helpers.SSH.Services.Concrete.SSHService>();

					services
						.AddTransient<Workflows.Steps.BlacklistSubnetsStep>()
						.AddTransient<Workflows.Steps.ClearBlacklistStep>()
						.AddTransient<Workflows.Steps.GetASNNumbersStep>()
						.AddTransient<Workflows.Steps.GetSubnetsStep>();

					services.AddWorkflow();
				});

			return hostBuilder;
		}
	}
}
