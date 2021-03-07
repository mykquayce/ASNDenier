using System;

namespace ASNBlacklister.Services.Tests
{
	public sealed class Fixture : IDisposable
	{
		public Fixture()
		{
			WhoIsClient = new Helpers.Networking.Clients.Concrete.WhoIsClient();

			var configurationFixture = new ConfigurationFixture();

			var host = configurationFixture["Router:Host"];
			var port = int.Parse(configurationFixture["Router:Port"]);
			var username = configurationFixture["Router:Username"];
			var password = configurationFixture["Router:Password"];

			SSHService = new Helpers.SSH.Services.Concrete.SSHService(host, port, username, password);
		}

		public Helpers.Networking.Clients.IWhoIsClient WhoIsClient { get; }
		public Helpers.SSH.Services.ISSHService SSHService { get; }

		public void Dispose()
		{
			WhoIsClient?.Dispose();
			SSHService?.Dispose();
		}
	}
}
