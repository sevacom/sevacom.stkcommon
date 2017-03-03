using System;
using System.Windows;
using Microsoft.Win32;
using StkCommon.UI.Wpf.ViewModels;
using StkCommon.UI.Wpf.Views.Windows;

namespace StkCommon.UI.Wpf.Views
{
	/// <summary>
	/// Интерфейс агента, поднимающего диалоги.
	/// </summary>
	public interface IShowDialogAgent
	{
		/// <summary>
		/// Заголовок окна по умолчанию для отображения в ShowMessageDialog, если не удалось получить активное окно
		/// </summary>
		string DefaultWindowTitle { get; set; }

		/// <summary>
		/// Показать диалог.
		/// </summary>
		/// <param name="dialogViewModel">ViewModel диалога.</param>		
		/// <typeparam name="T">Тип View диалога.</typeparam>
		/// <returns>
		/// Возвращаемое значение такое же как у System.Windows.Window.ShowDialog().
		/// </returns>
		bool? ShowDialog<T>(object dialogViewModel) where T : Window, new();

		/// <summary>
		/// Показать диалог.
		/// </summary>
		/// <param name="dialogViewModel">ViewModel диалога.</param>
		/// <param name="owner">Владелец создаваемого окна, возможно null</param>
		/// <typeparam name="T">Тип View диалога.</typeparam>
		/// <returns>
		/// Возвращаемое значение такое же как у System.Windows.Window.ShowDialog().
		/// </returns>
		bool? ShowDialog<T>(object dialogViewModel, IWindow owner) where T : Window, new();

		/// <summary>
		/// Показать окно в немодальном режиме, owner у окна указывается текущее активное окно
		/// </summary>
		/// <param name="viewModel">ViewModel окна.</param>
		/// <returns>Интерфейс окна для управления закрытием</returns>
		IWindow Show<T>(object viewModel)
			where T : Window, IWindow, new();

		/// <summary>
		/// Показать окно в немодальном режиме
		/// </summary>
		/// <typeparam name="T">Тип View для окна.</typeparam>
		/// <param name="viewModel">ViewModel окна.</param>
		/// <param name="owner">Владелец создаваемого окна</param>
		/// <returns>Интерфейс окна для управления закрытием</returns>
		IWindow Show<T>(object viewModel, IWindow owner)
			where T : Window, IWindow, new();

        /// <summary>
        /// Показать MessageBox.
        /// </summary>
        /// <param name="message">Текст для отображения.</param>
        /// <param name="caption">Заголовок. если пустой то берётся заголовок активного окна</param>
        /// <param name="button">
        /// Параметр, определяющий какие кнопки должен содержать MessageBox.
        /// </param>
        /// <param name="icon">Иконка для отображения.</param>
        /// <returns>
        /// MessageBoxResult определяет какую кнопку нажал пользователь.
        /// </returns>
        MessageBoxResult ShowMessageDialog(string message, string caption,
            MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Information);

		/// <summary>
		/// Показать MessageBox. с установкой Caption из текущего активного окна
		/// </summary>
		/// <param name="message">Текст для отображения.</param>
		/// <param name="button">
		/// Параметр, определяющий какие кнопки должен содержать MessageBox.
		/// </param>
		/// <param name="icon">Иконка для отображения.</param>
		/// <returns>
		/// MessageBoxResult определяет какую кнопку нажал пользователь.
		/// </returns>
		MessageBoxResult ShowMessageDialog(string message, MessageBoxButton button = MessageBoxButton.OK,
			MessageBoxImage icon = MessageBoxImage.Information);

        /// <summary>
        /// Показать MessageBox.
        /// </summary>
        /// <param name="message">Текст для отображения.</param>
        /// <param name="caption">Заголовок. если пустой то берётся заголовок активного окна</param>
        /// <param name="button">
        /// Параметр, определяющий какие кнопки должен содержать MessageBox.
        /// </param>
        /// <param name="icon">Иконка для отображения.</param>
        /// <param name="defaultButton">Параметр определяющий какая кнопка будет выбрана по умолчанию</param>
        /// <param name="options">Specifies special display options for a message box.</param>
        /// <returns>
        /// MessageBoxResult определяет какую кнопку нажал пользователь.
        /// </returns>
        MessageBoxResult ShowMessageDialog(string message, string caption,
            MessageBoxButton button, MessageBoxImage icon,
            MessageBoxResult defaultButton, MessageBoxOptions options);

