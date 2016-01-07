namespace StkCommon.UI.Wpf.DemoApp.Repository
{
	public class Model
	{
		public Model()
		{
			Name = "";
		}
		public Model(int id, bool isChecked, string name)
		{
			Id = id;
			IsChecked = isChecked;
			Name = name;
		}
		public int Id;
		public bool IsChecked;
		public string Name;
	}
}