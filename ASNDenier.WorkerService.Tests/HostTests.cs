using Microsoft.Extensions.Hosting;
using System.Reflection;
using Xunit;

namespace ASNDenier.WorkerService.Tests;

public sealed class HostTests : IClassFixture<Fixtures.HostFixture>
{
	private readonly IHost _host;

	public HostTests(Fixtures.HostFixture hostFixture)
	{
		_host = hostFixture.Host;
	}
}

