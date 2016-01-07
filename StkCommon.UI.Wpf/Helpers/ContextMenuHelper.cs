using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace StkCommon.UI.Wpf.Helpers
{
	public static class ContextMenuHelper
	{
		/// <summary>
		/// Показать контекстное меню, устанавливает DataContext такой же как у элемента
		/// </summary>
		/// <param name="element"></param>
		/// <param name="contextMenu"></param>
		/// <param name="beforeShow">Настройка контекстного меню перед показом</param>
		/// <param name="placementMode">Где показывать контекстное меню</param>
		public static bool ShowContextMenu(FrameworkElement element, ContextMenu contextMenu = null,
			Action<ContextMenu> beforeShow = null, PlacementMode placementMode = PlacementMode.Bottom)
		{
			if (element == null)
				return false;

			contextMenu = contextMenu ?? element.ContextMenu;

			if (contextMenu == null)
				return false;

			contextMenu.IsEnabled = true;
			contextMenu.Visibility = Visibility.Visible;
			contextMenu.PlacementTarget = element;
			contextMenu.Placement = placementMode;
			contextMenu.DataContext = null;
			contextMenu.DataContext = element.DataContext;
			if (beforeShow != null)
				beforeShow(contextMenu);
			contextMenu.IsOpen = true;
			return true;
		}

		public static ContextMenu GetContextMenuOrThorwException(this ResourceDictionary resources,
			string contextMenuResourceName)
		{
			var contextMenu = resources[contextMenuResourceName] as ContextMenu;
			if (contextMenu == null)
			{
				throw new ResourceReferenceKeyNotFoundException(
					"Не найдено контекстное меню '" + contextMenuResourceName + "' в ресурсах.", contextMenuResourceName);
			}
			return contextMenu;
		}
	}
}