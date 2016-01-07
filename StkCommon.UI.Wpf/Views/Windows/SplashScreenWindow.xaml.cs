using System.Windows;

namespace StkCommon.UI.Wpf.Views.Windows
{
	/// <summary>
	/// Заставка приложения при запуске 
	/// Поддерживает ISplashScreenViewModel
	/// </summary>
	public partial class SplashScreenWindow : Window, IWindow
	{
		public SplashScreenWindow()
		{
			InitializeComponent();
		}
	}
}
