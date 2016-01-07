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
		//TODO: поддержать возможность указывать строку форматирования в count1Name и т.д и сделать правки в конвертере
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
			var index2 = count % 100;
			if (index2 >= 11 && index2 <= 20)
			{
				return countManyName;
			}
			var index = count % 10;
			switch (index)
			{
				case 1: return count1Name;
				case 2:
				case 3:
				case 4: return count234Name;
				default: return countManyName;
			}
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
			if (separator == null) throw new ArgumentNullException("separator");
			if (values == null) throw new ArgumentNullException("values");

			return string.Join(separator, values.Where(p => !string.IsNullOrWhiteSpace(p))).Trim();
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
