using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.UI.Wpf.Converters;

namespace StkCommon.UI.Wpf.Test.Converters
{
	[TestFixture]
	public class NullConverterBaseTest
	{
		private object _nullObject;
		private object _notNullObject;
		private NullConverter _target;

		private class NullConverter : NullConverterBase<object>
		{
			public NullConverter(object nullValue, object notNullValue)
				: base(nullValue, notNullValue)
			{
			}
		}

		[SetUp]
		public void SetUpTest()
		{
			_nullObject = new object();
			_notNullObject = new object();
			_target = new NullConverter(_nullObject, _notNullObject);
		}

		/// <summary>
		/// Проверка что при подаче null возвращается объект NullObject, при подаче не null возвращает объект NotNullObject
		/// </summary>
		[TestCase(null, true)]
		[TestCase("", false)]
		public void ShouldConvertStringValue(object value, bool isNullObject)
		{
			//Given
			var expectedObject = isNullObject ? _target.NullValue : _target.NotNullValue;

			//When
			var actualValue = _target.Convert(value, typeof(object), null, CultureInfo.InvariantCulture);

			//Then
			actualValue.Should().Be(expectedObject, "После конвертации вернулся не тот объект");
		}
	}
}