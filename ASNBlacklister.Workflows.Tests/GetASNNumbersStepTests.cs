using Microsoft.Extensions.Options;
using Moq;
using WorkflowCore.Interface;
using Xunit;

namespace ASNDenier.Workflows.Tests
{
	public class GetASNNumbersStepTests
	{
		[Theory]
		[InlineData(1)]
		[InlineData(1, 2, 3)]
		public void InjectNumbers(params int[] numbers)
		{
			// Arrange
			var asnNumbers = new Models.ASNNumbers();
			asnNumbers.AddRange(numbers);
			var options = Options.Create(asnNumbers);

			// Act
			var sut = new Steps.GetASNNumbersStep(options);
			var result = sut.Run(Mock.Of<IStepExecutionContext>());

			// Assert
			Assert.True(result.Proceed);
			Assert.Equal(numbers, sut.ASNNumbers);
		}
	}
}
