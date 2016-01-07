namespace StkCommon.UI.Wpf.DemoApp.Repository
{
	public interface IServerCommands
	{
		void AddItem(Model m);
		void EditItem(Model m);
		void DeleteItem(int id);
	}
}