using System;
using StkCommon.Data.Text;

namespace StkCommon.Data.Diagnostics
{
	public class ErrorMessageFormater
	{
		/// <summary>
		/// Формирует сообщение об ошибке. message + separator + exception.Message
		/// Если в exception нет сообщения или exception = null, то выводится только message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		/// <param name="separator">null соответствует одинарному переводу строки</param>
		/// <returns></returns>
		public static string FormatErrorMessage(string message, Exception exception = null, string separator = null)
		{
			if (exception == null)
				return message;

			if (separator == null)
				separator = Environment.NewLine;

			return TextExtensions.JoinNotEmpty(separator, message, exception.Message);
		}
	}
}