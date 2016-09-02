using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.UI.Wpf.Converters;

namespace StkCommon.UI.Wpf.Test.Converters
{
	[TestFixture]
	public class JoinStringsWithSeparatorConverterTest
	{
		private JoinStringsWithSeparatorConverter _target;

		[SetUp]
		public void SetUp()
		{
			_target = new JoinStringsWithSeparatorConverter();
		}

		private readonly object[] _joinStringsSource = 
		{
			new object[] { new[]{"1","2","3"}, null, "1, 2, 3"},
			new object[] { new[]{"1","2","3"}, 5, "1, 2, 3"},
			new object[] { new[]{"1","2","3"}, "; ", "1; 2; 3"},
			new object[] { "123", null, "123"},
			new object[] { 123, null, 123}
		};

		[TestCaseSource("_joinStringsSource")]
		public void ShouldConvert(object value, object param, object expected)
		{
			//Given //When
			var actualValue = _target.Convert(value, typeof(string), param,
				CultureInfo.InvariantCulture);

			//Then
			actualValue.Should().Be(expected);
		}
	}
}