using FluentAssertions;
using NUnit.Framework;
using StkCommon.Data.Text;

namespace StkCommon.Data.Test.Text
{
	[TestFixture]
	public class TextExtensionsTest
    {
		/// <summary>
		/// Должен правильно вычислять числительные
		/// </summary>
		[TestCase(1, "Разговор", "Разговоров", "Разговора", "Разговор")]
		[TestCase(12, "Разговор", "Разговоров", "Разговора", "Разговоров")]
		[TestCase(22, "Разговор", "Разговоров", "Разговора", "Разговора")]
		[TestCase(10111, "Разговор", "Разговоров", "Разговора", "Разговоров")]
		[TestCase(1, "Разговор {0}{1}", "Разговоров {0}", "Разговора {0}", "Разговор {0}{1}")]
		public void ShouldNumeralText(int count, string count1Name, string countManyName, string count234Name, string expectedResult)
		{
			//Given //When
			var actualResult = TextExtensions.NumeralText(count, count1Name, countManyName, count234Name);

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
