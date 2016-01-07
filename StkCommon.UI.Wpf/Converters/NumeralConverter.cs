using System;
using System.Globalization;
using StkCommon.Data.Text;

namespace StkCommon.UI.Wpf.Converters
{
	/// <summary>
	/// Преобразование числительнных, необходимо заполнить варианты текста для одного, для многих и для 2,3,4
	/// Опционально можно указать четвертый параметр, который будет возвращен, если конвертируемая величина равна нулю
    /// Можно указать через ConverterParameter: "день, дней, дня" (разделители можно указать в свойстве Separator, по умолчанию - пробел или запятая)
	/// Можно указать через свойства класса
	/// Если в вариантах текста встретится {0}, то он будет подменен на конвертируемое значение
	/// </summary>
	public class NumeralConverter : ValueConverterBase
	{
		private static readonly string[] Separators = {" ", ","};

		/// <summary>
		/// Вариант текста для одного
		/// </summary>
		public string Count1 { get; set; }

		/// <summary>
		/// Вариант текста для многих
		/// </summary>
		public string CountMany { get; set; }

		/// <summary>
		/// Вариант текста для 2,3,4
		/// </summary>
		public string Count234 { get; set; }

        /// <summary>
        /// Вариант текста для нуля (опционально)
        /// </summary>
        public string CountNothing { get; set; }

        /// <summary>
        /// Разделитель для значения параметра
        /// </summary>
        public string Separator { get; set; }

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int number;
			if (!TryParseValue(value, out number))
			{
				return value;
			}

			string count1;
			string countMany;
			string count234;
		    string countNothing;

		    var separators = string.IsNullOrEmpty(Separator) ? Separators : new[]{ Separator };

			if (!TryParseParameter(parameter, separators, out count1, out countMany, out count234, out countNothing))
			{
				count1 = Count1;
				countMany = CountMany;
				count234 = Count234;
			    countNothing = CountNothing;
			}

			count1 = FormatWithTrim(count1, number);
			countMany = FormatWithTrim(countMany, number);
			count234 = FormatWithTrim(count234, number);
			countNothing = FormatWithTrim(countNothing, number);

			if (string.IsNullOrWhiteSpace(count1) && 
				string.IsNullOrWhiteSpace(countMany) && 
				string.IsNullOrWhiteSpace(count234))
				return value;

		    if (number == 0 && !string.IsNullOrEmpty(countNothing))
				return countNothing;

			return TextExtensions.NumeralText(number, count1, countMany, count234);
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}

		private static bool TryParseParameter(object parameter, string[] separators, out string count1, out string countMany, out string count234, out string countNothing)
		{
			count1 = null;
			countMany = null;
			count234 = null;
		    countNothing = null;

			if (parameter is string)
			{
				var countItems = ((string)parameter).Split(separators, StringSplitOptions.RemoveEmptyEntries);
				if (countItems.Length >= 3)
				{
					count1 = countItems[0];
					countMany = countItems[1];
					count234 = countItems[2];
				    if (countItems.Length >= 4)
				        countNothing = countItems[3];
					return true;
				}
			}

			return false;
		}

		private static bool TryParseValue(object value, out int number)
		{
			number = 0;

			if (value is int)
			{
				number = (int) value;
				return true;
			}
			if (value is string)
			{
				return int.TryParse((string) value, out number);
			}
			return false;
		}

		private static string FormatWithTrim(string format, object value)
		{
			return !string.IsNullOrEmpty(format) ? string.Format(format.Trim(), value) : format;
		}
	}
}
