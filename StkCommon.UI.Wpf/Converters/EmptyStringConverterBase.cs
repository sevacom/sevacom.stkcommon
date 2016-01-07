using System;
using System.Globalization;
using System.Windows;

namespace StkCommon.UI.Wpf.Converters
{
	/// <summary>
	/// Конвертер, базовый класс для конвертации пустой строки во что-то
	/// Если входное значение не соответствует string то возвращается EmptyString значение 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class EmptyStringConverterBase<T> : ValueConverterBase
	{
		protected EmptyStringConverterBase(T trueValue, T falseValue)
		{
			EmptyString = trueValue;
			NotEmptyString = falseValue;
			IsCheckWhiteSpace = true;
		}

		/// <summary>
		/// Значение для пустой строки
		/// </summary>
		public T EmptyString { get; set; }
		
		/// <summary>
		/// Значение для не пустой строки
		/// </summary>
		public T NotEmptyString { get; set; }

		/// <summary>
		/// Считать white-space сиволы пустой строкой или нет
		/// </summary>
		public bool IsCheckWhiteSpace { get; set; }

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var str = value as string;
			return IsCheckWhiteSpace
						? (string.IsNullOrWhiteSpace(str) ? EmptyString : NotEmptyString)
						: (string.IsNullOrEmpty(str) ? EmptyString : NotEmptyString);
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

	/// <summary>
	/// Конвертер, преобразование пустоты строки в Visibility
	/// По умолчанию: пустая строка -> Collapsed, не пустая -> Visible
	/// </summary>
	public class EmptyStringToVisibilityConverter : EmptyStringConverterBase<Visibility>
	{
		public EmptyStringToVisibilityConverter()
			: base(Visibility.Collapsed, Visibility.Visible)
		{
		}
	}

	/// <summary>
	/// Конвертер, преобразование пустоты строки в bool
	/// По умолчанию: пустая строка -> true, не пустая -> false
	/// </summary>
	public class EmptyStringToBoolConverter : EmptyStringConverterBase<bool>
	{
		public EmptyStringToBoolConverter()
			: base(true, false)
		{
		}
	}

	/// <summary>
	/// Конвертер, преобразование пустой строки в параметр, если значение не пустое, то возвращается само значение.
	/// Если параметр пустой, то возвращает само значение
	/// </summary>
	public class EmptyStringToParameterConverter : ValueConverterBase
	{
		/// <summary>
		/// Считать white-space сиволы пустой строкой или нет
		/// </summary>
		public bool IsCheckWhiteSpace { get; set; }

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var str = value as string;
			var param = parameter as string;

			var isParameterEmpty = IsCheckWhiteSpace ? string.IsNullOrWhiteSpace(param) : string.IsNullOrEmpty(param);
			if (isParameterEmpty)
				return str;

			var isStringEmpty = IsCheckWhiteSpace ? string.IsNullOrWhiteSpace(str) : string.IsNullOrEmpty(str);
			return isStringEmpty ? param : str;
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
