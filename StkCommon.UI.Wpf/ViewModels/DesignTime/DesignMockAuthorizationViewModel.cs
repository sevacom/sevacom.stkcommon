using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using StkCommon.UI.Wpf.Model;

namespace StkCommon.UI.Wpf.ViewModels.DesignTime
{
	internal class DesignMockAuthorizationViewModel : AuthorizationViewModelBase
	{
		public DesignMockAuthorizationViewModel() : base(AuthorizationMode.ServerDatabase, null)
		{
			ApplicationName = "Новое приложение";
			ApplicationIcon =
				new BitmapImage(new Uri("pack://application:,,,/Wpf.DemoApp;component/Resources/AppIcon.ico", UriKind.Absolute));
			UserName = "admin";
			Password = "123456";
			Servers = new List<string> { "Server1" };
			Server = "сервер";
			Databases = new List<string> { "База данных 1", "База данных 2"};
			Database = "база данных";
		}

		protected override bool Authorize()
		{
			return true;
		}
	}
}
