using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace StkCommon.UI.Wpf.Events
{
	/// <summary>
	///     This class contains methods for the CommandManager that help avoid memory leaks by
	///     using weak references.
	/// </summary>
	internal class WeakReferenceEventManager
	{
		internal static void CallWeakReferenceHandlers(List<WeakReference> handlers)
		{
			if (handlers != null)
			{
				// Take a snapshot of the handlers before we call out to them since the handlers
				// could cause the array to me modified while we are reading it.

				var callees = new EventHandler[handlers.Count];
				int count = 0;

				for (int i = handlers.Count - 1; i >= 0; i--)
				{
					var reference = handlers[i];
					var handler = reference.Target as EventHandler;
					if (handler == null)
					{
						// Clean up old handlers that have been collected
						handlers.RemoveAt(i);
					}
					else
					{
						callees[count] = handler;
						count++;
					}
				}

				// Call the handlers that we snapshotted
				for (var i = 0; i < count; i++)
				{
					var handler = callees[i];
					handler(null, EventArgs.Empty);
				}
			}
		}

		internal static void AddHandlersToRequerySuggested(IEnumerable<WeakReference> handlers)
		{
			if (handlers != null)
			{
				foreach (var handlerRef in handlers)
				{
					var handler = handlerRef.Target as EventHandler;
					if (handler != null)
					{
						CommandManager.RequerySuggested += handler;
					}
				}
			}
		}

		internal static void RemoveHandlersFromRequerySuggested(IEnumerable<WeakReference> handlers)
		{
			if (handlers != null)
			{
				foreach (WeakReference handlerRef in handlers)
				{
					var handler = handlerRef.Target as EventHandler;
					if (handler != null)
					{
						CommandManager.RequerySuggested -= handler;
					}
				}
			}
		}

		internal static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler)
		{
			AddWeakReferenceHandler(ref handlers, handler, -1);
		}

		internal static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler, int defaultListSize)
		{
			if (handlers == null)
			{
				handlers = (defaultListSize > 0 ? new List<WeakReference>(defaultListSize) : new List<WeakReference>());
			}

			handlers.Add(new WeakReference(handler));
		}

		internal static void RemoveWeakReferenceHandler(List<WeakReference> handlers, EventHandler handler)
		{
			if (handlers != null)
			{
				for (int i = handlers.Count - 1; i >= 0; i--)
				{
					var reference = handlers[i];
					var existingHandler = reference.Target as EventHandler;
					if ((existingHandler == null) || (existingHandler == handler))
					{
						// Clean up old handlers that have been collected
						// in addition to the handler that is to be removed.
						handlers.RemoveAt(i);
					}
				}
			}
		}
	}
}