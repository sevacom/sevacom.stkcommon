using System;
using System.Globalization;
using System.Linq;

namespace StkCommon.UI.Wpf.Converters
{
	/// <summary>
	/// Возвращает результат логической операции между значениями.
	/// Логическая операция задаётся через параметр LogicalOperation
	/// Если значение равно своему defaultValue, значит оно false, иначе true
	/// </summary>
	public class MultiLogicalAndConverter : MultiValueConverterBase
	{
		/// <summary>
		/// Тип логической операции
		/// </summary>
		public enum LogicalOperationType
		{
			And = 0,
			Or = 1
		}

		/// <summary>
		/// Тип логической операции, по умолчанию And
		/// </summary>
		public LogicalOperationType LogicalOperation { get; set; }

		public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(bool))
				throw new NotSupportedException("Преобразование к типу \"" + targetType + "\" не поддерживается");

			var boolValues = values.Select(ValueToBool);
			switch (LogicalOperation)
			{
				case LogicalOperationType.And:
					return boolValues.All(p => p);
				case LogicalOperationType.Or:
					return boolValues.Any(p => p);
				default:
					throw new NotSupportedException("Операция " + LogicalOperation + " не поддерживается.");
			}
		}

		public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		private static bool ValueToBool(object value)
		{
			if (value == null)
				return false;
			var defaultValue = GetDefaultValue(value.GetType());
			return !value.Equals(defaultValue);
		}

		private static object GetDefaultValue(Type type)
		{
			return type.IsValueType ? Activator.CreateInstance(type) : null;
		}
	}
}