		/// <summary>
		/// Показать MessageBox.
		/// </summary>
		/// <param name="owner">Владелец окна, возможно null</param>
		/// <param name="message">Текст для отображения.</param>
		/// <param name="caption">Заголовок. если пустой то берётся заголовок активного окна</param>
		/// <param name="button">
		/// Параметр, определяющий какие кнопки должен содержать MessageBox.
		/// </param>
		/// <param name="icon">Иконка для отображения.</param>
		/// <param name="defaultButton">Параметр определяющий какая кнопка будет выбрана по умолчанию</param>
		/// <param name="options">Specifies special display options for a message box.</param>
		/// <returns>
		/// MessageBoxResult определяет какую кнопку нажал пользователь.
		/// </returns>
		MessageBoxResult ShowMessageDialog(
			IWindow owner, 
			string message, 
			string caption = null,
			MessageBoxButton button = MessageBoxButton.OK, 
			MessageBoxImage icon = MessageBoxImage.Information, 
			MessageBoxResult defaultButton = MessageBoxResult.OK,
			MessageBoxOptions options = MessageBoxOptions.None);

		/// <summary>
		/// Показать диалог с сообщением об ошибке для активного окна.
		/// </summary>
		void ShowErrorMessageDialog(string message, string details, string caption = null);

		/// <summary>
		/// Показать диалог с сообщением об ошибке с указанием owner.
		/// </summary>
		void ShowErrorMessageDialog(IWindow owner, string message, string details, string caption = null);

		/// <summary>
		/// Показать диалог открытия файла
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="fileName"></param>
		/// <param name="owner">Владелец создаваемого окна</param>
		/// <returns></returns>
		bool? ShowOpenFileDialog(string filter, out string fileName, IWindow owner = null);

		/// <summary>
		/// Показать диалог сохранения файла
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="defFileName"></param>
		/// <param name="fileName"></param>
		/// <param name="owner">Владелец создаваемого окна</param>
		/// <returns></returns>
		bool? ShowSaveFileDialog(string filter, string defFileName, out string fileName, IWindow owner = null);
	}

	/// <summary>
	/// Реализация поднятия диалогов
	/// </summary>
	public class ShowDialogAgent : IShowDialogAgent
	{
		private readonly IDispatcher _dispatcher;
		private Window _theActiveWindow;

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="dispatcher"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public ShowDialogAgent(IDispatcher dispatcher)
		{
			if (dispatcher == null) throw new ArgumentNullException("dispatcher");
			_dispatcher = dispatcher;
			DefaultWindowTitle = string.Empty;
		}

		#region Properties

		/// <summary>
		/// Заголовок окна по умолчанию для отображения в ShowMessageDialog если не удалось получить активное окно
		/// </summary>
		public string DefaultWindowTitle { get; set; }

		/// <summary>
		/// Главное окно приложения
		/// </summary>
		private Window MainWindow
		{
			get
			{
				if (!_dispatcher.CheckAccess())
				{
					return _dispatcher.Invoke(new Func<Window>(() => MainWindow)) as Window;
				}
				return Application.Current == null ? null : Application.Current.MainWindow;
			}
		}

