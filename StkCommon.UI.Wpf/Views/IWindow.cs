using System;
using System.ComponentModel;

namespace StkCommon.UI.Wpf.Views
{
	/// <summary>
	/// Интерфейс абстрактного окна
	/// </summary>
	public interface IWindow
	{
		/// <summary>
		/// Сообщение о том что окно закрыто
		/// </summary>
		event EventHandler Closed;

		/// <summary>
		/// Сообщение о закрытии окна с возможностью отмены
		/// </summary>
		event CancelEventHandler Closing;

		/// <summary>
		/// Активное окно или нет
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// Контекст данных для окна
		/// </summary>
		object DataContext { get; set; }

		/// <summary>
		/// Активировать окно
		/// </summary>
		bool Activate();

		/// <summary>
		/// Установить фокус
		/// </summary>
		bool Focus();

		/// <summary>
		/// Закрыть окно
		/// </summary>
		void Close();
	}
}
