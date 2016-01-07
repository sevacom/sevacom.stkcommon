using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.UI.Wpf.Converters;

namespace StkCommon.UI.Wpf.Test.Converters
{
	[TestFixture]
	public class EmptyStringConverterBaseTest
	{
		private object _emptyStringObject;
		private object _notEmptyStringObject;
		private EmptyStringConverter _target;

		private class EmptyStringConverter: EmptyStringConverterBase<object>
		{
			public EmptyStringConverter(object trueValue, object falseValue) : base(trueValue, falseValue)
			{
			}
		}

		[SetUp]
		public void SetUpTest()
		{
			_emptyStringObject = new object();
			_notEmptyStringObject = new object();
			_target = new EmptyStringConverter(_emptyStringObject, _notEmptyStringObject);
		}

		/// <summary>
		/// Проверка что при подаче true возвращается объект True, при подаче false возвращает объект False
		/// </summary>
		[TestCase(null, true, true)]
		[TestCase("", true, true)]
		[TestCase("   ", true, true)]
		[TestCase("   ", false, false)]
		[TestCase("ddd", false, false)]
		public void ShouldConvertStringValue(string value, bool isCheckWhiteSpace, bool isEmptyStringObject)
		{
			//Given
			var expectedObject = isEmptyStringObject ? _target.EmptyString : _target.NotEmptyString;
			_target.IsCheckWhiteSpace = isCheckWhiteSpace;

			//When
			var actualValue = _target.Convert(value, typeof(object), null, CultureInfo.InvariantCulture);

			//Then
			actualValue.Should().Be(expectedObject, "После конвертации вернулся не тот объект");
		}
	}
}