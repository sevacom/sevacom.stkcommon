using System;
using System.Linq;

namespace StkCommon.TestUtils
{
	public static class EnumsComparer
	{
		/// <summary>
		/// Сравнение двух разных enum-а на эквивалентность имен и значений
		/// exclude - список значений которые надо исключить из TFirst и TSecond
		/// </summary>
		public static bool Compare<TFirst, TSecond>(params Enum[] exclude)
			where TFirst : struct
			where TSecond : struct
		{
			var excludeFirst = exclude.OfType<TFirst>().ToArray();
			var excludeSecond = exclude.OfType<TSecond>().ToArray();

			var firstEnumNames = Enum.GetNames(typeof(TFirst))
				.Except(excludeFirst.Select(p => p.ToString()))
				.ToArray();

			var firstEnumValues = Enum.GetValues(typeof(TFirst))
				.Cast<int>()
				.Except(excludeFirst.Cast<int>())
				.ToArray();

			var secondEnumNames = Enum.GetNames(typeof(TSecond))
				.Except(excludeSecond.Select(p => p.ToString()))
				.ToArray();

			var secondEnumValues = Enum.GetValues(typeof(TSecond))
				.Cast<int>()
				.Except(excludeSecond.Cast<int>())
				.ToArray();

			if (firstEnumNames.Length != secondEnumNames.Length)
				return false;

			for (var i = 0; i < firstEnumNames.Length; i++)
			{
				if (firstEnumNames[i] != secondEnumNames[i])
					return false;
			}

			for (var i = 0; i < firstEnumValues.Length; i++)
			{
				if (firstEnumValues[i] != secondEnumValues[i])
					return false;
			}

			return true;
		}
	}
}
