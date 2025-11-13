using Cronos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASNDenier.Services.Tests;

public class CronosTests
{
	private static readonly DateTimeOffset _now = DateTimeOffset.Parse("2025-02-09T11:13:21.0540598Z");


	[Theory]
	[InlineData("0 */3 * * *", "2025-02-09T12:00:00Z")]
	[InlineData("0 */6 * * *", "2025-02-09T12:00:00Z")]
	[InlineData("0 */8 * * *", "2025-02-09T16:00:00Z")]
	public void GetNextOccurrenceTests(string s, string expected)
	{
		var ok = CronExpression.TryParse(s, CronFormat.Standard, out var expession);

		Assert.True(ok);

		var actual = expession!.GetNextOccurrence(_now, TimeZoneInfo.Utc);

		Assert.NotNull(actual);
		Assert.Equal(DateTimeOffset.Parse(expected), actual.Value);
	}

	[Theory, InlineData("0 */6 * * *", "2025-02-09T12:00:00Z")]
	public void DependencyInjectionTests(string schedule, string expected)
	{
		var initialData = new Dictionary<string, string?>
		{
			[nameof(schedule)] = schedule,
		};

		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(initialData)
			.Build();

		using var provider = new ServiceCollection()
			.AddSingleton<IOptions<CronExpression>>(p =>
			{
				var schedule = configuration["schedule"];
				if (string.IsNullOrWhiteSpace(schedule))
				{
					throw new KeyNotFoundException("no schedule found.");
				}

				var ok = CronExpression.TryParse(schedule, CronFormat.Standard, out var cronExpression);
				if (!ok)
				{
					throw new Exception($"could not parse {schedule} as cron.");
				}
				return Options.Create(cronExpression!);
			})
			.BuildServiceProvider();

		var options = provider.GetRequiredService<IOptions<CronExpression>>();

		Assert.NotNull(options);
		Assert.NotNull(options.Value);
		var actual = options.Value.GetNextOccurrence(_now, TimeZoneInfo.Utc);
		Assert.Equal(DateTimeOffset.Parse(expected), actual!.Value);
	}
}
