namespace StkCommon.UI.Wpf.Model
{
	/// <summary>
	/// Режимы работы окна авторизации (аутентификации)
	/// </summary>
	public enum AuthorizationMode
	{
		/// <summary>
		/// Логин и пароль
		/// </summary>
		LoginPassword,
		
		/// <summary>
		/// Логин, пароль и сервер 
		/// </summary>
		Server,

		/// <summary>
		/// Логин, пароль и база данных
		/// </summary>
		Database,

		/// <summary>
		/// Логин, пароль, сервер, база данных
		/// </summary>
		ServerDatabase
	}
}
