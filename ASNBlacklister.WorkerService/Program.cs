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
					services.AddHostedService<Worker>();

					services.AddTransient<Workflows.Steps.EchoStep>();

					services.AddWorkflow();
				});

			return hostBuilder;
		}
	}
}
