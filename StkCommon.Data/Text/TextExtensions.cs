using System;
using System.Collections.Generic;
using System.Linq;

namespace StkCommon.Data.Text
{
	/// <summary>
	/// Расширения для работы с текстом
	/// </summary>
	public static class TextExtensions
    {
		/// <summary>
		/// Правильно выбирает вариант текста исходя из числа
		/// </summary>
		/// <param name="count">число для которого надо окончание</param>
		/// <param name="count1Name">что подставить если цифра заканчивается на 1 (1 сеанс)</param>
		/// <param name="countManyName">что подставить если множественное число (25 сеансов)</param>
		/// <param name="count234Name">что подставить если число заканчивается на 2,3,4 (24 сеанса)</param>
		/// <returns></returns>
		public static string NumeralText(int count, string count1Name, string countManyName, string count234Name)
		{
			return NumeralTextEx(count, count1Name, countManyName, count234Name);
		}

		/// <summary>
		/// Правильно выбирает вариант текста исходя из числа
		/// </summary>
		/// <param name="count">число для которого надо окончание</param>
		/// <param name="count1Name">что подставить если цифра заканчивается на 1 (1 сеанс)</param>
		/// <param name="countManyName">что подставить если множественное число (25 сеансов)</param>
		/// <param name="count234Name">что подставить если число заканчивается на 2,3,4 (24 сеанса)</param>
		/// <param name="countZero">Текст если знчение 0, если не задан то используется countManyNameFormat</param>
		/// <returns></returns>
		public static string NumeralTextEx(int count, string count1Name, string countManyName,
			string count234Name, string countZero = null)
		{
			if (count == 0 && !string.IsNullOrEmpty(countZero))
				return countZero;
		
			var significantValue = count % 100;
			if (significantValue >= 10 && significantValue <= 20)
				return countManyName;

			var lastDigit = count % 10;

			switch (lastDigit)
			{
				case 1:
					return count1Name;
				case 2:
				case 3:
				case 4:
					return count234Name;
				default:
					return countManyName;
			}
		}

		/// <summary>
		/// Правильно выбирает вариант текста исходя из числа
		/// Строковые параметры count поддерживают форматирование '{0}'
		/// </summary>
		/// <param name="count">число для которого надо окончание</param>
		/// <param name="count1NameFormat">что подставить если цифра заканчивается на 1 (1 сеанс)</param>
		/// <param name="countManyNameFormat">что подставить если множественное число (25 сеансов)</param>
		/// <param name="count234NameFormat">что подставить если число заканчивается на 2,3,4 (24 сеанса)</param>
		/// <param name="countZeroFormat">Текст если знчение 0, если не задан то используется countManyNameFormat</param>
		/// <returns></returns>
		public static string NumeralTextFormat(int count, string count1NameFormat,
			string countManyNameFormat, string count234NameFormat, string countZeroFormat = null)
		{
			var text = NumeralTextEx(count, count1NameFormat, countManyNameFormat, count234NameFormat, countZeroFormat);
			return !string.IsNullOrEmpty(text) ? string.Format(text, count) : text;
		}

		/// <summary>
		/// Объеденить только непустые (white-space символы считаются пустыми) значения через разделитель
		/// </summary>
		/// <param name="separator">Разделитель</param>
		/// <param name="values">Список значений</param>
		/// <returns></returns>
		public static string JoinNotEmpty(string separator, params string[] values)
		{
			return JoinNotEmpty(separator, (IEnumerable<string>)values);
		}

		/// <summary>
		/// Объеденить только непустые (white-space символы считаются пустыми) значения через разделитель
		/// </summary>
		public static string JoinNotEmpty(string separator, IEnumerable<string> values)
		{
			if (separator == null) throw new ArgumentNullException(nameof(separator));
			if (values == null) throw new ArgumentNullException(nameof(values));

		    return values.JoinNotEmpty(separator, true);
		}

        /// <summary>
		/// Объеденить только непустые значения через разделитель
		/// </summary>
		public static string JoinNotEmpty(this IEnumerable<string> values, string separator, bool checkWhiteSpace)
        {
            if (separator == null) throw new ArgumentNullException(nameof(separator));
            if (values == null) throw new ArgumentNullException(nameof(values));

            var valuesTmp = checkWhiteSpace
                    ? values.Where(p => !string.IsNullOrWhiteSpace(p))
                    : values.Where(p => !string.IsNullOrEmpty(p));

            return string.Join(separator, valuesTmp).Trim();
        }

        /// <summary>
        /// TrimIfNotNull
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimIfNotNull(this string str)
		{
			return !string.IsNullOrEmpty(str) ? str.Trim() : str;
		}

		/// <summary>
		/// Проверить вхождение подстроки с использованием типа сравнения
		/// </summary>
		public static bool Contains(this string source, string toCheck, StringComparison comp)
		{
			if (source == null || toCheck == null)
				return false;

			return source.IndexOf(toCheck, comp) >= 0;
		}
    }
}
