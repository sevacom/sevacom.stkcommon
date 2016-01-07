using System.Globalization;
using System.Windows.Data;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.UI.Wpf.Converters;

namespace StkCommon.UI.Wpf.Test.Converters
{
	public class EqualToParameterConverterTest
	{
		private enum TestEnum {Val1, Val2}
		
		private class TestBoolEqualParameterConverter: EqualParameterConverterBase<bool>
		{
			public TestBoolEqualParameterConverter(bool equal, bool notEqual) : base(equal, notEqual)
			{
			}
		}

		private TestBoolEqualParameterConverter _target;

		[SetUp]
		public void SetUpTest()
		{
			_target = new TestBoolEqualParameterConverter(true, false);
		}

		/// <summary>
		/// Проверка конвертации сравнения с параметром 
		/// </summary>
		[TestCase("", "", true)]
		[TestCase("abc", "abc", true)]
		[TestCase("abc", "abcd", false)]
		[TestCase(null, null, true)]
		[TestCase(true, true, true)]
		[TestCase(true, false, false)]
		[TestCase(1, 1, true)]
		[TestCase(1, 2, false)]
		[TestCase(1.0, 1.0, true)]
		[TestCase(1.0, 2.0, false)]
		[TestCase(1.0, 1, false)]
		[TestCase(TestEnum.Val1, TestEnum.Val2, false)]
		[TestCase(TestEnum.Val1, TestEnum.Val1, true)]
		public void ShouldConvertWithCheckEqual(object value, object parameter, bool expectedValue)
		{
			//Given
			//When
			var actualValue = _target.Convert(value, typeof (bool), parameter, CultureInfo.InvariantCulture);

			//Then
			actualValue.Should().Be(expectedValue, "Не удалось правильно конвертировать результат");
		}

		/// <summary>
		/// Проверка обратной конвертации если true то возвращаем параметр, если false то Binding.DoNothing
		/// expectedValue если null значит Binding.DoNothing
		/// </summary>
		[TestCase(true, "12345", "12345")]
		[TestCase(true, 1, 1)]
		[TestCase(false, 1, null)]
		[TestCase(false, "12345", null)]
		public void ShouldConvertBack(bool value, object parameter, object expectedValue)
		{
			//Given
			if (expectedValue == null)
				expectedValue = Binding.DoNothing;

			//When
			var actualValue = _target.ConvertBack(value, typeof(object), parameter, CultureInfo.InvariantCulture);

			//Then
			actualValue.Should().Be(expectedValue, "Не удалось правильно конвертировать результат");
		}


	}
}