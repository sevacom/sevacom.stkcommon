using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.UI.Wpf.Converters;

namespace StkCommon.UI.Wpf.Test.Converters
{
	[TestFixture]
	public class BoolConverterBaseTest
	{
		private object _trueObject;
		private object _falseObject;
		private BoolConverter _target;

		private class BoolConverter: BoolConverterBase<object>
		{
			public BoolConverter(object trueValue, object falseValue) : base(trueValue, falseValue)
			{
			}
		}

		[SetUp]
		public void SetUpTest()
		{
			_trueObject = new object();
			_falseObject = new object();
			_target = new BoolConverter(_trueObject, _falseObject);
		}

		/// <summary>
		/// Проверка что при подаче true возвращается объект True, при подаче false возвращает объект False
		/// </summary>
		[TestCase(true, true)]
		[TestCase(false, false)]
		public void ShouldConvertBoolValue(bool value, bool expectedValue)
		{
			//Given
			var expectedObject = expectedValue ? _target.True : _target.False;

			//When
			var actualValue = _target.Convert(value, typeof(object), null, CultureInfo.InvariantCulture);
			
			//Then
			actualValue.Should().Be(expectedObject, "После конвертации вернулся не тот объект");
		}

		/// <summary>
		/// Должен конвертировать в обрантную сторону из TrueObject в true
		/// </summary>
		[Test]
		public void ShouldConvertBackTrueObjectToTrue()
		{
			//Given
			//When
			var actualValue = _target.ConvertBack(_trueObject, typeof (bool), null, CultureInfo.InvariantCulture);
			
			//Then
			actualValue.Should().Be(true, "обратная конвертация для TrueObject не работает");
		}

		/// <summary>
		/// Должен конвертировать в обрантную сторону из FalseObject в false
		/// </summary>
		[Test]
		public void ShouldConvertBackFalseObjectToFalse()
		{
			//Given
			//When
			var actualValue = _target.ConvertBack(_falseObject, typeof(bool), null, CultureInfo.InvariantCulture);

			//Then
			actualValue.Should().Be(false, "обратная конвертация для FalseObject не работает");
		}

		/// <summary>
		/// Проверка что при подаче значение не bool возвращается объект False
		/// </summary>
		[Test]
		public void ShouldReturnFalseObjectWhenValueTypeNotBool()
		{
			//Given
			//When
			var actualValue = _target.Convert(1, typeof(object), null, CultureInfo.InvariantCulture);

			//Then
			actualValue.Should().Be( _target.False, "После конвертации вернулся не объект False");
		}
	}
}
