using StkCommon.UI.Wpf.Commands;
using StkCommon.UI.Wpf.ViewModels;

namespace StkCommon.UI.Wpf.DemoApp
{
	public class MainWindowViewModel: ViewModelBase
	{
		public MainWindowViewModel()
		{
			SimpleCommand = new SimpleDelegateCommand(ExecuteMethod, CanExecuteMethod, 
				"DemoCommand");
		}

		public int SwitchTestProperty2
		{
			get { return 2; }
		}

		public int SwitchTestProperty3
		{
			get { return 3; }
		}

		private bool CanExecuteMethod()
		{
			return true;
		}

		private void ExecuteMethod()
		{
			
		}

		public IDelegateCommand SimpleCommand { get; private set; }
	}
}
