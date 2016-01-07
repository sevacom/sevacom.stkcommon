using System.Collections.Generic;
using StkCommon.UI.Wpf.Model;
using StkCommon.UI.Wpf.ViewModels;
using StkCommon.UI.Wpf.Views;

namespace StkCommon.UI.Wpf.DemoApp
{
	/// <summary>
	/// Вьюмодель окна авторизации (аутентификации) 
	/// </summary>
	public class AuthorizationViewModel : AuthorizationViewModelBase
	{
		public AuthorizationViewModel(AuthorizationMode mode, IShowDialogAgent agent)
			: base(mode, agent)
		{
		}

		/// <summary>
		/// Реализация метода авторизации: здесь непосредственно выполняется вход в систему
		/// При успешной авторизации возвращаем true, иначе - false или
		/// бросаются исключения типа UserMessageException или производных от него. 
		/// </summary>
		/// <returns></returns>
		protected override bool Authorize()
		{
			//throw new UserMessageException("Сервер недоступен");
			return true;
		}

		protected override void OnServersDropDownChanged(bool isDropDown)
		{
			if (isDropDown)
			{
				Servers = new List<string> { "Сервер1", "Сервер2", "Сервер3", "Сервер4" };
			}
		}

		protected override void OnDataBasesDropDownChanged(bool isDropDown)
		{
			if (isDropDown)
			{
				Databases = new List<string> { "БазаДанных1", "БазаДанных2", "БазаДанных3", "БазаДанных4" };
			}
		}
	}
}
