using StkCommon.UI.Wpf.Commands;
using StkCommon.UI.Wpf.ViewModels;

namespace StkCommon.UI.Wpf.DemoApp
{
	public class DemoViewModel: ViewModelBase
	{
		public DemoViewModel()
		{
			SimpleCommand = new SimpleDelegateCommand(ExecuteMethod, CanExecuteMethod, 
				"DemoCommand");
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
