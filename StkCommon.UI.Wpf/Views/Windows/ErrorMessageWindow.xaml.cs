using System.Windows;
using System.Windows.Input;
using StkCommon.UI.Wpf.ViewModels;

namespace StkCommon.UI.Wpf.Views.Windows
{
	/// <summary>
	/// Interaction logic for ErrorMessageWindow.xaml
	/// </summary>
	public partial class ErrorMessageWindow : BaseDialogWindow
	{
		public ErrorMessageWindow()
		{
			InitializeComponent();

			KeyDown += OnMessageWindowKeyDown;
		}

		private void OnMessageWindowKeyDown(object sender, KeyEventArgs e)
		{
			if ((e.Key == Key.C) && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
			{
				OnCopyClick(this, null);
			}
		}

		private void OnCopyClick(object sender, RoutedEventArgs e)
		{
			var vm = DataContext as ErrorMessageViewModel;
			if (vm != null)
			{
				Clipboard.SetText(vm.GetFullInfoText());
			}
		}
	}
}
