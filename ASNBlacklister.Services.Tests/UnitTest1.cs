using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ASNBlacklister.Services.Tests
{
	public sealed class UnitTest1 : IClassFixture<Fixture>
	{
		private readonly Helpers.Networking.Clients.IWhoIsClient _whoIsClient;
		private readonly Helpers.SSH.Services.ISSHService _sshService;

		public UnitTest1(Fixture fixture)
		{
			_whoIsClient = fixture.WhoIsClient;
			_sshService = fixture.SSHService;
		}

#pragma warning disable xUnit1004 // Test methods should not be skipped
		[Theory(Skip = "desctructive")]
#pragma warning restore xUnit1004 // Test methods should not be skipped
		[InlineData(32_934)] // facebook
		[InlineData(7_224)] // AMAZON - AS, US
		[InlineData(8_987)] // AMAZON EXPANSION, IE
		[InlineData(14_618)] // AMAZON-AES, US
		[InlineData(16_509)] // AMAZON-02, US
		public async Task AddIPsToBlackHole_ConfirmTheyWereAdded(int asn)
		{
			var subnets = await _whoIsClient.GetIpsAsync(asn).ToListAsync();

			Assert.NotNull(subnets);
			Assert.NotEmpty(subnets);
			Assert.DoesNotContain(default, subnets);

			await _sshService.AddBlackholesAsync(subnets);
		}

		[Fact]
		public async Task GetBlacklistedASNs()
		{
			var cache = new Dictionary<IPAddress, (int, string)>();
			//ssh into router
			//	ip route show | grep ^blackhole | awk '{print($2)}'
			var blackholes = _sshService.GetBlackholesAsync();
			//foreach ip
			await foreach (var (ip, _) in blackholes)
			{
				//	check cache
				//	tryget ip
				if (cache.TryGetValue(ip, out var _)) continue;
				//	false
				//	tcp client
				//		111.111.111.111\n
				var details = _whoIsClient.GetWhoIsDetailsAsync(ip);
				//		returns asns
				await foreach (var (_, asn, description, _) in details)
				{
					//		-F -K -i 11111\n
					var ips = _whoIsClient.GetIpsAsync(asn);
					//		returns ips
					//			foreach ip
					await foreach (var (ip2, _) in ips)
					{
						//				cache
						//				ip=asn
						cache.TryAdd(ip2, (asn, description));
					}
				}
			}

			var asns = cache.Values
				.Distinct()
				.OrderBy(t => t.Item2, StringComparer.InvariantCultureIgnoreCase)
				.ToList();
		}
	}
}
