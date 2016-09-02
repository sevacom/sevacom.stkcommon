using System.Globalization;
using System.Windows.Media;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.UI.Wpf.Converters;

namespace StkCommon.UI.Wpf.Test.Converters
{
	[TestFixture]
	public class ReadableColorConverterTest
	{
		private ReadableColorConverter _target;

		[SetUp]
		public void SetUp()
		{
			_target = new ReadableColorConverter();
		}

		private readonly object[] _readableColorsSource = 
		{
			new object[] {Colors.Red, Colors.White},
			new object[] {Colors.White, Colors.Black},
			new object[] {Colors.Black, Colors.White},
			new object[] {Brushes.Red, Colors.White},
			new object[] {Brushes.White, Colors.Black},
			new object[] {Brushes.Black, Colors.White}
		};

		[TestCaseSource("_readableColorsSource")]
		public void ShouldConvertToReadableColor(object value, Color expectedColor)
		{
			//Given //When
			var actualValue = _target.Convert(value, typeof(Color), null,
				CultureInfo.InvariantCulture);

			//Then
			actualValue.Should().Be(expectedColor);
		}
	}
}