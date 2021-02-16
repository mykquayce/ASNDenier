using Microsoft.Extensions.Hosting;
using System;

namespace ASNDenier.WorkerService.Tests.Fixtures
{
	public sealed class HostFixture : IDisposable
	{
		public HostFixture()
		{
			var hostBuilder = Program.CreateHostBuilder();
			Host = hostBuilder.Build();
		}

		public void Dispose() => Host.Dispose();

		public IHost Host { get; }
	}
}
