namespace ASNDenier.Services.Tests.Fixtures;

public class ClientFixture
{
	public ClientFixture()
	{
		WhoIsClient = new Helpers.Networking.Clients.Concrete.WhoIsClient();
	}

	public Helpers.Networking.Clients.IWhoIsClient WhoIsClient { get; }
}
