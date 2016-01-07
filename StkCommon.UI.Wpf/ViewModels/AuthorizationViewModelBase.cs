using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using StkCommon.UI.Wpf.Commands;
using StkCommon.UI.Wpf.Exceptions;
using StkCommon.UI.Wpf.Model;
using StkCommon.UI.Wpf.Properties;
using StkCommon.UI.Wpf.Views;

namespace StkCommon.UI.Wpf.ViewModels
{
	/// <summary>
	/// Базовый класс вьюмодели окна авторизации пользователя
	/// </summary>
	public abstract class AuthorizationViewModelBase : ViewModelBase, IAuthorizationViewModel
	{
		#region Fields

		private string _userName;
		private string _password;
		private IEnumerable<object> _servers;
		private IEnumerable<object> _databases; 
		private string _server;
		private string _database;
		private string _inputLanguage;
		private InputLanguageManager _languageManager;
		private bool? _dialogResult;
		private readonly IShowDialogAgent _agent;
		private bool _isServersDropDownOpen;
		private bool _isDataBaseDropDownOpen;

		#endregion

		#region Constructors

		public AuthorizationViewModelBase(AuthorizationMode mode, IShowDialogAgent agent)
		{
			_agent = agent;
			Mode = mode;
			UiScale = 1.0;

			InitializeLanguage();

			OkCommand = new DelegateCommand(OkCommandHandler, CanOkCommandHandler);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Логин пользователя
		/// </summary>
		public string UserName
		{
			get { return _userName; }
			set
			{
				if (_userName == value)
					return;

				_userName = value;
				OnPropertyChanged(() => UserName);
			}
		}

		/// <summary>
		/// Пароль пользователя
		/// </summary>
		public string Password
		{
			get { return _password; }
			set
			{
				if (_password == value)
					return;

				_password = value;
				OnPropertyChanged(() => Password);
			}
		}

		/// <summary>
		/// Список серверов
		/// </summary>
		public IEnumerable<object> Servers
		{
			get
			{
				return _servers;
			}
			set
			{
				if (_servers == value)
					return;
				_servers = value;
				OnPropertyChanged(() => Servers);
			}
		}

		/// <summary>
		/// Список баз данных
		/// </summary>
		public IEnumerable<object> Databases
		{
			get
			{
				return _databases;
			}
			set
			{
				if (_databases == value)
					return;
				_databases = value;
				OnPropertyChanged(() => Databases);
			}
		}

		/// <summary>
		/// Выбранный или введенный сервер
		/// </summary>
		public string Server
		{
			get
			{
				return _server;
			} 
			set
			{
				if (_server == value)
					return;
				_server = value;
				OnPropertyChanged(() => Server);
			}
		}

		/// <summary>
		/// Выбранная или введенная база данных
		/// </summary>
		public string Database
		{
			get
			{
				return _database;
			}
			set
			{
				if (_database == value)
					return;
				_database = value;
				OnPropertyChanged(() => Database);
			}
		}

		/// <summary>
		/// Режим работы окна авторизации
		/// </summary>
		public AuthorizationMode Mode { get; protected set; }

		/// <summary>
		/// Язык ввода
		/// </summary>
		public string InputLanguage
		{
			get { return _inputLanguage; }
			set
			{
				if (_inputLanguage == value) return;
				_inputLanguage = value;
				OnPropertyChanged(() => InputLanguage);    
			}
		}

		/// <summary>
		/// Иконка приложения
		/// </summary>
		public ImageSource ApplicationIcon { get; set; }

		/// <summary>
		/// Название приложения
		/// </summary>
		public string ApplicationName { get; set; }

		/// <summary>
		/// Признак управления закрытием окна в случае успешной авторизации 
		/// </summary>
		public bool? DialogResult
		{
			get
			{
				return _dialogResult;
			}
			set
			{
				if (_dialogResult == value)
					return;
				_dialogResult = value;
				Close();
				OnPropertyChanged(() => DialogResult);
			}
		}

		/// <summary>
		/// Масштаб окна, по умолчанию 1.0
		/// </summary>
		public double UiScale { get; set; }

		/// <summary>
		/// Признак отображения поля для ввода (выбора) сервера 
		/// </summary>
		public bool IsShowServer
		{
			get
			{
				return Mode == AuthorizationMode.Server || Mode == AuthorizationMode.ServerDatabase;
			}
		}

		/// <summary>
		/// Признак отображения поля для ввода (выбора) базы данных 
		/// </summary>
		public bool IsShowDatabase
		{
			get
			{
				return Mode == AuthorizationMode.Database || Mode == AuthorizationMode.ServerDatabase;
			}
		}

		/// <summary>
		/// Признак выпадающего списка серверов
		/// </summary>
		public bool IsServersDropDownOpen
		{
			get { return _isServersDropDownOpen; }
			set
			{
				if (_isServersDropDownOpen == value)
					return;
				_isServersDropDownOpen = value;

				OnPropertyChanged(() => IsServersDropDownOpen);
				OnServersDropDownChanged(_isServersDropDownOpen);
			}
		}

		/// <summary>
		/// Признак выпадающего списка БД
		/// </summary>
		public bool IsDataBaseDropDownOpen
		{
			get { return _isDataBaseDropDownOpen; }
			set
			{
				if (_isDataBaseDropDownOpen == value)
					return;
				_isDataBaseDropDownOpen = value;

				OnPropertyChanged(() => IsDataBaseDropDownOpen);

				OnDataBasesDropDownChanged(_isDataBaseDropDownOpen);
			}
		}

		#endregion

		#region Commands

		public ICommand OkCommand { get; private set; }

		#endregion

		#region Сommand Handlers

		private bool CanOkCommandHandler(object obj)
		{
			return !string.IsNullOrWhiteSpace(_userName) && 
				((Mode == AuthorizationMode.LoginPassword )
				   || (Mode == AuthorizationMode.Server && !string.IsNullOrWhiteSpace(_server))
				   || (Mode == AuthorizationMode.Database && !string.IsNullOrWhiteSpace(_database))
				   || (Mode == AuthorizationMode.ServerDatabase && !string.IsNullOrWhiteSpace(_server) && !string.IsNullOrWhiteSpace(_database)));
		}

		private void OkCommandHandler(object obj)
		{
			if (!CanOkCommandHandler(obj)) 
				return;

			try
			{
				var result = Authorize();
				DialogResult = result;
			}
			catch (UserMessageException uex)
			{
				_agent.ShowMessageDialog(uex.Message, Resources.AuthorizationWindow_Title, MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch (Exception ex)
			{
				_agent.ShowMessageDialog(Resources.AuthorizationWindow_FaultExceptionMessage + "\r\n" + ex.Message, 
					Resources.AuthorizationWindow_Title, MessageBoxButton.OK, MessageBoxImage.Error);
				DialogResult = false;
			}

		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Выполнить авторизацию (аутентификацию)
		/// Всё что завёрнуто в UserMessageException обрабатывается, оторажается сообщение с текстом Message, закрытие окна авторизации отменяется 
		/// Остальные Exception тоже обрабатываются, окно авторизации закрывается
		/// </summary>
		/// <returns>true - авторизация прошла успешно, false - закрыть оконо авторизации с DialogResult = false</returns>
		protected abstract bool Authorize();

		/// <summary>
		/// Инициализация языка
		/// </summary>
		protected virtual void InitializeLanguage()
		{
			_languageManager = InputLanguageManager.Current;
			_languageManager.InputLanguageChanged += InputLanguageChanged;

			InputLanguageChanged(this, null);
		}

		/// <summary>
		/// Открытие/закрытие выпадающего списока серверов
		/// </summary>
		/// <param name="isDropDown">true - открыт, false - закрыт</param>
		protected virtual void OnServersDropDownChanged(bool isDropDown)
		{
			
		}

		/// <summary>
		/// Открытие/закрытие выпадающего списока БД
		/// </summary>
		/// <param name="isDropDown">true - открыт, false - закрыт</param>
		protected virtual void OnDataBasesDropDownChanged(bool isDropDown)
		{

		}

		#endregion

		#region Private Methods

		private void InputLanguageChanged(object sender, InputLanguageEventArgs e)
		{
			InputLanguage = _languageManager.CurrentInputLanguage.TwoLetterISOLanguageName.ToUpper();
		}

		private void Close()
		{
			if (_languageManager != null)
				_languageManager.InputLanguageChanged -= InputLanguageChanged;
		}

		#endregion

	}
}
