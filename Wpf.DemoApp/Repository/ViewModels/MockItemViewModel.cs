namespace StkCommon.UI.Wpf.DemoApp.Repository.ViewModels
{
	class MockItemViewModel : IItemViewModel
	{
		public MockItemViewModel(string name, bool isChecked)
		{
			Name = name;
			IsChecked = isChecked;
		}

		public int Id { get; private set; }
		public bool IsChecked { get; set; }
		public string Name { get; set; }
		public void Parse(Model model)
		{
			throw new System.NotImplementedException();
		}

		public int ModelId { get; private set; }
	}
}