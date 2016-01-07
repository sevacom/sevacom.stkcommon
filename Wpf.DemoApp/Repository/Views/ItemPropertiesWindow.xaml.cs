using System.Windows;

namespace StkCommon.UI.Wpf.DemoApp.Repository.Views
{
	/// <summary>
	/// Interaction logic for ItemPropertiesWindow.xaml
	/// </summary>
	public partial class ItemPropertiesWindow : Window
	{
		public ItemPropertiesWindow()
		{
			InitializeComponent();
		}

		private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
