using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace StkCommon.Data.Extensions
{
	/// <summary>
	/// Методы расширения для перечислений
	/// </summary>
	public static class EnumExtensions
	{
		/// <summary>
		/// Взять значение атрибута Description
		/// </summary>
		/// <param name="member">элемент перечисления</param>
		/// <param name="valueIfNotDescription"></param>
		/// <returns></returns>
		public static string GetDescription(this Enum member, bool valueIfNotDescription = false)
		{
			if (member.GetType().IsEnum == false)
				throw new ArgumentOutOfRangeException("member", "member is not enum");

			var defaultValue = valueIfNotDescription ? member.ToString() : string.Empty;

			var fieldInfo = member.GetType().GetField(member.ToString());

			if (fieldInfo == null)
				return defaultValue;

			var attribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();

			return attribute != null ? attribute.Description : defaultValue;
		}

		/// <summary>
		/// Создать список из перечисления
		/// </summary>
		public static IEnumerable<KeyValuePair<int, string>> ToKeyValuePairs<TEnum>() where TEnum : struct, IConvertible
		{
			if (typeof(TEnum).IsEnum == false)
				throw new ArgumentException("TEnum must be an enumerated type");

			var items = Enum.GetValues(typeof(TEnum))
				.Cast<Enum>()
				.Select(x => new KeyValuePair<int, string>(int.Parse(x.ToString("D")), x.GetDescription()))
				.ToList();

			return items;
		}
	}
}
