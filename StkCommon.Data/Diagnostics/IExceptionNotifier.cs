using System;

namespace StkCommon.Data.Diagnostics
{
	/// <summary>
	/// Механизм дополнителного оповещение об исключениях.
	/// </summary>
	public interface IExceptionNotifier
	{
		/// <summary>
		/// Оповестить об исключении.
		/// </summary>
		/// <param name="exception">Исключение для оповещения.</param>
		/// <param name="errorMessage">Дополнительное сообщение к исключению.</param>
		/// <param name="errorId">Идентификатор ошибки</param>
		void Notify(Exception exception, string errorMessage, string errorId = null);


		/// <summary>
		/// Оповестить об ошибке.
		/// </summary>
		/// <param name="errorMessage">сообщение исключения.</param>
		/// <param name="errorId">Идентификатор ошибки</param>
		void Notify(string errorMessage, string errorId = null);
	}
}