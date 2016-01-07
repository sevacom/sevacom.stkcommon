using System.Collections.Generic;
using System.Windows.Media;
using StkCommon.UI.Wpf.Model;

namespace StkCommon.UI.Wpf.ViewModels
{
	/// <summary>
	/// Интерфейс вьюмодели окна авторизации пользователя
	/// </summary>
	public interface IAuthorizationViewModel
	{
		/// <summary>
		/// Масштаб окна, по умолчанию 1.0
		/// </summary>
		double UiScale { get; set; }

		/// <summary>
		/// Логин пользователя
		/// </summary>
		string UserName { get; set; }

		/// <summary>
		/// Пароль пользователя
		/// </summary>
		string Password { get; set; }

		/// <summary>
		/// Выбранный сервер
		/// </summary>
		string Server { get; set; }

		/// <summary>
		/// Выбранная база данных
		/// </summary>
		string Database { get; set; }
		
		/// <summary>
		/// Иконка приложения
		/// </summary>
		ImageSource ApplicationIcon { get; set; }

		/// <summary>
		/// Название приложения
		/// </summary>
		string ApplicationName { get; set; }

		/// <summary>
		/// Признак управления закрытием окна в случае успешной авторизации 
		/// </summary>
		bool? DialogResult { get; set; }

		/// <summary>
		/// Режим работы окна авторизации
		/// </summary>
		AuthorizationMode Mode { get; }

		/// <summary>
		/// Список серверов
		/// </summary>
		IEnumerable<object> Servers { get; set; }

		/// <summary>
		/// Список баз данных
		/// </summary>
		IEnumerable<object> Databases { get; set; } 

	}
}