using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace ASNDenier.Services.Tests
{
	public sealed class Fixture : IDisposable
	{
		public Fixture()
		{
			WhoIsClient = new Helpers.Networking.Clients.Concrete.WhoIsClient();

			var configuration = new ConfigurationBuilder()
				.AddUserSecrets(typeof(ASNDenier.WorkerService.Program).Assembly)
				.Build();

			string s(string key) => configuration![key] ?? throw new KeyNotFoundException($"{key} {nameof(key)} not found.");

			var endPoint = "http://" + s("Router:EndPoint");
			var password = s("Router:Password");

			var settings = new Helpers.OpenWrt.Clients.Concrete.OpenWrtClient.Settings(endPoint, password);

			var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false, };
			var httpClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(settings.EndPoint!), };

			var options = Options.Create(settings);
			var openWrtClient = new Helpers.OpenWrt.Clients.Concrete.OpenWrtClient(httpClient, options);

			OpenWrtService = new Helpers.OpenWrt.Services.Concrete.OpenWrtService(openWrtClient);
		}

		public Helpers.Networking.Clients.IWhoIsClient WhoIsClient { get; }
		public Helpers.OpenWrt.Services.IOpenWrtService OpenWrtService { get; }

		public void Dispose()
		{
			WhoIsClient?.Dispose();
			OpenWrtService?.Dispose();
		}
	}
}
