using System.Collections.ObjectModel;
using System.Windows;

namespace StkCommon.UI.Wpf.DemoApp
{
	/// <summary>
	/// Interaction logic for ControlsWindow.xaml
	/// </summary>
	public partial class ControlsWindow
	{
		public ControlsWindow()
		{
			InitializeComponent();
			DataContext = new DesignMockControlsWindowVm();
		    var b = new ObservableCollection<string> {"Раз", "Два", "Три", "Четыре", "Пять", "Вышел зайчик", "Погулять"};
		    List.ItemsSource = b;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			SearchComboBox1.FocusTextBox();
		}
	}
}
