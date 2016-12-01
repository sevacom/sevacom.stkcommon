using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;

namespace StkCommon.Data.Collections
{
	/// <summary>
	/// Коллекция индексированных элементов на основе словаря с оповещением об изменениях. Оптимально по скорости модификации для больших коллекций
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TItem"></typeparam>
	public class ObservableKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem>, INotifyCollectionChanged, INotifyPropertyChanged
		where TItem : class
	{
		private readonly Func<TItem, TKey> _getKeyForItemDelegate;

		// Constructor now requires a delegate to get the key from the item
		public ObservableKeyedCollection(Func<TItem, TKey> getKeyForItemDelegate)
		{
			if (getKeyForItemDelegate == null)
				throw new ArgumentNullException("getKeyForItemDelegate");

			_getKeyForItemDelegate = getKeyForItemDelegate;
		}

		protected override TKey GetKeyForItem(TItem item)
		{
			return _getKeyForItemDelegate(item);
		}

		// Overrides a lot of methods that can cause collection change
		protected override void SetItem(int index, TItem item)
		{
			var oldItem = ((IList<TItem>)this)[index];
			base.SetItem(index, item);
			OnPropertyChanged(() => Items);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem));
		}

		protected override void InsertItem(int index, TItem item)
		{
			base.InsertItem(index, item);
			OnPropertyChanged(() => Count);
			OnPropertyChanged(() => Items);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
		}

		protected override void ClearItems()
		{
			base.ClearItems();
			OnPropertyChanged(() => Count);
			OnPropertyChanged(() => Items);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		protected override void RemoveItem(int index)
		{
			TItem item = this[index];
			base.RemoveItem(index);
			OnPropertyChanged(() => Count);
			OnPropertyChanged(() => Items);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
		}

		private bool _deferNotifyCollectionChanged;
		public void AddRange(IEnumerable<TItem> items)
		{
			_deferNotifyCollectionChanged = true;
			foreach (var item in items)
				Add(item);
			_deferNotifyCollectionChanged = false;

			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (_deferNotifyCollectionChanged)
				return;
			var handler = CollectionChanged;
			if (handler != null) handler(this, e);
		}

		/// <summary>
		///     Clears the current collection and replaces it with the specified item.
		/// </summary>
		public void Replace(TItem oldItem, TItem newItem)
		{
			if (newItem == null)
				throw new ArgumentNullException("newItem");

			if (oldItem == null)
				throw new ArgumentNullException("oldItem");

			var index = Items.IndexOf(oldItem);

			if (index == -1)
				throw new ArgumentException("Попытка замены элемента не содержащегося в коллекции!", "oldItem");

			base.SetItem(index, newItem);
			OnPropertyChanged(() => Items);
			OnCollectionChanged(ReferenceEquals(oldItem, newItem)
				? new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, newItem, index, index)
				: new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
		}

		public void Replace(TItem newItem)
		{
			if (newItem == null)
				throw new ArgumentNullException("newItem");

			var oldItem = this[GetKeyForItem(newItem)];
			Replace(oldItem, newItem);
		}

		/// <summary>
		///     Clears the current collection and replaces it with the specified collection.
		/// </summary>
		public void ReplaceAll(IEnumerable<TItem> collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			Clear();
			AddRange(collection);
		}

		#region INotifyCollectionChanged Members

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged()
		{
			PropertyChangedEventHandler changedEventHandler = PropertyChanged;
			if (changedEventHandler == null)
				return;
			var e = new PropertyChangedEventArgs(string.Empty);
			changedEventHandler(this, e);
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler changedEventHandler = PropertyChanged;
			if (changedEventHandler == null)
				return;
			var e = new PropertyChangedEventArgs(propertyName);
			changedEventHandler(this, e);
		}

		protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			if (propertyExpression == null)
				throw new ArgumentNullException("propertyExpression");
			if (PropertyChanged == null)
				return;
			var memberExpression = propertyExpression.Body as MemberExpression;
			if (memberExpression == null)
				throw new ArgumentNullException("propertyExpression");
			OnPropertyChanged(memberExpression.Member.Name);
		}

		#endregion
	}
}