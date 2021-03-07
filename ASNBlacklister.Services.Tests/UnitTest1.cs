using System.Linq;
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

		[Theory]
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
	}
}
