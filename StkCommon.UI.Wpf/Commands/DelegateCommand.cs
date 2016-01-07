using System;
using System.Collections.Generic;
using System.Windows.Input;
using StkCommon.UI.Wpf.Events;
using StkCommon.UI.Wpf.Model;
using StkCommon.UI.Wpf.ViewModels;

namespace StkCommon.UI.Wpf.Commands
{
	/// <summary>
	///     This class allows delegating the commanding logic to methods passed as parameters,
	///     and enables a View to bind commands to objects that are not part of the element tree.
	/// </summary>
	public class SimpleDelegateCommand : DelegateCommand<object>
	{
		#region Constructors

		/// <summary>
		///     Constructor
		/// </summary>
		public SimpleDelegateCommand(Action executeMethod, Func<bool> canExecuteMethod = null, 
			string commandName = null, bool isAutomaticRequeryDisabled = false)
			: base(ConvertExecuteAction(executeMethod), ConvertCanExecuteFunc(canExecuteMethod), commandName, isAutomaticRequeryDisabled)
		{
			if (executeMethod == null)
			{
				throw new ArgumentNullException("executeMethod");
			}
		}

		#endregion

		#region Public Methods

		public virtual bool CanExecute()
		{
			return base.CanExecute(null);
		}

		/// <summary>
		/// Execution of the command
		/// </summary>
		public virtual void Execute()
		{
			base.Execute(null);
		}

		#endregion

		#region Private Methods

		private static Func<object, bool> ConvertCanExecuteFunc(Func<bool> canExecuteMethod)
		{
			if (canExecuteMethod == null)
				return null;
			return p => canExecuteMethod();
		}

		private static Action<object> ConvertExecuteAction(Action executeAction)
		{
			return p => executeAction();
		}

		#endregion
	}

	/// <summary>
	///     This class allows delegating the commanding logic to methods passed as parameters,
	///     and enables a View to bind commands to objects that are not part of the element tree.
	/// </summary>
	public class DelegateCommand: DelegateCommand<object>
	{
		public DelegateCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod = null, string commandName = null) :
			base(executeMethod, canExecuteMethod, commandName, false)
		{
		}
	}

	/// <summary>
	///     This class allows delegating the commanding logic to methods passed as parameters,
	///     and enables a View to bind commands to objects that are not part of the element tree.
	/// </summary>
	/// <typeparam name="T">Type of the parameter passed to the delegates</typeparam>
	public class DelegateCommand<T> : ViewModelBase, IDelegateCommand
	{
		#region Private fields

		private readonly Action<T> _executeMethod;
		private readonly Func<T, bool> _canExecuteMethod;
		private ShortCut _hotKey;
		private bool _isAutomaticRequeryDisabled;
		private List<WeakReference> _canExecuteChangedHandlers;

		#endregion

		#region Constructors

		/// <summary>
		///     Constructor
		/// </summary>
		public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod = null, 
			string commandName = null, bool isAutomaticRequeryDisabled = false)
		{
			if (executeMethod == null)
			{
				throw new ArgumentNullException("executeMethod");
			}

			CommandName = commandName;
			_isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
			_hotKey = ShortCut.None;
			_executeMethod = executeMethod;
			_canExecuteMethod = canExecuteMethod;
		}

		#endregion

		#region IDelegateCommand Members

		/// <summary>
		///     Property to enable or disable CommandManager's automatic requery on this command
		/// </summary>
		public bool IsAutomaticRequeryDisabled
		{
			get
			{
				return _isAutomaticRequeryDisabled;
			}
			set
			{
				if (_isAutomaticRequeryDisabled != value)
				{
					if (value)
					{
						WeakReferenceEventManager.RemoveHandlersFromRequerySuggested(_canExecuteChangedHandlers);
					}
					else
					{
						WeakReferenceEventManager.AddHandlersToRequerySuggested(_canExecuteChangedHandlers);
					}
					_isAutomaticRequeryDisabled = value;
				}
			}
		}

		/// <summary>
		/// Название команды
		/// </summary>
		public string CommandName { get; private set; }

		/// <summary>
		/// Горячяя клавиша
		/// </summary>
		public ShortCut HotKey
		{
			get
			{
				return _hotKey;
			}
			set
			{
				if (_hotKey.Equals(value))
					return;
				_hotKey = value;

				OnPropertyChanged(() => HotKey);
			}
		}

		/// <summary>
		///     ICommand.CanExecuteChanged implementation
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add
			{
				if (!IsAutomaticRequeryDisabled)
				{
					CommandManager.RequerySuggested += value;
				}
				WeakReferenceEventManager.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value, 2);
			}
			remove
			{
				if (!IsAutomaticRequeryDisabled)
				{
					CommandManager.RequerySuggested -= value;
				}
				WeakReferenceEventManager.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value);
			}
		}

		bool ICommand.CanExecute(object parameter)
		{
			return parameter == null ?
				CanExecute(default(T)) :
				CanExecute((T)parameter);
		}

		void ICommand.Execute(object parameter)
		{
			Execute((T)parameter);
		}

		#endregion

		#region Public Methods

		/// <summary>
		///     Method to determine if the command can be executed
		/// </summary>
		public virtual bool CanExecute(T parameter)
		{
			if (_canExecuteMethod != null)
			{
				return _canExecuteMethod(parameter);
			}
			return true;
		}

		/// <summary>
		///     Execution of the command
		/// </summary>
		public virtual void Execute(T parameter)
		{
			_executeMethod(parameter);
		}

		/// <summary>
		///     Raises the CanExecuteChaged event
		/// </summary>
		public void RaiseCanExecuteChanged()
		{
			OnCanExecuteChanged();
		}

		/// <summary>
		///     Protected virtual method to raise CanExecuteChanged event
		/// </summary>
		protected virtual void OnCanExecuteChanged()
		{
			WeakReferenceEventManager.CallWeakReferenceHandlers(_canExecuteChangedHandlers);
		}

		#endregion
	}
}
