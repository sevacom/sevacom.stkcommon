using System.Windows.Input;
using StkCommon.UI.Wpf.Repositories;

namespace StkCommon.UI.Wpf.DemoApp.Repository.ViewModels
{
	public interface IRepositoryExampleWindowViewModel
	{
		IViewedObservableCollection<IItemViewModel> AllItemViewModels { get; }

		IViewedObservableCollection<IItemViewModel> CheckedItemViewModels { get; }

		ICommand AddItemCommand { get; }
		ICommand EditItemCommand { get; }
		ICommand DeleteItemCommand { get; }
	}
}