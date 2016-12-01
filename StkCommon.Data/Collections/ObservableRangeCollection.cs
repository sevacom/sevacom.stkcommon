using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace StkCommon.Data.Collections
{
	/// <summary>
	/// ObservableCollection с поддержкой AddRange и RemoveRange
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ObservableRangeCollection<T> : ObservableCollection<T>
	{
		private int _maxCountRiseAddEvent;

		/// <summary>
		/// Максимальное кол-во элементов при котором НЕ отправляется событие Reset при большом кол-ве изменений
		/// Если 0 то Reset не отправляется никогда 
		/// </summary>
		public int MaxCountRiseAddEvent
		{
			get { return _maxCountRiseAddEvent; }
			set
			{
				if(value >= 0)
					_maxCountRiseAddEvent = value;
			}
		}

		/// <summary> 
		/// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class. 
		/// </summary> 
		public ObservableRangeCollection()
		{
		}

		/// <summary> 
		/// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class that contains elements copied from the specified collection. 
		/// </summary> 
		/// <param name="collection">collection: The collection from which the elements are copied.</param> 
		/// <exception cref="System.ArgumentNullException">The collection parameter cannot be null.</exception> 
		public ObservableRangeCollection(IEnumerable<T> collection)
			: base(collection) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Collections.ObjectModel.ObservableCollection`1"/> class that contains elements copied from the specified list.
		/// </summary>
		/// <param name="list">The list from which the elements are copied.</param><exception cref="T:System.ArgumentNullException">The <paramref name="list"/> parameter cannot be null.</exception>
		public ObservableRangeCollection(List<T> list)
			: base(list)
		{
		}

		/// <summary> 
		/// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
		/// </summary> 
		public void AddRange(IEnumerable<T> collection)
		{
			if (collection == null) throw new ArgumentNullException("collection");

			CheckReentrancy();

			AddRangeInternal(collection);
		}

		/// <summary> 
		/// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T). 
		/// </summary> 
		public void RemoveRange(IEnumerable<T> collection)
		{
			if (collection == null) throw new ArgumentNullException("collection");
			
			CheckReentrancy();

			var list = collection.ToList();
			var args = IsDramaticalChange(list.Count) ? 
				new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset) :
				new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list);

			foreach (var item in list) 
				Items.Remove(item);

			RiseNotifyCollectionChangedEventArgs(args);
		}

		/// <summary>
		///     Clears the current collection and replaces it with the specified item.
		/// </summary>
		public void Replace(T oldItem, T newItem)
		{
			if (newItem == null) throw new ArgumentNullException("newItem");
			if (oldItem == null) throw new ArgumentNullException("oldItem");
			
			CheckReentrancy();

			var index = Items.IndexOf(oldItem);

			if (index == -1)
				throw new ArgumentException("Попытка замены элемента не содержащегося в коллекции!", "oldItem");

			Items[index] = newItem;

			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, 
				newItem, oldItem, index);
			OnCollectionChanged(args);
		}

		/// <summary>
		///     Clears the current collection and replaces it with the specified collection.
		/// </summary>
		public void ReplaceAll(IEnumerable<T> collection)
		{
			if (collection == null) throw new ArgumentNullException("collection");

			CheckReentrancy();

			Items.Clear();
			AddRangeInternal(collection, true);
		}

		protected void RiseNotifyCollectionChangedEventArgs(NotifyCollectionChangedEventArgs args)
		{
			OnCollectionChanged(args);
			OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
		}

		protected virtual bool IsDramaticalChange(int changedCount)
		{
			if (changedCount <= 0)
				return false;
			return changedCount > MaxCountRiseAddEvent;
		}

		private void AddRangeInternal(IEnumerable<T> collection, bool isReset = false)
		{
			var list = collection.ToList();
			var args = isReset || IsDramaticalChange(list.Count) ?
				new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset) :
				new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list);

			foreach (var item in list)
				Items.Add(item);

			RiseNotifyCollectionChangedEventArgs(args);
		}
	}
}