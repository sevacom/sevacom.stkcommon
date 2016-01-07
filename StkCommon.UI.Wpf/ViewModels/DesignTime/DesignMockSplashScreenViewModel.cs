using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StkCommon.UI.Wpf.ViewModels.DesignTime
{
	/// <summary>
	/// ВМ заставки приложения при запуске
	/// </summary>
	internal class DesignMockSplashScreenViewModel : ISplashScreenViewModel
	{
		public DesignMockSplashScreenViewModel()
		{
			ApplicationName = "Имя приложения";
			ApplicationVersion = "Версия 1.2.0.235";
			var uriSource = new Uri(@"../Resources/AppIcon.ico", UriKind.Relative);
			ApplicationIcon = new BitmapImage(uriSource);
			Copyright = "2014 DesignTimeCompany";
			ApkVersion = "Версия ядра 15.0";
			CurrentLoadingModuleName = "Инициализация приложения...";
		}
		/// <summary>
		/// Название приложения
		/// </summary>
		public string ApplicationName { get; private set; }
		/// <summary>
		/// Версия приложения
		/// </summary>
		public string ApplicationVersion { get; private set; }
		/// <summary>
		/// Иконка приложения
		/// </summary>
		public ImageSource ApplicationIcon { get; private set; }
		/// <summary>
		/// год и название компании
		/// </summary>
		public string Copyright { get; private set; }
		/// <summary>
		/// Название и версия комплекса
		/// </summary>
		public string ApkVersion { get; private set; }
		/// <summary>
		/// Текущий загружаемый модуль
		/// </summary>
		public string CurrentLoadingModuleName { get; private set; }
	}
}