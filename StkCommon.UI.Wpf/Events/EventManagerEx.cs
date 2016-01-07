using System;
using System.Reflection;
using System.Windows;

namespace StkCommon.UI.Wpf.Events
{
	public static class EventManagerEx
	{
		#region Methods

		/// <summary>
		/// add a event handler (Delegate) to a dependency object
		/// </summary>
		/// <param name="target">a dependency object</param>
		/// <param name="parameters">the params of the method UIElement.AddHandler (RoutedEvent, Delegate)</param>
		public static void AddHandlerToDependencyObject(object target, params object[] parameters)
		{
			if (target is UIElement || target is ContentElement)
				InvokeMethod(target, "AddHandler", parameters);
		}

		/// <summary>
		/// raise a routed event for the given target object
		/// </summary>
		/// <param name="target">the target object for the given routed event</param>
		/// <param name="re">a routed event that should be raised</param>
		public static void RaiseRoutedEvent(DependencyObject target, RoutedEvent re)
		{
			if (target is UIElement || target is ContentElement)
			{
				var rea = new RoutedEventArgs(re, target);
				InvokeMethod(target, "RaiseEvent", rea);
			}
		}

		/// <summary>
		/// remove a event handler (Delegate) from a dependency object
		/// </summary>
		/// <param name="target">a dependency object</param>
		/// <param name="parameters">the params of the method UIElement.RemoveHandler (RoutedEvent, Delegate)</param>
		public static void RemoveHandlerFromDependencyObject(object target, params object[] parameters)
		{
			if (target is UIElement || target is ContentElement)
			{
				InvokeMethod(target, "RemoveHandler", parameters);
			}
		}

		/// <summary>
		/// invoke a event for the given EventHandler
		/// </summary>
		/// <typeparam name="T">the subtype of a EventArgs</typeparam>
		/// <param name="handler">a event handler</param>
		/// <param name="target">the sender of this event</param>
		/// <param name="args">a event args</param>
		public static void RaiseEvent<T>(EventHandler<T> handler, object target, T args) where T : EventArgs
		{
			if (handler != null)
				handler(target, args);
		}

		/// <summary>
		/// get all handlers of the given RoutedEvent and UIElement
		/// WPF uses EventHandlerStore to manage the handlers of a RoutedEvent. But this class is marked as internal. man can't directly touch it out of the assembly.
		/// the only way to get the instance and use the methods of it is reflection
		/// </summary>
		/// <param name="eventSource">a source UIElement that have the handlers</param>
		/// <param name="re">a routed event</param>
		/// <returns>a array of RoutedEventHandlerInfo</returns>
		public static RoutedEventHandlerInfo[] GetHandlers(UIElement eventSource, RoutedEvent re)
		{
			var testWindowType = eventSource.GetType();

			var eventHandlersStoreType =
					testWindowType.GetProperty("EventHandlersStore",
					BindingFlags.Instance | BindingFlags.NonPublic);

			// Get the actual "value" of the store, not just the reflected PropertyInfo
			var eventHandlersStore = eventHandlersStoreType.GetValue(eventSource, null);

			// Get the store's type ...
			var storeType = eventHandlersStore.GetType();

			var handlers = storeType.InvokeMember("GetRoutedEventHandlers", BindingFlags.InvokeMethod, null, eventHandlersStore, new object[] { re }) as RoutedEventHandlerInfo[];
			return handlers;
		}

		/// <summary>
		/// invokes a method of the given target object with some parameters
		/// </summary>
		/// <param name="target">a object whose method should be called</param>
		/// <param name="methodName">the name of a method that should be called</param>
		/// <param name="parameters">the parameters that be used by calling the target method</param>
		internal static void InvokeMethod(object target, string methodName, params object[] parameters)
		{
			target.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, target, parameters);
		}

		#endregion
	}
}
