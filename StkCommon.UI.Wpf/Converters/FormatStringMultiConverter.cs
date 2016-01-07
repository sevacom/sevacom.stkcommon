using System;
using System.Globalization;

namespace StkCommon.UI.Wpf.Converters
{
	/// <summary>
	/// Форматирование строки values - значения, parameter - формат
	/// parameter должен быть строкой
	/// </summary>
	public class FormatStringMultiConverter : MultiValueConverterBase
	{
		public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(string))
				throw new NotSupportedException("Преобразование к типу \"" + targetType + "\" не поддерживается.");

			var formatString = parameter as string;
			if (formatString == null)
				throw new ArgumentException("Необходимо указать не пустую строку форматирования через параметр.");

			return string.Format(culture, formatString, values);
		}

		public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
