using StkCommon.UI.Wpf.ViewModels;

namespace StkCommon.UI.Wpf.DemoApp.Repository.ViewModels
{
	public interface IItemViewModel
	{
		int Id { get; }
		bool IsChecked { get; set; }
		string Name { get; set; }
	}

	public class ItemViewModel : ViewModelBase, IItemViewModel
	{
		public int Id { get { return Model.Id; } }

		public bool IsChecked
		{
			get { return Model.IsChecked; }
			set { Model.IsChecked = value; }
		}

		public string Name
		{
			get { return Model.Name; }
			set { Model.Name = value; }
		}

		public Model Model { get; private set; }

		public void Parse(Model model)
		{
			Model = model;
			OnPropertyChanged("");
		}

	}
}
