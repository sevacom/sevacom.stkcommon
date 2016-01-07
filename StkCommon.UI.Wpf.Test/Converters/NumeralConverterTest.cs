using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.UI.Wpf.Converters;

namespace StkCommon.UI.Wpf.Test.Converters
{
	[TestFixture]
	public class NumeralConverterTest
	{
		private NumeralConverter _target;

		[SetUp]
		public void SetUpTest()
		{
			_target = new NumeralConverter();
		}

		[TestCase(1, null, "чемодан , чемоданов, чемодана, нет чемодана", "чемодан")]
		[TestCase(10, null, "чемодан, чемоданов , чемодана, нет чемодана", "чемоданов")]
		[TestCase(2, null, "чемодан, чемоданов, чемодана , нет чемодана", "чемодана")]

		[TestCase(0, null, "чемодан, чемоданов, чемодана", "чемоданов")]
		[TestCase(0, null, "чемодан, чемоданов, чемодана, нет_чемодана", "нет_чемодана")]
		[TestCase(0, null, "чемодан, чемоданов, чемодана,  нет_чемодана ", "нет_чемодана")]
		[TestCase(0, ",", "чемодан, чемоданов, чемодана, нет чемодана", "нет чемодана")]
		[TestCase(0, ",", "invalid string ", 0)]

		[TestCase(1, ",", "{0} чемодан , чемоданов, чемодана, нет чемодана", "1 чемодан")]
		[TestCase(10, ",", "чемодан, {0} чемоданов , чемодана, нет чемодана", "10 чемоданов")]
		[TestCase(2, ",", "чемодан, чемоданов, {0} чемодана , нет чемодана", "2 чемодана")]
		[TestCase(0, ",", "чемодан, {0} чемоданов, чемодана", "0 чемоданов")]
		public void ShouldConvertToNumeralFromParameter(int number, string separator, string parameter, object expectedResult)
		{
			//Given
			if (!string.IsNullOrEmpty(separator))
				_target.Separator = separator;

			//When
			var result = _target.Convert(number, typeof (string), parameter, CultureInfo.InvariantCulture);

			//Then
			result.Should().Be(expectedResult);
		}
	}
}