using System;
using System.Globalization;
using System.Windows;

namespace StkCommon.UI.Wpf.Converters
{
	/// <summary>
	/// Конвертер, базовый класс для конвертации null объекта во что-то
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class NullConverterBase<T> : ValueConverterBase
	{
		protected NullConverterBase(T trueValue, T falseValue)
		{
			NullValue = trueValue;
			NotNullValue = falseValue;
		}

		/// <summary>
		/// Значение для null
		/// </summary>
		public T NullValue { get; set; }

		/// <summary>
		/// Значение для не null 
		/// </summary>
		public T NotNullValue { get; set; }

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value == null ? NullValue : NotNullValue;
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

	/// <summary>
	/// Конвертер, преобразование null в Visibility
	/// По умолчанию: null -> Visibility.Collapsed, not null -> Visible
	/// </summary>
	public class NullToVisibilityConverter : NullConverterBase<Visibility>
	{
		public NullToVisibilityConverter() : base(Visibility.Collapsed, Visibility.Visible) { }
	}

	/// <summary>
	/// Конвертер, преобразование null в bool
	/// По умолчанию: null -> false, not null -> true
	/// </summary>
	public class NullToBoolConverter : NullConverterBase<bool>
	{
		public NullToBoolConverter() : base(false, true) { }
	}
}