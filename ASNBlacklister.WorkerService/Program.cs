using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace ASNBlacklister.WorkerService
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
				config.AddUserSecrets(assembly);
			});

			hostBuilder
				.ConfigureServices((hostContext, services) =>
				{
					services
						.Configure<Models.ASNNumbers>(hostContext.Configuration.GetSection(nameof(Models.ASNNumbers)))
						.Configure<Helpers.OpenWrt.Clients.Concrete.OpenWrtClient.Settings>(hostContext.Configuration.GetSection("Router"));

					services.AddHostedService<Worker>();

					services
						.AddHttpClient<Helpers.OpenWrt.Clients.IOpenWrtClient, Helpers.OpenWrt.Clients.Concrete.OpenWrtClient>(
						(provider, client) =>
						{
							var settings = hostContext.Configuration
								.GetSection("Router")
								.Get<Helpers.OpenWrt.Clients.Concrete.OpenWrtClient.Settings>();

							client.BaseAddress = new Uri("http://" + settings.EndPoint);
						});

					services
						.AddScoped<Helpers.Networking.Clients.IWhoIsClient, Helpers.Networking.Clients.Concrete.WhoIsClient>()
						.AddScoped<Helpers.OpenWrt.Services.IOpenWrtService, Helpers.OpenWrt.Services.Concrete.OpenWrtService>();

					services
						.AddScoped<Workflows.Steps.BlacklistSubnetsStep>()
						.AddTransient<Workflows.Steps.EchoStep>()
						.AddTransient<Workflows.Steps.GetASNNumbersStep>()
						.AddTransient<Workflows.Steps.GetSubnetsStep>();

					services.AddWorkflow();
				});

			return hostBuilder;
		}
	}
}
