using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StkCommon.UI.Wpf.Converters
{
	/// <summary>
	/// Получить цвет, который будет видно на фоне переданного
	/// Поддерживается цвет int, Color, SolidColorBrush
	/// </summary>
	public class ReadableColorConverter : ValueConverterBase
	{
		/// <summary>
		/// Получить цвет, который будет видно на фоне backColor
		/// </summary>
		public static Color GetReadableForeColor(Color backColor)
		{
			return (backColor.R + backColor.B + backColor.G) / 3 > 128 ?
				Colors.Black : Colors.White;
		}

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Color color;
			if (value is int)
				color = ColorConverter.ToColor((int)value);
			else if (value is Color)
				color = (Color)value;
			else if (value is SolidColorBrush)
				color = ((SolidColorBrush)value).Color;
			else
				return value;

			if (color == Colors.Transparent)
				return Binding.DoNothing;

			color = GetReadableForeColor(color);

			if (targetType == typeof(Color))
				return color;
			if (targetType == typeof(Brush))
				return new SolidColorBrush(color);

			return Binding.DoNothing;
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}