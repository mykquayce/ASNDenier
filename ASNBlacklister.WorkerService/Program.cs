using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace ASNBlacklister.WorkerService
{
	public class Program
	{
		public static Task Main(string[] args) => CreateHostBuilder(args).RunConsoleAsync();

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			var hostBuilder = Host.CreateDefaultBuilder(args);

			hostBuilder
				.ConfigureServices((hostContext, services) =>
				{
					services.Configure<Models.ASNNumbers>(hostContext.Configuration.GetSection(nameof(Models.ASNNumbers)));

					services.AddHostedService<Worker>();

					services
						.AddTransient<Helpers.Networking.Clients.IWhoIsClient, Helpers.Networking.Clients.Concrete.WhoIsClient>();

					services
						.AddTransient<Workflows.Steps.EchoStep>()
						.AddTransient<Workflows.Steps.GetASNNumbersStep>()
						.AddTransient<Workflows.Steps.GetSubnets>();

					services.AddWorkflow();
				});

			return hostBuilder;
		}
	}
}
