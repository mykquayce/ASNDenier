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
			var port = ushort.Parse(configurationFixture["Router:Port"]);
			var username = configurationFixture["Router:Username"];
			var pathToPrivateKey = configurationFixture["Router:PathToPrivateKey"];
			var pathToPublicKey = configurationFixture["Router:PathToPublicKey"];

			var config = new Helpers.SSH.Services.Concrete.SSHService.Config(host, port, username, PathToPrivateKey: pathToPrivateKey, PathToPublicKey: pathToPublicKey);

			SSHService = new Helpers.SSH.Services.Concrete.SSHService(config);
		}

		public Helpers.Networking.Clients.IWhoIsClient WhoIsClient { get; }
		public Helpers.SSH.Services.ISSHService SSHService { get; }

		public void Dispose() => SSHService?.Dispose();
	}
}
