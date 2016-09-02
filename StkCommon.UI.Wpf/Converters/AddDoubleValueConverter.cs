using System;
using System.Globalization;

namespace StkCommon.UI.Wpf.Converters
{
	/// <summary>
	/// Если value double и параметр строка, которая конвертируется в double, тосложить их и вернуть результат
	/// </summary>
	public class AddDoubleValueConverter : ValueConverterBase
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double addValue;
			if (!(value is double) || !(parameter is string) || !double.TryParse((string)parameter, out addValue))
				return value;
			return (double)value + addValue;
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
