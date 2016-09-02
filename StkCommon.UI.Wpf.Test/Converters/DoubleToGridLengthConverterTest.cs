using System.Globalization;
using System.Windows;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.UI.Wpf.Converters;

namespace StkCommon.UI.Wpf.Test.Converters
{
	[TestFixture]
	public class DoubleToGridLengthConverterTest
	{
		private DoubleToGridLengthConverter _target;

		[SetUp]
		public void SetUp()
		{
			_target = new DoubleToGridLengthConverter();
		}

		[TestCase(12.0, GridUnitType.Pixel, true, 12.0)]
		[TestCase(15.0, GridUnitType.Star, true, 15.0)]
		[TestCase("123", GridUnitType.Pixel, false)]
		[TestCase("123", GridUnitType.Star, false)]
		[TestCase(12, GridUnitType.Pixel, false)]
		public void ShouldConvert(object value, GridUnitType unitType, 
			bool isConverted, double? expectedDouble = null)
		{
			//Given
			_target.UnitType = unitType;

			//When
			var actualValue = _target.Convert(value, typeof(GridLength), null, 
				CultureInfo.InvariantCulture);

			//Then
			if (isConverted)
			{
				actualValue.Should().BeOfType<GridLength>();
				actualValue.As<GridLength>().Value.Should().Be(expectedDouble);
				actualValue.As<GridLength>().GridUnitType.Should().Be(unitType);
			}
			else
			{
				actualValue.Should().Be(value);
			}
		}	 
	}
}