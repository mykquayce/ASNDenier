using Microsoft.Extensions.Hosting;

namespace ASNDenier.WorkerService.Tests.Fixtures;

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
