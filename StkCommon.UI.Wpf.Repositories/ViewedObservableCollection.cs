using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using StkCommon.Data;

namespace StkCommon.UI.Wpf.Repositories
{
	/// <summary>
	/// Несинхронизованная с источником реализация IViewedObservableCollection. Позволяет вручную добавлять/удалять элементы, можно инициализировать коллекцией
	/// </summary>
	public class ViewedObservableCollection<T> : ObservableCollection<T>, IViewedObservableCollection<T>
	{
		private ListCollectionView _collectionView;
		private GroupDescription _groupDescription;
		private Func<T, T, int> _customSort;

		#region Implementation of IViewedObservableCollection<out T>

		/// <summary>
		/// Представление которое будет использоваться для сортировки и группировки элементов
		/// </summary>
		public ICollectionView CollectionView
		{
			get
			{
				if (_collectionView == null)
				{
					_collectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(this);
					if (CustomSort != null)
						_collectionView.CustomSort = FunctionalComparer<T>.Create(CustomSort);
					if (GroupDescription != null && _collectionView.GroupDescriptions != null)
						_collectionView.GroupDescriptions.Add(_groupDescription);
				}

				return _collectionView;
			}
		}

		/// <summary>
		/// Сортирующая функция
		/// </summary>
		public Func<T, T, int> CustomSort
		{
			get { return _customSort; }
			set
			{
				_customSort = value;

				if (_collectionView != null)
					_collectionView.CustomSort = FunctionalComparer<T>.Create(_customSort);
			}
		}

		/// <summary>
		/// Группировка
		/// </summary>
		public GroupDescription GroupDescription
		{
			get { return _groupDescription; }
			set { _groupDescription = value; }
		}

		/// <summary>
		/// Возвращает элементы входящие в конкретную группу, если есть группировка
		/// </summary>
		public IEnumerable<T> GetGroupItems(string groupName)
		{
			return
				(
					from CollectionViewGroup g in CollectionView.Groups
					where g.Name.ToString() == groupName
					select g.Items.Cast<T>()
					).FirstOrDefault();
		}

		#endregion
	}
}