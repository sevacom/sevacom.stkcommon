using System.Windows.Media;

namespace StkCommon.UI.Wpf.ViewModels
{
	/// <summary>
	/// Интерфейс вьюмодели заставки приложения при запуске
	/// </summary>
	public interface ISplashScreenViewModel
	{
		/// <summary>
		/// Название приложения
		/// </summary>
		string ApplicationName { get; }

		/// <summary>
		/// Версия приложения
		/// </summary>
		string ApplicationVersion { get; }

		/// <summary>
		/// Иконка приложения
		/// </summary>
		ImageSource ApplicationIcon { get; }

		/// <summary>
		/// год и название компании
		/// </summary>
		string Copyright { get; }

		/// <summary>
		/// Название и версия комплекса
		/// </summary>
		string ApkVersion { get;  }

		/// <summary>
		/// Текущий загружаемый модуль
		/// </summary>
		string CurrentLoadingModuleName { get; }
	}

	public class SplashScreenViewModel: ViewModelBase, ISplashScreenViewModel
	{
		private string _currentLoadingModuleName;
		
		public string ApplicationName { get; set; }
		
		public string ApplicationVersion { get; set; }
		
		public ImageSource ApplicationIcon { get; set; }
		
		public string Copyright { get; set; }
		
		public string ApkVersion { get; set; }
		
		public string CurrentLoadingModuleName
		{
			get { return _currentLoadingModuleName; }
			set
			{
				if (_currentLoadingModuleName == value)
					return;
				_currentLoadingModuleName = value;

				OnPropertyChanged(() => CurrentLoadingModuleName);
			}
		}
	}
}
