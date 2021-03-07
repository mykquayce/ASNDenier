using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ASNBlacklister.Services.Tests
{
	public class ConfigurationFixture
	{
		private readonly IConfiguration _configuration;

		public ConfigurationFixture()
		{
			_configuration = new ConfigurationBuilder()
				.AddUserSecrets(typeof(ASNBlacklister.WorkerService.Program).Assembly)
				.Build();
		}

		public string this[string key] => _configuration[key] ?? throw new KeyNotFoundException($"{key} {nameof(key)} not found");
	}
}
