using System;
using System.Globalization;
using System.Windows.Media;

namespace StkCommon.UI.Wpf.Converters
{
	/// <summary>
	/// Конвертация int в Color или Brush и обратно
	/// </summary>
	public class ColorConverter : ValueConverterBase
	{
		/// <summary>
		/// Color To int
		/// </summary>
		public static int ToInt(Color color)
		{
			var iCol = (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;
			return iCol;
		}

		/// <summary>
		/// integer from Color
		/// </summary>
		/// <param name="iCol"></param>
		/// <returns></returns>
		public static Color ToColor(int iCol)
		{
			var color = Color.FromArgb((byte)(iCol >> 24),
				(byte)(iCol >> 16),
				(byte)(iCol >> 8),
				(byte)iCol);
			//ставим полную непрозрачность
			//color.A = 255;
			return color;
		}

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is int)
			{
				if (targetType == typeof(Color))
					return ToColor((int)value);
				if (targetType == typeof(Brush))
					return new SolidColorBrush(ToColor((int)value));
			}
			return null;
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Color && targetType == typeof(int))
				return ToInt((Color)value);
			if (value is SolidColorBrush && targetType == typeof(int))
				return ToInt(((SolidColorBrush)value).Color);
			return null;
		}
	}
}
