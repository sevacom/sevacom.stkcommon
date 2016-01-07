using System.Collections.Generic;
using System.Linq;
using StkCommon.Data.Repositories;

namespace StkCommon.UI.Wpf.Repositories.Test.TestHelpers
{
	public class ModelRepository : ElementRepositoryBase<TestModel, int>
	{
		protected override void LoadDataIfNeed()
		{
			Items = new[]
			        {
				        new TestModel(1, "Астрахань"),
						new TestModel(2, "Новосибирск"),
						new TestModel(3, "Бердск"),
						new TestModel(4, "Москва"),
						new TestModel(5, "Омск"),
						new TestModel(6, "Рязань"),
						new TestModel(7, "Владивосток"),
						new TestModel(8, "Иркутск"),
						new TestModel(9, "Хабаровск"),
						new TestModel(10, "Томск")
			        }.ToDictionary(i => i.Id, i => i);
		}

		public void AddItem(TestModel m)
		{
			lock (SyncObject)
			{
				var id = Items.Max(i => i.Key) + 1;
				m.Id = id;
				Items[id] = m;
			}
			RaiseElementChanged(ChangedElementType.Added, m, null, m.Id);
		}

		public void EditItem(TestModel m)
		{
			TestModel oldM;
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

		public IDictionary<int, TestModel> Data {
			get { return Items; }
		} 
	}

	public class TestModel
	{
		public TestModel(int id, string caption)
		{
			Id = id;
			Caption = caption;
		}
		public int Id;
		public string Caption;
	}
}
