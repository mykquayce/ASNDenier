using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace ASNDenier.Services.Tests;

public class ConfigTests
{
	private readonly IConfiguration _configuration, _configurationSection;
	private readonly IServiceProvider _serviceProvider;

	public ConfigTests()
	{
		_configuration = new ConfigurationBuilder()
			.AddUserSecrets<WorkerService.Worker>()
			.Build();

		_configurationSection = _configuration.GetSection("router");

		_serviceProvider = new ServiceCollection()
			.Configure<Helpers.SSH.Config>(_configurationSection)
			.AddTransient<TestClass>()
			.BuildServiceProvider();
	}

	[Fact]
	public void Section() => Assert.NotNull(_configurationSection);

	[Theory]
	[InlineData("host")]
	[InlineData("port")]
	[InlineData("username")]
	public void Values(string key)
	{
		var actual = _configurationSection[key];

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
	}

	[Theory]
	[InlineData(Helpers.SSH.Config.DefaultHost)]
	public void Binding(string @default)
	{
		var actual = new Helpers.SSH.Config();

		Assert.Equal(@default, actual.Host);

		_configuration.GetSection("router").Bind(actual);

		Assert.NotEqual(@default, actual.Host);
	}

	[Theory]
	[InlineData(Helpers.SSH.Config.DefaultHost)]
	public void Services(string @default)
	{
		var options = _serviceProvider.GetService<IOptions<Helpers.SSH.Config>>();

		Assert.NotNull(options);

		var config = options.Value;

		Assert.NotNull(config);
		Assert.NotEqual(@default, config.Host);
	}

	[Theory]
	[InlineData(Helpers.SSH.Config.DefaultHost)]
	public void Injection(string @default)
	{
		var sut = _serviceProvider.GetService<TestClass>();

		Assert.NotNull(sut);
		Assert.NotNull(sut.Config);
		Assert.NotNull(sut.Config.Host);
		Assert.NotEqual(@default, sut.Config.Host);
	}

	private class TestClass
	{
		public TestClass(IOptions<Helpers.SSH.Config> configuration)
		{
			Config = configuration?.Value;
		}

		public Helpers.SSH.Config? Config { get; }
	}
}
