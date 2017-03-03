using System.Collections.ObjectModel;
using System.Windows.Input;
using StkCommon.UI.Wpf.Commands;
using StkCommon.UI.Wpf.ViewModels;

namespace StkCommon.UI.Wpf.DemoApp
{
	public class DesignMockControlsWindowVm : ViewModelBase
	{
		private readonly ObservableCollection<string> _collection = new ObservableCollection<string>();
		private string _selectedItem;
		private string _searchText;
	    private string _editableText;

	    public DesignMockControlsWindowVm()
		{
			_collection.Add("Первый");
			_collection.Add("Второй");
            _collection.Add("");

			SearchCommand = new DelegateCommand(SearchCommandHandler);
		}

		public ObservableCollection<string> Collection
		{
			get { return _collection; }
		}

		public string SearchText
		{
			get { return _searchText; }
			set
			{
				if (_searchText == value)
					return;

				_searchText = value;
				OnPropertyChanged("SearchText");
			}
		}

		public ICommand SearchCommand { get; private set; }

		public string SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				if (_selectedItem == value)
					return;

				_selectedItem = value;
				OnPropertyChanged("SelectedItem");
			}
		}

	    public string EditableText
	    {
	        get { return _editableText; }
	        set
	        {
	            if (_editableText == value)
                    return;

	            _editableText = value;
	            OnPropertyChanged(() => EditableText);
	        }
	    }

	    private void SearchCommandHandler(object obj)
		{

		}
	}
}
