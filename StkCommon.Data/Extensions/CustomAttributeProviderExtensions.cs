using System;
using System.Reflection;

namespace StkCommon.Data.Extensions
{
	/// <summary>
	///     Various extension methods for classes implementing <see cref="ICustomAttributeProvider"/>
	/// </summary>
	public static class CustomAttributeProviderExtensions
	{
		/// <summary>
		///     Строготипизированная версия метода GetCustomAttributes
		/// </summary>
		/// <param name="attributeProvider"></param>
		/// <param name="inherit"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider attributeProvider, 
			bool inherit)
			where T : Attribute
		{
			return (T[])attributeProvider.GetCustomAttributes(typeof(T), inherit);
		}

		/// <summary>
		/// Строготипизированная версия метода GetCustomAttribute
		/// </summary>
		/// <param name="member"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetCustomAttribute<T>(this MemberInfo member)
			where T : Attribute
		{
			
			return Attribute.GetCustomAttribute(member, typeof (T)) as T;
		}
	}
}
