using System;
using StkCommon.Data.Text;

namespace StkCommon.Data.Diagnostics
{
	public class ErrorMessageFormater
	{
		/// <summary>
		/// ‘ормирует сообщение об ошибке. message + separator + exception.Message
		/// ≈сли в exception нет сообщени€ или exception = null, то выводитс€ только message
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