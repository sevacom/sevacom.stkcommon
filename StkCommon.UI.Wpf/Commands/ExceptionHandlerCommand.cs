using System;
using System.ComponentModel;
using System.Windows.Input;
using StkCommon.Data.Text;
using StkCommon.UI.Wpf.Model;
using StkCommon.UI.Wpf.Properties;
using StkCommon.UI.Wpf.ViewModels;

namespace StkCommon.UI.Wpf.Commands
{
	/// <summary>
	/// Команда обертка для <see cref="ICommand"/> следящая за ошибками.
	/// </summary>
	public class ExceptionHandlerCommand: ViewModelBase, IDelegateCommand
	{
		private readonly Action<Exception, string> _registerException;
		private readonly Action<Exception, ICommand> _registerCommandException;
		private readonly ICommand _command;
		private readonly IDelegateCommand _delegateCommand;

		private ExceptionHandlerCommand(ICommand command)
		{
			if (command == null) 
				throw new ArgumentNullException("command");
			_command = command;
			_delegateCommand = command as IDelegateCommand;
		}

		/// <summary>
		/// Регистрирует все ошибки при выполнении команды, используя регистратор ошибок от команд
		/// </summary>
		/// <param name="command">Оборачиваемая команда</param>
		/// <param name="commandExceptionRegistrator">Регистратор ошибок команд</param>
		[Obsolete("ICommandExceptionRegistrator скоро будет удалён, необходимо использовать конструктор с Action<Exception, string> registerException")]
		public ExceptionHandlerCommand(ICommand command, ICommandExceptionRegistrator commandExceptionRegistrator)
			: this(command, commandExceptionRegistrator.RegisterCommandException)
		{	
		}

		/// <summary>
		/// Регистрирует все ошибки при выполнении команды, используя передачу команды по которой произошла ошибка
		/// </summary>
		/// <param name="command">Оборачиваемая команда</param>
		/// <param name="registerCommandException">Регистрация ошибки с указанием какая команда</param>
		public ExceptionHandlerCommand(ICommand command, Action<Exception, ICommand> registerCommandException)
			: this(command)
		{
			if (registerCommandException == null) throw new ArgumentNullException("registerCommandException");
			_registerCommandException = registerCommandException;
		}

		/// <summary>
		/// Регистрирует все ошибки при выполнении команды, используя стандартное сообщение для команд
		/// </summary>
		/// <param name="command">Оборачиваемая команда</param>
		/// <param name="registerException">Регистрация ошибки со стандартным сообщением</param>
		public ExceptionHandlerCommand(ICommand command, Action<Exception, string> registerException)
			: this(command)
		{
			if (registerException == null) throw new ArgumentNullException("registerException");
			_registerException = registerException;
		}

		public event EventHandler CanExecuteChanged
		{
			add
			{
				_command.CanExecuteChanged += value;
			}
			remove
			{
				_command.CanExecuteChanged -= value;
			}
		}

		public override event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				if (_delegateCommand != null)
					_delegateCommand.PropertyChanged += value;
			}
			remove
			{
				if (_delegateCommand != null)
					_delegateCommand.PropertyChanged -= value;
			}
		}

		public virtual void Execute(object parameter)
		{
			try
			{
				_command.Execute(parameter);
			}
			catch (Exception exception)
			{
				OnCommandExceptionOccured(exception);
			}
		}

		public virtual bool CanExecute(object parameter)
		{
			try
			{
				return _command.CanExecute(parameter);
			}
			catch (Exception exception)
			{
				OnCommandExceptionOccured(exception);
				return false;
			}
		}

		/// <summary>
		/// Обработка ошибки произошедшей при выполнении команды, может быть переопределена
		/// </summary>
		/// <param name="exception"></param>
		protected virtual void OnCommandExceptionOccured(Exception exception)
		{
			if (_registerException != null)
			{
				_registerException(exception, FormatCommandToErrorMessage(_command));
			}
			else if (_registerCommandException != null)
			{
				_registerCommandException(exception, _command);
			}
		}

		public string CommandName
		{
			get
			{
				return _delegateCommand == null ? string.Empty : _delegateCommand.CommandName;
			}
		}

		public ShortCut HotKey
		{
			get
			{
				return _delegateCommand == null ? ShortCut.None : _delegateCommand.HotKey;
			}
			set
			{
				if (_delegateCommand == null)
					return;

				_delegateCommand.HotKey = value;
			}
		}

		public void RaiseCanExecuteChanged()
		{
			if(_delegateCommand != null)
				_delegateCommand.RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Выдаёт стандартное сообщение об ошибке при выполнении команды
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public static string FormatCommandToErrorMessage(ICommand command)
		{
			var delegateCommand = command as IDelegateCommand;
			string commandName = null;
			if (delegateCommand != null)
				commandName = delegateCommand.CommandName.TrimIfNotNull();

			commandName = !string.IsNullOrWhiteSpace(commandName) ? string.Format(" '{0}' ", commandName) : " ";
			return string.Format(Resources.ExceptionHandlerCommand_ErrorMessage, commandName);
		}
	}
}
