using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace StkCommon.UI.Wpf.Converters
{
	/// <summary>
	/// Базовый класс для конвертеров нескольких параметров
	/// Наследование от MarkupExtension позволяет не инстанцировать конвертер в ресурсах xaml
	/// </summary>
	/// <example>
	///     //<Label>
	///		//<Label.Content>
	///		//		<MultiBinding Converter="{Converters:FirstLastMultiValueConverter}">
	///		//			<Binding Path="FirstName" />
	///		//			<Binding Path="LastName" />
	///		//		</MultiBinding>
	///		//	</Label.Content>
	///		//</Label>
	/// </example>
	public abstract class MultiValueConverterBase : MarkupExtension, IMultiValueConverter
	{
		public MultiValueConverterBase()
		{ }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}

		public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);
		public abstract object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);
	}
}
