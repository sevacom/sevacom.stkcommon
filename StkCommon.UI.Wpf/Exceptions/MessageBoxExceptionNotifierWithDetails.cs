using System;
using StkCommon.Data.Diagnostics;
using StkCommon.UI.Wpf.Views;

namespace StkCommon.UI.Wpf.Exceptions
{
	/// <summary>
	/// Отображение ошибок с подробностями
	/// </summary>
	public class MessageBoxExceptionNotifierWithDetails: IExceptionNotifier
	{
		private readonly IShowDialogAgent _dialogAgent;

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="dialogAgent"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public MessageBoxExceptionNotifierWithDetails(IShowDialogAgent dialogAgent)
		{
			if (dialogAgent == null) throw new ArgumentNullException("dialogAgent");
			_dialogAgent = dialogAgent;
		}

		/// <summary>
		/// Оповестить об исключении.
		/// </summary>
		/// <param name="exception">Исключение для оповещения.</param>
		/// <param name="errorMessage">Дополнительное сообщение к исключению.</param>
		/// <param name="errorId">Идентификатор ошибки</param>
		public void Notify(Exception exception, string errorMessage, string errorId = null)
		{
			if (exception == null) throw new ArgumentNullException("exception");

			var message = string.IsNullOrEmpty(errorMessage)
				? string.Format("Произошла ошибка в ходе выполнения операции.{0}{0}{1}", Environment.NewLine, exception.Message)
				: errorMessage + Environment.NewLine + Environment.NewLine + exception.Message;
			_dialogAgent.ShowErrorMessageDialog(message, exception.ToString());
		}

		/// <summary>
		/// Оповестить об ошибке.
		/// </summary>
		/// <param name="errorMessage">сообщение исключения.</param>
		/// <param name="errorId">Идентификатор ошибки</param>
		public void Notify(string errorMessage, string errorId = null)
		{
			if (string.IsNullOrEmpty(errorMessage))
				throw new ArgumentException("Сообщение не может быть пустым", "errorMessage");

			_dialogAgent.ShowErrorMessageDialog(errorMessage, null);
		}
	}
}
