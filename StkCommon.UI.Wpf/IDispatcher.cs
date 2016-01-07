using System;
using System.Windows.Threading;

namespace StkCommon.UI.Wpf
{
	/// <summary>
	/// Интерфейс диспатчера (необходим для тестируемости вьюмоделей и пр.)
	/// </summary>
	public interface IDispatcher
	{
		DispatcherOperation BeginInvoke(Delegate method, params Object[] args);
		DispatcherOperation BeginInvoke(Action action);
		DispatcherOperation BeginInvoke(DispatcherPriority priority, Action action);

		Object Invoke(Delegate method, params Object[] args);
		void Invoke(Action action);
		void Invoke(Action action, DispatcherPriority priority);

		/// <summary>
		/// Определяет, исполняется ли вызывающий поток в потоке Dispatcher. Если true, то вызывать Invoke и BeginInvoke нет смысла
		/// </summary>
		bool CheckAccess();
	}

	/// <summary>
	/// Адаптер  System.Windows.Threading.Dispatcher к IDispatcher
	/// </summary>
	public class DispatcherAdapter : IDispatcher
	{
		protected readonly Dispatcher Dispatcher;

		public DispatcherAdapter(Dispatcher dispatcher)
		{
			if (dispatcher == null) throw new ArgumentNullException("dispatcher");
			Dispatcher = dispatcher;
		}

		public DispatcherOperation BeginInvoke(Delegate method, params object[] args)
		{
			return Dispatcher.BeginInvoke(method, args);
		}

		public DispatcherOperation BeginInvoke(Action action)
		{
			return Dispatcher.BeginInvoke(action);
		}

		public DispatcherOperation BeginInvoke(DispatcherPriority priority, Action action)
		{
			return Dispatcher.BeginInvoke(priority, action);
		}

		public object Invoke(Delegate method, params object[] args)
		{
			return Dispatcher.Invoke(method, args);
		}

		public void Invoke(Action action)
		{
			Dispatcher.Invoke(action);
		}

		public void Invoke(Action action, DispatcherPriority priority)
		{
			Dispatcher.Invoke(action, priority);
		}

		/// <summary>
		/// Определяет, исполняется ли вызывающий поток в потоке Dispatcher. Если true, то вызывать Invoke и BeginInvoke нет смысла
		/// </summary>
		public bool CheckAccess()
		{
			return Dispatcher.CheckAccess();
		}
	}
}
