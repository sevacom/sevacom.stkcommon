using System;
using System.Windows.Input;
using StkCommon.Data.Repositories;
using StkCommon.UI.Wpf.Commands;
using StkCommon.UI.Wpf.DemoApp.Repository.Views;
using StkCommon.UI.Wpf.Repositories;
using StkCommon.UI.Wpf.Views;

namespace StkCommon.UI.Wpf.DemoApp.Repository.ViewModels
{
	public class RepositoryExampleWindowViewModel : IRepositoryExampleWindowViewModel
	{
		private readonly IServerCommands _serverCommands;
		private readonly IShowDialogAgent _showDialogAgent;

		public RepositoryExampleWindowViewModel(IElementRepository<Model, int> modelRepository, IServerCommands serverCommands, 
			IShowDialogAgent showDialogAgent, IDispatcher dispatcher)
		{
			_serverCommands = serverCommands;
			_showDialogAgent = showDialogAgent;

			AddItemCommand = new DelegateCommand(AddItemCommandHandler);
			EditItemCommand = new DelegateCommand(EditItemCommandHandler);
			DeleteItemCommand = new DelegateCommand(DeleteItemCommandHandler);
			Func<Model, IItemViewModel> factory = model => { var vm = new ItemViewModel(); vm.Parse(model); return vm;};
			AllItemViewModels = new SourceSynchronizedViewModelCollection<IItemViewModel, Model, int>(factory, vm => vm.Id, (model, vm) => ((ItemViewModel)vm).Parse(model), dispatcher)
				{ Source = modelRepository, ModelFilter = ConstFilters.AllElementsFilter, 
				CustomSort = (m1, m2) => -String.Compare(m1.Name, m2.Name, StringComparison.CurrentCulture) };
			CheckedItemViewModels = new SourceSynchronizedViewModelCollection<IItemViewModel, Model, int>(factory, vm => vm.Id, (model, vm) => ((ItemViewModel)vm).Parse(model), dispatcher) 
				{ Source = modelRepository, ModelFilter = m => m.IsChecked, 
				CustomSort = (m1, m2) => String.Compare(m1.Name, m2.Name, StringComparison.CurrentCulture) };
		}

		private void AddItemCommandHandler(object o)
		{
			var vm = new ItemViewModel();
			vm.Parse(new Model());
			_showDialogAgent.ShowDialog<ItemPropertiesWindow>(vm);
			_serverCommands.AddItem(vm.Model);
		}

		private void EditItemCommandHandler(object obj)
		{
			var vm = (ItemViewModel)obj;
			_showDialogAgent.ShowDialog<ItemPropertiesWindow>(vm);
			_serverCommands.EditItem(vm.Model);
		}

		private void DeleteItemCommandHandler(object obj)
		{
			var vm = (ItemViewModel)obj;
			_serverCommands.DeleteItem(vm.Id);			
		}

		public IViewedObservableCollection<IItemViewModel> AllItemViewModels { get; private set; }
		public IViewedObservableCollection<IItemViewModel> CheckedItemViewModels { get; private set; }
		public ICommand AddItemCommand { get; private set; }
		public ICommand EditItemCommand { get; private set; }
		public ICommand DeleteItemCommand { get; private set; }
	}
}