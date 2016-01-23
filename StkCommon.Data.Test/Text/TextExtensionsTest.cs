using FluentAssertions;
using NUnit.Framework;
using StkCommon.Data.Text;

namespace StkCommon.Data.Test.Text
{
	[TestFixture]
	public class TextExtensionsTest
    {
		private const string Count1Name = "Разговор {0}";
		private const string CountManyName = "Разговоров {0}";
		private const string Count234Name = "Разговора {0}";

		/// <summary>
		/// Должен правильно вычислять числительные
		/// </summary>
		[TestCase(1, Count1Name)]
		[TestCase(12, CountManyName)]
		[TestCase(22, Count234Name)]
		[TestCase(10111, CountManyName)]
		public void ShouldNumeralText(int count, string expectedResult)
		{
			//Given //When
			var actualResult = TextExtensions.NumeralText(count, Count1Name, CountManyName, Count234Name);

			//Then
			actualResult.Should().Be(expectedResult);
		}

		/// <summary>
		/// Должен правильно вычислять числительные
		/// </summary>
		[TestCase(1, "Разговор 1")]
		[TestCase(12, "Разговоров 12")]
		[TestCase(22, "Разговора 22")]
		[TestCase(10111, "Разговоров 10111")]
		public void ShouldNumeralTextFormat(int count, string expectedResult)
		{
			//Given //When
			var actualResult = TextExtensions.NumeralTextFormat(count, Count1Name, CountManyName, Count234Name);

			//Then
			actualResult.Should().Be(expectedResult);
		}

		[TestCase("-", "1-2-3",		"1", "", "2", null, "3")]
		[TestCase(" ", "1 2 3",		"1", "2", "3", null, "")]
		[TestCase(" ", "1 2 3",		null, "", "1", "2", "3")]
		public void ShouldJoinNotEmpty(string separator, string expectedValue, params string[] values)
		{
			//Given //When
			var actualValue = TextExtensions.JoinNotEmpty(separator, values);

			//Then
			expectedValue.Should().Be(actualValue, "Объединение прошло неправильно");
		}
    }
}
