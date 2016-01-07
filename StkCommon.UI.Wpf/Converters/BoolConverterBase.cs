using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace StkCommon.UI.Wpf.Converters
{
	/// <summary>
	/// Конвертер, базовый класс для конвертации bool во что-то 
	/// Если передали не bool то возвращается значение False
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class BoolConverterBase<T> : ValueConverterBase
	{
		protected BoolConverterBase(T trueValue, T falseValue)
		{
			True = trueValue;
			False = falseValue;
		}

		/// <summary>
		/// Значение для параметра навного True
		/// </summary>
		public T True { get; set; }

		/// <summary>
		/// Значение для параметра равного False
		/// </summary>
		public T False { get; set; }

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is bool && (bool)value ? True : False;
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is T && EqualityComparer<T>.Default.Equals((T)value, True);
		}
	}

	/// <summary>
	/// Конвертер, инвертирует значение bool
	/// </summary>
	public class BoolInvertConverter : BoolConverterBase<bool>
	{
		public BoolInvertConverter() : base(false, true) { }
	}

	/// <summary>
	/// Конвертер, преобразование bool в Visibility.
	/// По умолчанию:  true -> Visible, false -> Collapsed 
	/// </summary>
	public sealed class BoolToVisibilityConverter : BoolConverterBase<Visibility>
	{
		public BoolToVisibilityConverter() :
			base(Visibility.Visible, Visibility.Collapsed) { }
	}
}