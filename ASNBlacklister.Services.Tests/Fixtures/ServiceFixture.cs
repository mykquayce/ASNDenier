namespace ASNBlacklister.Services.Tests.Fixtures;

public sealed class ServiceFixture : ConfigurationFixture, IDisposable
{
	public ServiceFixture()
	{
		Client = new Helpers.SSH.Concrete.Client(base.Config);
		Service = new Helpers.SSH.Concrete.Service(Client);
	}

	public Helpers.SSH.IClient Client { get; }
	public Helpers.SSH.IService Service { get; }

	public void Dispose() => Client?.Dispose();
}
