using System;
using System.Collections.Generic;
using System.Globalization;
using StkCommon.Data.Text;

namespace StkCommon.UI.Wpf.Converters
{
	/// <summary>
	/// объндинение коллекции строк через Separator. Если разделитель указан через parameter то используется он
	/// Разделитель по умолчанию ', '
	/// </summary>
	public class JoinStringsWithSeparatorConverter: ValueConverterBase
	{
		public JoinStringsWithSeparatorConverter()
		{
			Separator = ", ";
		}

		/// <summary>
		/// Разделитель списка строковых значений,
		/// по умолчанию равен ', '
		/// </summary>
		public string Separator { get; set; }

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var stringValues = value as IEnumerable<string>;
			if (stringValues == null)
				return value;

			var separator = Separator;
			if (parameter is string)
				separator = (string)parameter;

			return TextExtensions.JoinNotEmpty(separator, stringValues);
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
