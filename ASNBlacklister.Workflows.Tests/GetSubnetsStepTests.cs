using Microsoft.Extensions.Logging;
using Moq;
using WorkflowCore.Interface;
using Xunit;

namespace ASNBlacklister.Workflows.Tests;

public class GetSubnetsStepTests
{
	private readonly Steps.GetSubnetsStep _sut;

	public GetSubnetsStepTests()
	{
		var whoIsClient = new Helpers.Networking.Clients.Concrete.WhoIsClient();
		var logger = Mock.Of<ILogger<Steps.GetSubnetsStep>>();

		_sut = new Steps.GetSubnetsStep(whoIsClient, logger);
	}

	[Theory]
	[InlineData("apple", 714)]
	[InlineData("netflix", 2906)]
	[InlineData("facebook", 32934)]
	public async Task Run(string organization, params int[] asns)
	{
		_sut.ASNNumbers = new KeyValuePair<string, int[]>(organization, asns);

		var result = await _sut.RunAsync(Mock.Of<IStepExecutionContext>());

		Assert.True(result.Proceed);

		Assert.NotEmpty(_sut.Prefixes);
		Assert.DoesNotContain(default, _sut.Prefixes);

		foreach (var (ip, mask) in _sut.Prefixes)
		{
			Assert.NotNull(ip);
			Assert.InRange(mask, 1, 63);
		}
	}
}
