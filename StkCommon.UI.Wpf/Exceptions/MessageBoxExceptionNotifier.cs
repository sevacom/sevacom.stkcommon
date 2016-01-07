using System;
using System.Windows;
using StkCommon.Data.Diagnostics;
using StkCommon.UI.Wpf.Views;

namespace StkCommon.UI.Wpf.Exceptions
{
	/// <summary>
	/// Механизм оповещения пользователя о непредвиденных исключениях через <see cref="IShowDialogAgent"/>.
	/// Показывает полную детадизацию исключения.
	/// </summary>
	public class MessageBoxExceptionNotifier : IExceptionNotifier
	{
		private readonly IShowDialogAgent _dialogAgent;

		public MessageBoxExceptionNotifier(IShowDialogAgent dialogAgent)
		{
			if (dialogAgent == null) throw new ArgumentNullException("dialogAgent");
			_dialogAgent = dialogAgent;
		}

		#region IExceptionNotifier Members

		/// <summary>
		/// Оповестить об исключении.
		/// </summary>
		/// <param name="exception">Исключение для оповещения.</param>
		/// <param name="errorMessage">Дополнительное сообщение к исключению.</param>
		/// <param name="errorId">Идентификатор ошибки</param>
		public virtual void Notify(Exception exception, string errorMessage, string errorId = null)
		{
			if (exception == null) throw new ArgumentNullException("exception");

			ShowMessageBoxForUnexpectedException(_dialogAgent, exception, errorMessage, errorId);		
		}


		/// <summary>
		/// Оповестить об ошибке, отображает Warning
		/// </summary>
		/// <param name="errorMessage">Сообщение ошибки.</param>
		/// <param name="errorId">Идентификатор ошибки</param>
		public virtual void Notify(string errorMessage, string errorId = null)
		{
			if (string.IsNullOrEmpty(errorMessage)) 
				throw new ArgumentException("Сообщение не может быть пустым", "errorMessage");

			_dialogAgent.ShowMessageDialog(errorMessage, MessageBoxButton.OK, MessageBoxImage.Warning);	
		}

		#endregion

		private static void ShowMessageBoxForUnexpectedException(IShowDialogAgent dialogAgent, Exception ex,
			string errorMessage, string errorId = null)
		{
			var msg = string.Format("{0}\n\n{1}\n\n{2}", errorMessage, GetExMessage(ex), "Показать детализированное сообщение?")
				.TrimStart('\n');

			if (dialogAgent.ShowMessageDialog(msg, MessageBoxButton.YesNo, MessageBoxImage.Error) != MessageBoxResult.Yes)
				return;

			ShowExceptionDetails(dialogAgent, ex, errorMessage, errorId);
		}

		private static void ShowExceptionDetails(IShowDialogAgent dialogAgent, Exception ex, string errorMessage, string id)
		{
			var msgStack = "-" + GetExMessage(ex);
			var curEx = ex.InnerException;
			while (curEx != null)
			{
				msgStack = string.Format("{0}\n-{1}", msgStack, GetExMessage(curEx));
				curEx = curEx.InnerException;
			}

			var msg = string.Format("{0}\n\nДанная ошибка произошла по причине:\r\n{1}", errorMessage, msgStack)
				.TrimStart('\n');

			if (!string.IsNullOrEmpty(id))
				msg = string.Format("{0}\n\nИдентификатор ошибки: '{1}'.", msg, id);

			msg += "\n\nНажмите 'Ctrl+C', чтобы скопировать текст сообщения в буфер обмена.";

			dialogAgent.ShowMessageDialog(msg);
		}

		private static string GetExMessage(Exception ex)
		{
			if (ex == null) throw new ArgumentNullException("ex");

			return ex.Message;
		}
	}
}
