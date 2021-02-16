﻿using Moq;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using Xunit;

namespace ASNDenier.Workflows.Tests
{
	public class GetSubnetsStepTests
	{
		private readonly Steps.GetSubnetsStep _sut;

		public GetSubnetsStepTests()
		{
			var whoIsClient = new Helpers.Networking.Clients.Concrete.WhoIsClient();

			_sut = new Steps.GetSubnetsStep(whoIsClient);
		}

		[Theory]
		[InlineData(714)]
		[InlineData(2906)]
		[InlineData(43650)]
		[InlineData(32934)]
		public async Task Run(int asnNumber)
		{
			_sut.ASNNumber = asnNumber;

			var result = await _sut.RunAsync(Mock.Of<IStepExecutionContext>());

			Assert.True(result.Proceed);

			Assert.NotEmpty(_sut.Subnets);
		}
	}
}
