using System.Linq;
using StkCommon.Data.Repositories;

namespace StkCommon.UI.Wpf.DemoApp.Repository
{
	public class ModelRepository : ElementRepositoryBase<Model, int>, IServerCommands
	{
		protected override void LoadDataIfNeed()
		{
			Items = new[]
			        {
				        new Model(1, true, "Астрахань"),
						new Model(2, true, "Новосибирск"),
						new Model(3, false, "Бердск"),
						new Model(4, true, "Москва"),
						new Model(5, false, "Омск"),
						new Model(6, true, "Рязань"),
						new Model(7, false, "Владивосток"),
						new Model(8, true, "Иркутск"),
						new Model(9, false, "Хабаровск"),
						new Model(10, true, "Томск")
			        }.ToDictionary(i => i.Id, i => i);
		}

		public void AddItem(Model m)
		{
			lock (SyncObject)
			{
				var id = Items.Max(i => i.Key) + 1;
				m.Id = id;
				Items[id] = m;
			}
			RaiseElementChanged(ChangedElementType.Added, m, null, m.Id);
		}

		public void EditItem(Model m)
		{
			Model oldM;
			lock (SyncObject)
			{
				Items.TryGetValue(m.Id, out oldM);
				Items[m.Id] = m;
			}
			RaiseElementChanged(ChangedElementType.Changed, m, oldM, m.Id);
		}

		public void DeleteItem(int id)
		{
			lock (SyncObject)
			{
				Items.Remove(id);
			}
			RaiseElementChanged(ChangedElementType.Removed, null, null, id);
		}
	}
}