		/// <summary>
		/// Текущее активное окно исходя из открытых через этот интерфейс. 
		/// Если нет открытых то MainWindow
		/// </summary>
		protected virtual Window ActiveWindow
		{
			get { return _theActiveWindow ?? (_theActiveWindow = MainWindow); }
			set { _theActiveWindow = value; }
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Показать диалог.
		/// </summary>
		/// <param name="dialogViewModel">ViewModel диалога.</param>		
		/// <typeparam name="T">Тип View диалога.</typeparam>
		/// <returns>
		/// Возвращаемое значение такое же как у System.Windows.Window.ShowDialog().
		/// </returns>
		public bool? ShowDialog<T>(object dialogViewModel) where T : Window, new()
		{
			return ShowDialogInternal<T>(dialogViewModel, ActiveWindow);
		}

		/// <summary>
		/// Показать диалог.
		/// </summary>
		/// <param name="dialogViewModel">ViewModel диалога.</param>
		/// <param name="owner">Владелец создаваемого окна</param>
		/// <typeparam name="T">Тип View диалога.</typeparam>
		/// <returns>
		/// Возвращаемое значение такое же как у System.Windows.Window.ShowDialog().
		/// </returns>
		public bool? ShowDialog<T>(object dialogViewModel, IWindow owner) where T : Window, new()
		{
			ThrowArgumentExceptionInNotWindow(owner);

			return ShowDialogInternal<T>(dialogViewModel, (Window) owner);
		}

		/// <summary>
		/// Показать окно в немодальном режиме, owner у окна указывается текущее активное окно
		/// </summary>
		/// <param name="viewModel">ViewModel окна.</param>
		/// <returns>Интерфейс окна для управления закрытием</returns>
		public IWindow Show<T>(object viewModel)
			where T : Window, IWindow, new()
		{
			return ShowInternal<T>(viewModel, ActiveWindow);
		}
		
		/// <summary>
		/// Показать окно в немодальном режиме
		/// </summary>
		/// <param name="viewModel">ViewModel окна.</param>
		/// <param name="owner">Владелец создаваемого окна</param>
		/// <returns>Интерфейс окна для управления закрытием</returns>
		public IWindow Show<T>(object viewModel, IWindow owner)
			where T : Window, IWindow, new()
		{
			ThrowArgumentExceptionInNotWindow(owner);

			return ShowInternal<T>(viewModel, (Window) owner);
		}

	    /// <summary>
	    /// Показать MessageBox. с установкой Caption из текущего активного окна
	    /// </summary>
	    /// <param name="message">Текст для отображения.</param>
	    /// <param name="button">
	    /// Параметр, определяющий какие кнопки должен содержать MessageBox.
	    /// </param>
	    /// <param name="icon">Иконка для отображения.</param>
	    /// <returns>
	    /// MessageBoxResult определяет какую кнопку нажал пользователь.
	    /// </returns>
	    public MessageBoxResult ShowMessageDialog(
			string message, 
			MessageBoxButton button = MessageBoxButton.OK,
			MessageBoxImage icon = MessageBoxImage.Information)
		{
			return ShowMessageDialogInternal(ActiveWindow, message, null, button, icon,
				MessageBoxResult.OK, MessageBoxOptions.None);
		}

	    /// <summary>
	    /// Показать MessageBox.
	    /// </summary>
	    /// <param name="message">Текст для отображения.</param>
	    /// <param name="caption">Заголовок. если пустой то берётся заголовок активного окна</param>
	    /// <param name="button">
	    /// Параметр, определяющий какие кнопки должен содержать MessageBox.
	    /// </param>
	    /// <param name="icon">Иконка для отображения.</param>
	    /// <returns>
	    /// MessageBoxResult определяет какую кнопку нажал пользователь.
	    /// </returns>
	    public MessageBoxResult ShowMessageDialog(
			string message, 
			string caption,
			MessageBoxButton button = MessageBoxButton.OK, 
			MessageBoxImage icon = MessageBoxImage.Information)
		{
			return ShowMessageDialogInternal(ActiveWindow, message, caption, button, icon, 
				MessageBoxResult.OK, MessageBoxOptions.None);
		}

		/// <summary>
	    /// Показать MessageBox.
	    /// </summary>
	    /// <param name="message">Текст для отображения.</param>
	    /// <param name="caption">Заголовок. если пустой то берётся заголовок активного окна</param>
	    /// <param name="button">
	    /// Параметр, определяющий какие кнопки должен содержать MessageBox.
	    /// </param>
	    /// <param name="icon">Иконка для отображения.</param>
	    /// <param name="defaultButton">Параметр определяющий какая кнопка будет выбрана по умолчанию</param>
	    /// <param name="options">Specifies special display options for a message box.</param>
	    /// <returns>
	    /// MessageBoxResult определяет какую кнопку нажал пользователь.
	    /// </returns>
	    public MessageBoxResult ShowMessageDialog(
	        string message,
	        string caption,
	        MessageBoxButton button,
	        MessageBoxImage icon,
	        MessageBoxResult defaultButton,
	        MessageBoxOptions options)
	    {
			return ShowMessageDialogInternal(ActiveWindow, message, caption, button, icon, 
				defaultButton, options);
	    }

		/// <summary>
		/// Показать MessageBox.
		/// </summary>
		/// <param name="owner">Владелец окна, допускает передачу null</param>
		/// <param name="message">Текст для отображения.</param>
		/// <param name="caption">Заголовок. если пустой то берётся заголовок активного окна</param>
		/// <param name="button">
		/// Параметр, определяющий какие кнопки должен содержать MessageBox.
		/// </param>
		/// <param name="icon">Иконка для отображения.</param>
		/// <param name="defaultButton">Параметр определяющий какая кнопка будет выбрана по умолчанию</param>
		/// <param name="options">Specifies special display options for a message box.</param>
		/// <returns>
		/// MessageBoxResult определяет какую кнопку нажал пользователь.
		/// </returns>
		public MessageBoxResult ShowMessageDialog(
			IWindow owner, 
			string message, 
			string caption = null,
			MessageBoxButton button = MessageBoxButton.OK, 
			MessageBoxImage icon = MessageBoxImage.Information, 
            MessageBoxResult defaultButton = MessageBoxResult.OK,
			MessageBoxOptions options = MessageBoxOptions.None)
		{
			ThrowArgumentExceptionInNotWindow(owner);
            return ShowMessageDialogInternal((Window)owner, message, caption, button, icon, 
				defaultButton, options);
		}

		/// <summary>
		/// Показать диалог с сообщением об ошибке.
		/// </summary>
		public void ShowErrorMessageDialog(string message, string details, string caption = null)
		{
			ShowErrorMessageDialogInternal(ActiveWindow, message, details, caption);
		}

		public void ShowErrorMessageDialog(IWindow owner, string message, string details, string caption = null)
		{
			ThrowArgumentExceptionInNotWindow(owner);
			ShowErrorMessageDialogInternal((Window)owner, message, details, caption);
		}

		/// <summary>
		/// Показать диалог открытия файла
		/// </summary>
		/// <param name="filter">Фильтр для фильтрации файлов по типам</param>
		/// <param name="fileName">Имя файла</param>
		/// <param name="owner">Владелец создаваемого окна</param>
		/// <returns></returns>
		public virtual bool? ShowOpenFileDialog(string filter, out string fileName, IWindow owner = null)
		{
			ThrowArgumentExceptionInNotWindow(owner);
			
			fileName = null;
			var ownerWindow = (Window)owner ?? ActiveWindow;

			var dlg = new OpenFileDialog
			{
				Filter = filter
			};
			var res = dlg.ShowDialog(ownerWindow);
			if (true == res)
			{
				fileName = dlg.FileName;
			}
			return res;
		}

		/// <summary>
		/// Показать диалог сохранения файла
		/// </summary>
		/// <param name="filter">Фильтр для фильтрации файлов по типам</param>
		/// <param name="defFileName">Первоначальное имя файла</param>
		/// <param name="fileName">Имя файла после закрытия диалога</param>
		/// <param name="owner">Владелец создаваемого окна</param>
		/// <returns></returns>
		public virtual bool? ShowSaveFileDialog(string filter, string defFileName, out string fileName, IWindow owner = null) 
		{
			ThrowArgumentExceptionInNotWindow(owner);

			fileName = null;
			var ownerWindow = (Window)owner ?? ActiveWindow;

			var dlg = new SaveFileDialog
			{
				Filter = filter, 
				FileName = defFileName
			};

			var res = dlg.ShowDialog(ownerWindow);
			if (true == res)
			{
				fileName = dlg.FileName;
			}
			return res;
		}

		#endregion

		#region Protected methods

		protected virtual bool? ShowDialogInternal<T>(object dialogViewModel, Window owner) where T : Window, new()
		{
			if (!_dispatcher.CheckAccess())
			{
				return (bool?)_dispatcher.Invoke(new Func<bool?>(() => ShowDialogInternal<T>(dialogViewModel, owner)));
			}

			bool? res;

			var prevActiveWindow = ActiveWindow;

			try
			{
				var dialog = new T
				{
					Owner = owner != null && owner.IsLoaded ? owner : null,
					DataContext = dialogViewModel
				};

				ActiveWindow = dialog;
				res = dialog.ShowDialog();
			}
			finally
			{
				ActiveWindow = prevActiveWindow;
			}
			return res;
		}

		protected virtual IWindow ShowInternal<T>(object viewModel, Window owner)
			where T : Window, IWindow, new()
		{
			if (!_dispatcher.CheckAccess())
			{
				return (IWindow)_dispatcher.Invoke(new Func<IWindow>(() => ShowInternal<T>(viewModel, owner)));
			}

			var window = new T
			{
				Owner = owner != null && owner.IsLoaded ? owner : null,
				DataContext = viewModel
			};
			window.Show();

			return window;
		}

		protected virtual void ShowErrorMessageDialogInternal(Window owner, string message, string details, string caption)
		{
			if (!_dispatcher.CheckAccess())
			{
				_dispatcher.Invoke(() => ShowErrorMessageDialogInternal(owner, message, details, caption));
				return;
			}

			var messageCaption = caption;
			try
			{
				messageCaption = GetMessageDialogCaption(owner, caption);
				ShowDialogInternal<ErrorMessageWindow>(new ErrorMessageViewModel(messageCaption, message, details), owner);
			}
			catch (Exception ex)
			{
				Console.WriteLine(@"Ошибка отображения окна об ошибке: " + ex);
				MessageBox.Show(message + Environment.NewLine + Environment.NewLine + details, messageCaption, 
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Реализация отображения Message диалога
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="messageBoxText"></param>
		/// <param name="caption">может быть пустым</param>
		/// <param name="button"></param>
		/// <param name="icon"></param>
        /// <param name="options"></param>
        /// <param name="defaultButton"></param>
		/// <returns>MessageBoxResult</returns>
		protected virtual MessageBoxResult ShowMessageDialogInternal(
			Window owner, 
			string messageBoxText, 
			string caption,
			MessageBoxButton button, 
			MessageBoxImage icon, 
			MessageBoxResult defaultButton,
			MessageBoxOptions options)
		{		
			if (icon == MessageBoxImage.Error && button == MessageBoxButton.OK)
			{
				ShowErrorMessageDialogInternal(owner, messageBoxText, null, caption);
				return MessageBoxResult.OK;
			}

			if (!_dispatcher.CheckAccess())
			{
				return (MessageBoxResult)_dispatcher.Invoke(
					new Func<MessageBoxResult>(() => ShowMessageDialogInternal(owner, messageBoxText, caption, button, icon, defaultButton, options)));
			}

			var messageBoxCaption = GetMessageDialogCaption(owner, caption);

			return owner != null
				? MessageBox.Show(owner, messageBoxText, messageBoxCaption, button, icon, defaultButton, options)
				: MessageBox.Show(messageBoxText, messageBoxCaption, button, icon, defaultButton);
		}

		/// <summary>
		/// Реализация получения Caption для Message диалога.
		/// Если передали пустой, то возвращает Title текущего активного окна
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="caption"></param>
		/// <returns></returns>
		protected virtual string GetMessageDialogCaption(Window owner, string caption)
		{
			if (!string.IsNullOrWhiteSpace(caption))
				return caption;

			if (owner != null)
				return owner.Title;

			try
			{
				return ActiveWindow == null || string.IsNullOrWhiteSpace(ActiveWindow.Title)
							? DefaultWindowTitle
							: ActiveWindow.Title;
			}
			catch
			{
				return DefaultWindowTitle;
			}
		}

		#endregion

		private static void ThrowArgumentExceptionInNotWindow(IWindow owner)
		{
			if (owner != null && !(owner is Window))
				throw new ArgumentException("owner должен быть наследником класса Window", "owner");
		}
	}
}
