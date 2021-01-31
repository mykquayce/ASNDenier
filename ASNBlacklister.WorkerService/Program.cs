using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace ASNDenier.WorkerService
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
						.AddTransient<Workflows.Steps.EchoStep>()
						.AddTransient<Workflows.Steps.GetASNNumbersStep>();

					services.AddWorkflow();
				});

			return hostBuilder;
		}
	}
}
