using System;
using System.Windows.Media.Imaging;

namespace StkCommon.UI.Wpf.ViewModels.DesignTime
{
	internal class DesignMockAboutViewModel: AboutViewModel
	{
		public DesignMockAboutViewModel()
		{
			ApplicationIcon = new BitmapImage(new Uri(@"../Resources/AppIcon.ico", UriKind.Relative));
			ApplicationName = "Имя приложения";
			ApplicationVersion = "1.2.3";
			AdditionalInfo = "Версия ядра 1.2";
			Copyright = "2014 DesignTimeCompany";
			Telephone = "+7 (888) 333-88-99 (многоканальный)";
			Fax = "+7 (888) 222-88-99 доб. 111";
			SiteUrl = @"http://www.site.com";
			Email = "mail@site.com";
		}
	}
}
