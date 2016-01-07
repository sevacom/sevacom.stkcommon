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
		private int _maxCountRiseAddEvent = 10;

		/// <summary>
		/// Максимальное кол-во элементов при котором отправляется событие Add при AddRange
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

			var list = collection.ToList();
			var args = list.Count > MaxCountRiseAddEvent
							? new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset) :
							new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list);
		
			foreach (var i in list) 
				Items.Add(i);

			OnCollectionChanged(args);
			OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
		}

		/// <summary> 
		/// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T). 
		/// </summary> 
		public void RemoveRange(IEnumerable<T> collection)
		{
			if (collection == null) throw new ArgumentNullException("collection");
			
			CheckReentrancy();

			var list = collection.ToList();
			var args = list.Count > MaxCountRiseAddEvent
							? new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset) :
							new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list);

			foreach (var i in list) 
				Items.Remove(i);

			OnCollectionChanged(args);
			OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
		}
	}
}