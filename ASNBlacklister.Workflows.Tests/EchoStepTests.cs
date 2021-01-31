using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using WorkflowCore.Interface;
using Xunit;

namespace ASNDenier.Workflows.Tests
{
	public class EchoStepTests
	{
		[Theory]
		[InlineData("ahitulahwfiltfhwt")]
		public void EchoMessage_SendsStringToLogger(string message)
		{
			// Arrange
			var logger = new MockLogger<Steps.EchoStep>();

			var sut = new Steps.EchoStep(logger)
			{
				Message = message,
			};

			// Act
			sut.Run(Mock.Of<IStepExecutionContext>());

			// Assert
			Assert.NotEmpty(logger.Entries);
			Assert.Single(logger.Entries);
			Assert.Contains(LogLevel.Information, logger.Entries.Keys);
			Assert.NotNull(logger.Entries[LogLevel.Information]);
			Assert.Equal(message, logger.Entries[LogLevel.Information]);
		}

		public class MockLogger<T> : ILogger<T>
		{
			public IDictionary<LogLevel, string> Entries { get; } = new Dictionary<LogLevel, string>();

			public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();

			public bool IsEnabled(LogLevel logLevel) => throw new NotImplementedException();

			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
			{
				var message = formatter(state, exception);
				Entries.Add(logLevel, message);
			}
		}
	}
}
