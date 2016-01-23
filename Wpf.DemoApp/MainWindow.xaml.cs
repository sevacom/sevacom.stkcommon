using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using StkCommon.UI.Wpf.Exceptions;
using StkCommon.UI.Wpf.Model;
using StkCommon.UI.Wpf.ViewModels;
using StkCommon.UI.Wpf.Views;
using StkCommon.UI.Wpf.Views.Windows;

namespace StkCommon.UI.Wpf.DemoApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IWindow
	{
		private IWindow _splashScreenWindow;
		private readonly IShowDialogAgent _showDialogAgent;
		private DispatcherAdapter _dispatcher;

		public MainWindow()
		{
			InitializeComponent();
			_dispatcher = new DispatcherAdapter(Dispatcher);
			_showDialogAgent = new ShowDialogAgent(_dispatcher);
		}

		private void ShowAboutWindowClick(object sender, RoutedEventArgs e)
		{
			var aboutViewModel = new AboutViewModel
				{
					ApplicationName = "Журнал сеансов",
					ApplicationVersion = "1.2.0",
					ApplicationIcon = new BitmapImage(new Uri("pack://application:,,,/Wpf.DemoApp;component/Resources/AppIcon.ico", UriKind.Absolute)),
					AdditionalInfo = "Версия ядра 1.2",
					Copyright = "2014 Сигнатек",
					Telephone = "+7 (888) 333-88-99",
					Fax = "+7 (888) 222-88-99 доб. 111",
					SiteUrl = @"http://www.site.com",
					Email = "mail@site.com",
				};

			_showDialogAgent.ShowDialog<AboutWindow>(aboutViewModel);
		}

		private void ShowSplashScreenWindowClick(object sender, RoutedEventArgs e)
		{
			CloseSplashScreenWindowClick(this, new RoutedEventArgs());

			var splashViewModel = new SplashScreenViewModel
				{
				ApplicationName = "Журнал сеансов",
				ApplicationVersion = "Версия 1.2.0.235",
				ApplicationIcon = new BitmapImage(new Uri("pack://application:,,,/Wpf.DemoApp;component/Resources/AppIcon.ico", UriKind.Absolute)),
				Copyright = "2014 Сигнатек",
				ApkVersion = "Версия ядра 15.0",
				CurrentLoadingModuleName = "Инициализация приложения..."
			};

			_splashScreenWindow = _showDialogAgent.Show<SplashScreenWindow>(splashViewModel, this);
		}

		private void CloseSplashScreenWindowClick(object sender, RoutedEventArgs e)
		{
			if (_splashScreenWindow != null)
			{
				_splashScreenWindow.Close();
				_splashScreenWindow = null;
			}
		}

		private void ShowAuthorizationWindowClick(object sender, RoutedEventArgs e)
		{
			
			var authorizationViewModel = new AuthorizationViewModel(AuthorizationMode.ServerDatabase, _showDialogAgent)
			{
				ApplicationName = "Новое приложение",
				ApplicationIcon = new BitmapImage(new Uri("pack://application:,,,/Wpf.DemoApp;component/Resources/AppIcon.ico", UriKind.Absolute)),
				Servers = new List<string> { "Server1" }
			};

			var result = _showDialogAgent.ShowDialog<AuthorizationWindow>(authorizationViewModel);
			if (result.HasValue && result.Value)
			{
				// здесь можно при необходимости использовать свойства authorizationViewModel
			}
		}

		private void ShowStkCommonUiWpfTheme(object sender, RoutedEventArgs e)
		{
			var themeWindow = new WpfThemeWindow {Owner = this};
			themeWindow.ShowDialog();
		}

		private void ShowControlsWindow(object sender, RoutedEventArgs e)
		{
			_showDialogAgent.ShowDialog<ControlsWindow>(new DesignMockControlsWindowVm());
		}

		private void ShowErrorMessageWindow(object sender, RoutedEventArgs e)
		{
			var details = "Произошла ошибка при добавлении компонента в базу данных.\r\nПроверьте подключение к базе данных или указанная база данных не существует.";

			_showDialogAgent.ShowErrorMessageDialog("Ошибка с подробностями аворп лоывар полывар полыварп олывар олывар лоыва hgkjsdfh gsdfg", details);
			_showDialogAgent.ShowErrorMessageDialog("Ошибка без подробностей", null);
			_showDialogAgent.ShowErrorMessageDialog(@"Ошибка
sdf
sdf
sdf
sdf
sdf
sd
f
sdf
sdf
sdf
sdf
sdf
sd
fs
fs
dfs
df
sdf
sd
f
sdf
sd
f
sdf
sf
s
f
sd
s
df
s
d
f
sd
fsg

fg
sdf
gs
df
gs
вап
ы
вап
ы
вап
f
s
df
sd
s
f
sdбез подробностей", @"
sdfasdf
a
df
фав
ф
а
adfa
sdf
a
df
a
ваф
ва
ф
ва
ф
ва
adf
a
df
as
fd
asfd
asd
f
ad
f
asdfa
df
a
df
a
df
a
df
a
df");
			_showDialogAgent.ShowErrorMessageDialog(this, "Ошибка без подробностей, указан owner", details, "Заголовок окна");

			_showDialogAgent.ShowMessageDialog("Ошибка через стандартный вызов", "Заголовок окна", MessageBoxButton.OK, MessageBoxImage.Error);

			var exception = new ArgumentNullException("Отсутствует параметр dispatcher.");
			var notifier = new MessageBoxExceptionNotifierWithDetails(_showDialogAgent);
			notifier.Notify(exception, "Ошибка через Notify с не пустым errorMessage");
			notifier.Notify(exception, null);
			notifier.Notify("Ошибка через Notify без Exception");
		}
	}
}
