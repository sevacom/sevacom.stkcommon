using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace StkCommon.UI.Wpf.Converters
{
	/// <summary>
	/// Базовый класс для конвертеров
	/// Наследование от MarkupExtension позволяет не инстанцировать конвертер в ресурсах xaml
	/// </summary>
	/// <example>
	/// //<Image Source="{Binding Path=Icon}" Visibility="{Binding Icon, Converter={Converters:NullToInvisibilityConverter}}"/>
	/// </example>
	public abstract class ValueConverterBase : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}

		public abstract object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture);
		public abstract object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture);
	}
}