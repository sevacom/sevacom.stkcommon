using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;
using StkCommon.Data.Collections;

namespace StkCommon.UI.Wpf.Model
{
	/// <summary>
	/// Переопределяет обработку подписчиков у <see cref="ObservableCollection{T}"/> и использует <see cref="DispatcherObject"/> если событие пришло из другого потока.
	/// 
	/// Украл тут:
	/// http://geekswithblogs.net/NewThingsILearned/archive/2008/01/16/have-worker-thread-update-observablecollection-that-is-bound-to-a.aspx
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	public class ThreadSafeObservableCollection<T> : ObservableRangeCollection<T>
	{
		public ThreadSafeObservableCollection()
		{
		}

		public ThreadSafeObservableCollection(IEnumerable<T> collection)
			: base(collection)
		{
		}

		public ThreadSafeObservableCollection(List<T> list)
			: base(list)
		{
		}

		// Override the event so this class can access it
		public override event NotifyCollectionChangedEventHandler CollectionChanged;

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			// Be nice - use BlockReentrancy like MSDN said
			using (BlockReentrancy())
			{
				var eventHandler = CollectionChanged; //-V3119
				if (eventHandler == null)
					return;

				var delegates = eventHandler.GetInvocationList();
				// Walk thru invocation list
				foreach (NotifyCollectionChangedEventHandler handler in delegates)
				{
					var dispatcherObject = handler.Target as DispatcherObject;
					// If the subscriber is a DispatcherObject and different thread
					if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
					{
						// Invoke handler in the target dispatcher's thread
						dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, e);
					}
					else // Execute handler as is
						handler(this, e);
				}
			}
		}
	}
}