using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StkCommon.UI.Wpf.Converters
{
	/// <summary>
	/// Конвертер, сравнивает значение и parametr и возвращает Equal если true, NotEqual если false
	/// </summary>
	public abstract class EqualParameterConverterBase<T> : ValueConverterBase
	{
		/// <summary>
		/// Значение если value == parameter
		/// </summary>
		public T Equal { get; set; }

		/// <summary>
		/// Значение если value != parameter
		/// </summary>
		public T NotEqual { get; set; }

		protected EqualParameterConverterBase(T equal, T notEqual)
		{
			Equal = equal;
			NotEqual = notEqual;
		}

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null && parameter == null)
				return Equal;

			if (value == null)
				return NotEqual;

			return value.Equals(parameter) ? Equal : NotEqual;
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value.Equals(Equal) ? parameter : Binding.DoNothing;
		}
	}

	/// <summary>
	/// Конвертер, сравнивает значение и parametr и возвращает bool
	/// </summary>
	public class EqualParameterToBoolConverter: EqualParameterConverterBase<bool>
	{
		public EqualParameterToBoolConverter() : base(true, false)
		{
		}
	}

	/// <summary>
	/// Конвертер, сравнивает значение и parametr и возвращает Visible если true, Collapsed если false
	/// </summary>
	public class EqualParameterToVisibilityConverter : EqualParameterConverterBase<Visibility>
	{
		public EqualParameterToVisibilityConverter()
			: base(Visibility.Visible, Visibility.Collapsed)
		{
		}
	}
}