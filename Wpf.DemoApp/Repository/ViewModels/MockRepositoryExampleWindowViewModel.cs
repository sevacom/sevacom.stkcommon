using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using StkCommon.UI.Wpf.Repositories;

namespace StkCommon.UI.Wpf.DemoApp.Repository.ViewModels
{
	public class MockRepositoryExampleWindowViewModel : IRepositoryExampleWindowViewModel
	{
		public MockRepositoryExampleWindowViewModel()
		{
			AllItemViewModels =
				new MockViewedObservableCollection(new[] { new MockItemViewModel("Лондон", true), new MockItemViewModel("Париж", false), new MockItemViewModel("Нью-Йорк", false) });
			CheckedItemViewModels = new MockViewedObservableCollection(new[] { new MockItemViewModel("Лондон", true)});
		}

		public IViewedObservableCollection<IItemViewModel> AllItemViewModels { get; private set; }
		public IViewedObservableCollection<IItemViewModel> CheckedItemViewModels { get; private set; }
		public ICommand AddItemCommand { get; private set; }
		public ICommand EditItemCommand { get; private set; }
		public ICommand DeleteItemCommand { get; private set; }

		private class MockViewedObservableCollection : ObservableCollection<IItemViewModel>, IViewedObservableCollection<IItemViewModel>
		{
			public MockViewedObservableCollection(IEnumerable<IItemViewModel> items) : base(items)
			{ }
			public ICollectionView CollectionView { get { return CollectionViewSource.GetDefaultView(this); } }
			public Func<IItemViewModel, IItemViewModel, int> CustomSort { set; private get; }
			public GroupDescription GroupDescription { get; set; }
			public IEnumerable<IItemViewModel> GetGroupItems(string groupName)
			{
				throw new NotImplementedException();
			}
		}
	}
}