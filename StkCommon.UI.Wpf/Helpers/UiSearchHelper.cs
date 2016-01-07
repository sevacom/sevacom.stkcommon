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
