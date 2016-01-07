using System;
using System.Globalization;
using NUnit.Framework;
using StkCommon.UI.Wpf.Converters;

namespace StkCommon.UI.Wpf.Test.Converters
{
	[TestFixture]
	public class FormatStringMultiConverterTest
	{
		/// <summary>
		/// Проверяет форматирование строки
		/// </summary>
		[Test]
		public void ShouldFormatString()
		{
			// Given
			var values = new object[] { 5, DateTime.Parse("2014-10-24T13:45:12") };
			var target = new FormatStringMultiConverter();

			// When
			var result = target.Convert(values, typeof(string), 
				"Найдено {0} объектов, последнее обновление {1:D} в {1:T}", CultureInfo.GetCultureInfo("ru-RU"));
			
			// Then
			Assert.AreEqual("Найдено 5 объектов, последнее обновление 24 октября 2014 г. в 13:45:12", result, "Неверно произведено форматирование");
		}

		/// <summary>
		/// Если тип, к которому надо преобразовать, не string, то должен кинуть NotSupportedException
		/// </summary>
		[Test]
		public void ShouldThrowExceptionIfInvalidTargetType()
		{
			// Given
			var testedType = typeof(int);
			var target = new FormatStringMultiConverter();

			// When
			// Then
			Assert.Throws<NotSupportedException>(() => target.Convert(null, testedType, null, CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Если первый параметр - не string, то должен кинуть ArgumentException
		/// </summary>
		[Test]
		public void ShouldThrowExceptionIfInvalidFirstBindingParameter()
		{
			// Given
			var values = new object[] {1, 1};
			var target = new FormatStringMultiConverter();

			// When
			// Then
			Assert.Throws<ArgumentException>(() => target.Convert(values, typeof(string), 1, CultureInfo.InvariantCulture));
		}
	}
}
