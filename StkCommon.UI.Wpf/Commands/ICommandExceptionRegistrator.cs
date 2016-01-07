using System;
using System.Windows.Input;

namespace StkCommon.UI.Wpf.Commands
{
	/// <summary>
	/// Интерфейс регистрации ошибок для команд
	/// </summary>
	public interface ICommandExceptionRegistrator
	{
		/// <summary>
		/// Зарегистрировать ошибку
		/// </summary>
		/// <param name="exception">Ошибка</param>
		/// <param name="command">Команда при выполнении которой произошла ошибка</param>
		void RegisterCommandException(Exception exception, ICommand command);
	}

	/// <summary>
	/// Регистрация ошибок при выполнении команд
	/// </summary>
	public class CommandExceptionRegistrator : ICommandExceptionRegistrator
	{
		private readonly Action<Exception, string> _registerExceptionAction;

		public CommandExceptionRegistrator(Action<Exception, string> registerExceptionAction)
		{
			if (registerExceptionAction == null) throw new ArgumentNullException("registerExceptionAction");
			_registerExceptionAction = registerExceptionAction;
		}

		public virtual void RegisterCommandException(Exception exception, ICommand command)
		{
			_registerExceptionAction(exception, ExceptionHandlerCommand.FormatCommandToErrorMessage(command));
		}
	}
}