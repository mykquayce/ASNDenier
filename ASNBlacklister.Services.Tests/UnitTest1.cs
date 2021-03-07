using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ASNBlacklister.Services.Tests
{
	public sealed class UnitTest1 : IClassFixture<Fixture>
	{
		public Helpers.Networking.Clients.IWhoIsClient _whoIsClient;
		private readonly Helpers.OpenWrt.Services.IOpenWrtService _openWrtService;

		public UnitTest1(Fixture fixture)
		{
			_whoIsClient = fixture.WhoIsClient;
			_openWrtService = fixture.OpenWrtService;
		}

		[Theory]
		[InlineData(32_934)] // facebook
		public async Task AddIPsToBlackHole_ConfirmTheyWereAdded(int asn)
		{
			var subnets = await _whoIsClient.GetIpsAsync(asn).ToListAsync();

			Assert.NotNull(subnets);
			Assert.NotEmpty(subnets);
			Assert.DoesNotContain(default, subnets);

			await _openWrtService.AddBlackholesAsync(subnets);
		}
	}
}
