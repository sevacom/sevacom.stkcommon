using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;
using StkCommon.Data;
using StkCommon.Data.Collections;
using StkCommon.Data.Repositories;

namespace StkCommon.UI.Wpf.Repositories
{
	/// <summary>
	/// Коллекция вьюмоделей, автоматически синхронизирующая свое состояние с коллекцией моделей
	/// </summary>
	/// <typeparam name="TIVm">Тип вьюмодели или ее абстракции</typeparam>
	/// <typeparam name="TM">Тип модели</typeparam>
	/// <typeparam name="TId">Тип идентификатора</typeparam>
	public class SourceSynchronizedViewModelCollection<TIVm, TM, TId> : ObservableKeyedCollection<TId, TIVm>,
		IViewedObservableCollection<TIVm>
		where TIVm : class
	{
		private Func<TM, bool> _modelFilter;

		private readonly IDispatcher _dispatcher;

		private IElementRepository<TM, TId> _source;

		private ListCollectionView _collectionView;
		private Func<TIVm, TIVm, int> _customSort;
		private GroupDescription _groupDescription;
		private readonly Func<TM, TIVm> _viewModelFactory;
		private readonly Func<TIVm, TId> _getViewModelKey;
		private readonly Action<TM, TIVm> _updateViewModel;

		/// <param name="viewModelFactory">Метод создания вью моделей из модели</param>
		/// <param name="getViewModelKey">Метод получения ключа вью модели</param>
		/// <param name="updateViewModel">Метод обновления вью модели данными из модели</param>
		/// <param name="dispatcher">Диспатчер</param>
		public SourceSynchronizedViewModelCollection(Func<TM, TIVm> viewModelFactory, Func<TIVm, TId> getViewModelKey,
			Action<TM, TIVm> updateViewModel, IDispatcher dispatcher)
			: base(getViewModelKey)
		{
			_dispatcher = dispatcher;
			_modelFilter = ConstFilters.NoElementsFilter;
			_viewModelFactory = viewModelFactory;
			_getViewModelKey = getViewModelKey;
			_updateViewModel = updateViewModel;
		}

		/// <summary>
		///     Коллекция моделей
		/// </summary>
		public IElementRepository<TM, TId> Source
		{
			get { return _source; }
			set
			{
				if (value == _source)
				{
					return;
				}

				// Отписка от предыдущего источника
				if (null != _source)
				{
					_source.Changed.Remove(this);
				}
				_source = value;
				_source.Changed.Add(this, (t, s, a) => OnSourceChanged(a));
				SyncFromSource();
			}
		}

		protected void DispatherInvoke(Action action)
		{
			if (_dispatcher.CheckAccess())
				action();
			else
				_dispatcher.Invoke(action, DispatcherPriority.Background);
		}

		public ICollectionView CollectionView
		{
			get
			{
				if (_collectionView == null)
				{
					DispatherInvoke(() => _collectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(this));
					if (CustomSort != null)
						_collectionView.CustomSort = FunctionalComparer<TIVm>.Create(CustomSort);
					if (GroupDescription != null && _collectionView.GroupDescriptions != null)
						_collectionView.GroupDescriptions.Add(_groupDescription);
				}

				return _collectionView;
			}
		}


		/// <summary>
		/// Сортирующая функция
		/// </summary>
		public Func<TIVm, TIVm, int> CustomSort
		{
			get { return _customSort; }
			set
			{
				_customSort = value;

				if (_collectionView != null)
					_collectionView.CustomSort = FunctionalComparer<TIVm>.Create(_customSort);
			}
		}

		/// <summary>
		/// Группировка
		/// </summary>
		public GroupDescription GroupDescription
		{
			get { return _groupDescription; }
			set
			{
				_groupDescription = value;
				if (_collectionView != null && _collectionView.GroupDescriptions != null)
				{
					_collectionView.GroupDescriptions.Clear();
					_collectionView.GroupDescriptions.Add(_groupDescription);
				}
			}
		}

		/// <summary>
		/// Возвращает элементы входящие в конкретную группу, если есть группировка
		/// </summary>
		public IEnumerable<TIVm> GetGroupItems(string groupName)
		{
			return
				(
					from CollectionViewGroup g in CollectionView.Groups
					where g.Name.ToString() == groupName
					select g.Items.Cast<TIVm>()
					).FirstOrDefault();
		}

		/// <summary>
		///     Фильтрующая функция
		/// </summary>
		public Func<TM, bool> ModelFilter
		{
			get { return _modelFilter; }
			set
			{
				_modelFilter = value;
				// При изменении фильтра делается перезапрос данных из источника
				SyncFromSource();
			}
		}

		protected void SyncFromSource()
		{
			// Если есть источник
			if (Source != null)
			{
				var shouldSyncAgain = false; // признак того что данную функцию нужно перезапустить в главном потоке, иначе в некоторых случаях возможен дедлок
				lock (Source.SyncObject)
				{
					if (_modelFilter != ConstFilters.NoElementsFilter)
					{
						// Получение от него всех данных
						var items = Source.Where(item => IsSuccessFilter(item.Value)).ToArray();

						bool shouldUpdate = items.Length != Count;

						if (!shouldUpdate) if (items.Any(item => !Contains(item.Key))) shouldUpdate = true;

						if (shouldUpdate)
						{
							if (_dispatcher.CheckAccess()) DispatherInvoke(() => ReplaceAll(items.Select(item => _viewModelFactory(item.Value))));
							else shouldSyncAgain = true;
						}
					}
					else if (Count != 0)
					{
						if (_dispatcher.CheckAccess()) DispatherInvoke(() => ReplaceAll(new TIVm[0]));
						else shouldSyncAgain = true;
					}
				}
				if (shouldSyncAgain)
				{
					DispatherInvoke(SyncFromSource);
				}
			}
		}

		protected void RemoveViewModelForModel(TId valueId)
		{
			TIVm item = this.FirstOrDefault(vm => Equals(_getViewModelKey(vm), valueId));
			if (null != item)
			{
				Remove(item);
			}
		}

		protected void OnSourceChanged(IElementRepositoryChangedArg<TM, TId> arg)
		{
			DispatherInvoke(() =>
			{
				foreach (var changedElement in arg.Elements)
				{
					switch (changedElement.ChangedType)
					{
						case ChangedElementType.Added:
							if (IsSuccessFilter(changedElement.Value))
								AddOnSourceChanged(_viewModelFactory, changedElement.Value, changedElement.ValueId);
							break;
						case ChangedElementType.Removed:
							RemoveViewModelForModel(changedElement.ValueId);
							break;
						case ChangedElementType.Changed:
							{
								if (IsSuccessFilter(changedElement.Value))
									AddOnSourceChanged(_viewModelFactory, changedElement.Value, changedElement.ValueId);
								else
									Remove(changedElement.ValueId);
							}
							break;
						default:
							throw new NotSupportedException();
					}
				}
			});
		}

		private void AddOnSourceChanged(Func<TM, TIVm> factory, TM m, TId id)
		{
			if (!Contains(id))
			{
				TIVm item = factory(m);
				Add(item);
			}
			else
			{
				var oldItem = this[id];
				_updateViewModel(m, oldItem);
				Replace(oldItem);

				// Если используется сортированное и группированное представление - сообщаем об изменении элемента
				if (_collectionView != null)
					_collectionView.Refresh();
			}
		}

		private bool IsSuccessFilter(TM m)
		{
			var filter = ModelFilter;

			return filter == null || filter(m);
		}

		public new TIVm this[int index]
		{
			get { return base[index]; }
		}

		public new IEnumerator<TIVm> GetEnumerator()
		{
			return base.GetEnumerator();
		}
	}


	public static class ConstFilters
	{
		public static bool NoElementsFilter<T>(T m)
		{
			return false;
		}

		public static bool AllElementsFilter<T>(T m)
		{
			return true;
		}
	}
}