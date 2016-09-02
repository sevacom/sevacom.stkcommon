using System.Windows;
using StkCommon.UI.Wpf.Views;

namespace StkCommon.UI.Wpf.ViewModels.DesignTime
{
	internal class DesignMockShowDialogAgent : IShowDialogAgent
	{
		/// <summary>
		/// Заголовок окна по умолчанию для отображения в ShowMessageDialog, если не удалось получить активное окно
		/// </summary>
		public string DefaultWindowTitle { get; set; }

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
			return null;
		}

		/// <summary>
		/// Показать диалог.
		/// </summary>
		/// <param name="dialogViewModel">ViewModel диалога.</param>
		/// <param name="owner">Владелец создаваемого окна, возможно null</param>
		/// <typeparam name="T">Тип View диалога.</typeparam>
		/// <returns>
		/// Возвращаемое значение такое же как у System.Windows.Window.ShowDialog().
		/// </returns>
		public bool? ShowDialog<T>(object dialogViewModel, IWindow owner) where T : Window, new()
		{
			return null;
		}

		/// <summary>
		/// Показать окно в немодальном режиме, owner у окна указывается текущее активное окно
		/// </summary>
		/// <param name="viewModel">ViewModel окна.</param>
		/// <returns>Интерфейс окна для управления закрытием</returns>
		public IWindow Show<T>(object viewModel) where T : Window, IWindow, new()
		{
			return null;
		}

		/// <summary>
		/// Показать окно в немодальном режиме
		/// </summary>
		/// <typeparam name="T">Тип View для окна.</typeparam>
		/// <param name="viewModel">ViewModel окна.</param>
		/// <param name="owner">Владелец создаваемого окна</param>
		/// <returns>Интерфейс окна для управления закрытием</returns>
		public IWindow Show<T>(object viewModel, IWindow owner) where T : Window, IWindow, new()
		{
			return null;
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
		public MessageBoxResult ShowMessageDialog(string message, string caption, MessageBoxButton button = MessageBoxButton.OK,
			MessageBoxImage icon = MessageBoxImage.Asterisk)
		{
			return MessageBoxResult.None;
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
		public MessageBoxResult ShowMessageDialog(string message, MessageBoxButton button = MessageBoxButton.OK,
			MessageBoxImage icon = MessageBoxImage.Asterisk)
		{
			return MessageBoxResult.None;
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
		public MessageBoxResult ShowMessageDialog(string message, string caption, MessageBoxButton button, MessageBoxImage icon,
			MessageBoxResult defaultButton, MessageBoxOptions options)
		{
			return MessageBoxResult.None;
		}

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
		public MessageBoxResult ShowMessageDialog(IWindow owner, string message, string caption = null,
			MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Asterisk,
			MessageBoxResult defaultButton = MessageBoxResult.OK, MessageBoxOptions options = MessageBoxOptions.None)
		{
			return MessageBoxResult.None;
		}

		/// <summary>
		/// Показать диалог с сообщением об ошибке для активного окна.
		/// </summary>
		public void ShowErrorMessageDialog(string message, string details, string caption = null)
		{
		}

		/// <summary>
		/// Показать диалог с сообщением об ошибке с указанием owner.
		/// </summary>
		public void ShowErrorMessageDialog(IWindow owner, string message, string details, string caption = null)
		{
		}

		/// <summary>
		/// Показать диалог открытия файла
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="fileName"></param>
		/// <param name="owner">Владелец создаваемого окна</param>
		/// <returns></returns>
		public bool? ShowOpenFileDialog(string filter, out string fileName, IWindow owner = null)
		{
			fileName = null;
			return null;
		}

		/// <summary>
		/// Показать диалог сохранения файла
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="defFileName"></param>
		/// <param name="fileName"></param>
		/// <param name="owner">Владелец создаваемого окна</param>
		/// <returns></returns>
		public bool? ShowSaveFileDialog(string filter, string defFileName, out string fileName, IWindow owner = null)
		{
			fileName = null;
			return null;
		}
	}
}