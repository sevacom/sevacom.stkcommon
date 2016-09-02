using System;
using System.ComponentModel;
using System.Reflection;
using StkCommon.Data.Extensions;

namespace StkCommon.Data.Common
{
	public static class DescriptionExtensions
	{
		/// <summary>
		/// Получить описание значения перечисления из атрибута Description
		/// </summary>
		/// <param name="value">Значение перечисления</param>
		/// <param name="showValueIfNotDescription">Признак отображения значения при отсутствии атрибута</param>
		[Obsolete("Использовать из EnumExtensions")]
		public static string GetDescriptionFromAttribute(this Enum value, bool showValueIfNotDescription = false)
		{
			return value.GetDescription(showValueIfNotDescription);
		}

		/// <summary>
		/// Получить описание значения филда если есть атрибут DescriptionAttribute
		/// </summary>
		/// <param name="field"></param>
		/// <param name="defaultValue">если нет атрибута</param>
		/// <returns></returns>
		[Obsolete("Использовать GetDescription")]
		public static string GetDescriptionByField(MemberInfo field, string defaultValue = null)
		{
			return field.GetDescription(defaultValue);
		}

		/// <summary>
		/// Получить описание значения филда если есть атрибут DescriptionAttribute
		/// </summary>
		/// <param name="field"></param>
		/// <param name="defaultValue">если нет атрибута</param>
		/// <returns></returns>
		public static string GetDescription(this MemberInfo field, string defaultValue = null)
		{
			if (field == null) throw new ArgumentNullException("field");

			var attribute = field.GetCustomAttribute<DescriptionAttribute>();
			return attribute != null ? attribute.Description : defaultValue;
		}
	}
}
