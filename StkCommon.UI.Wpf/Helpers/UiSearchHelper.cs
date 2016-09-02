using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StkCommon.UI.Wpf.Helpers
{
    /// <summary>
    /// Класс, содержащий набор методов для поиска элементов управления внутри дерева контролов
    /// </summary>
    public static class UiSearchHelper
    {
        /// <summary>
        /// Метод, осуществляющий поиск элемента управления по визуальному дереву 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static T FindAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            if (current == null)
                return null;

            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        /// <summary>
        /// Метод, позволяющий получить контекстное меню (родителя) по элементу этого меню
        /// </summary>
        /// <param name="menuItem"></param>
        /// <returns></returns>
        public static ContextMenu GetContextMenuByMenuItem(MenuItem menuItem)
        {
            if (menuItem == null)
                return null;

            while (true)
            {
                var contextMenu = menuItem.Parent as ContextMenu;
                if (contextMenu != null)
                    return contextMenu;

                return GetContextMenuByMenuItem((MenuItem)menuItem.Parent);
            }
        }

		/// <summary>
		/// Finds a Child of a given item in the visual tree. 
		/// </summary>
		/// <param name="parent">A direct parent of the queried item.</param>
		/// <typeparam name="TChildType">The type of the queried item.</typeparam>
		/// <param name="childName">x:Name or Name of child. </param>
		/// <returns>The first parent item that matches the submitted type parameter. 
		/// If not matching item can be found, 
		/// a null parent is being returned.</returns>
		public static TChildType FindChild<TChildType>(DependencyObject parent, string childName = null)
		   where TChildType : DependencyObject
		{
			// Confirm parent and childName are valid. 
			if (parent == null) return null;

			TChildType foundChild = null;

			int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < childrenCount; i++)
			{
				var child = VisualTreeHelper.GetChild(parent, i);
				// If the child is not of the request child type child

				if (!(child is TChildType))
				{
					// recursively drill down the tree
					foundChild = FindChild<TChildType>(child, childName);

					// If the child is found, break so we do not overwrite the found child. 
					if (foundChild != null) break;
				}
				else if (!string.IsNullOrEmpty(childName))
				{
					var frameworkElement = child as FrameworkElement;
					// If the child's name is set for search
					if (frameworkElement != null && frameworkElement.Name == childName)
					{
						// if the child's name is of the request name
						foundChild = (TChildType)child;
						break;
					}
				}
				else
				{
					// child element found.
					foundChild = (TChildType)child;
					break;
				}
			}

			return foundChild;
		}

		/// <summary>
		/// Analyzes both visual and logical tree in order to find all elements
		/// of a given type that are descendants of the <paramref name="source"/>
		/// item.
		/// </summary>
		/// <typeparam name="T">The type of the queried items.</typeparam>
		/// <param name="source">The root element that marks the source of the
		/// search. If the source is already of the requested type, it will not
		/// be included in the result.</param>
		/// <returns>All descendants of <paramref name="source"/> that match the
		/// requested type.</returns>
		public static IEnumerable<T> FindChildren<T>(DependencyObject source) where T : DependencyObject
		{
			if (source != null)
			{
				var childs = GetChildObjects(source);
				foreach (var child in childs)
				{
					//analyze if children match the requested type
					if (child is T)
					{
						yield return (T)child;
					}

					//recurse tree
					foreach (T descendant in FindChildren<T>(child))
					{
						yield return descendant;
					}
				}
			}
		}

		/// <summary>
		/// This method is an alternative to WPF's
		/// <see cref="VisualTreeHelper.GetChild"/> method, which also
		/// supports content elements. Do note, that for content elements,
		/// this method falls back to the logical tree of the element.
		/// </summary>
		/// <param name="parent">The item to be processed.</param>
		/// <returns>The submitted item's child elements, if available.</returns>
		public static IEnumerable<DependencyObject> GetChildObjects(DependencyObject parent)
		{
			if (parent == null) yield break;


			if (parent is ContentElement || parent is FrameworkElement)
			{
				//use the logical tree for content / framework elements
				foreach (object obj in LogicalTreeHelper.GetChildren(parent))
				{
					var depObj = obj as DependencyObject;
					if (depObj != null) yield return (DependencyObject)obj;
				}
			}
			else
			{
				//use the visual tree per default
				int count = VisualTreeHelper.GetChildrenCount(parent);
				for (int i = 0; i < count; i++)
				{
					yield return VisualTreeHelper.GetChild(parent, i);
				}
			}
		}

		/// <summary>
		/// Получить ScrollViewer у лемента или ближаешего child
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static DependencyObject GetScrollViewer(DependencyObject o)
		{
			if (o is ScrollViewer)
			{
				return o;
			}

			for (var i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
			{
				var child = VisualTreeHelper.GetChild(o, i);

				var result = GetScrollViewer(child);
				if (result != null)
				{
					return result;
				}
			}
			return null;
		}
    }
}
