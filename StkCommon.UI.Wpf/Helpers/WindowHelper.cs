using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace StkCommon.UI.Wpf.Helpers
{
	/// <summary>
	/// Вспомогательные методы для работы с Window
	/// </summary>
	public static class WindowHelper
	{
		private const int HideMinimizeMaximizeStyle = -16;
		private const int WsMaximizebox = 0x10000;
		private const int WsMinimizebox = 0x20000;
		
		private const int RemoveIconstyle = -20;
		private const int WsExDlgmodalframe = 0x0001;
		private const int SwpNosize = 0x0001;
		private const int SwpNomove = 0x0002;
		private const int SwpNozorder = 0x0004;
		private const int SwpFramechanged = 0x0020;
		private const int WmSetIcon = 0x0080;
		private const int IconSmall = 0;
		private const int IconBig = 1;

		[DllImport("user32.dll")]
		extern private static int GetWindowLong(IntPtr hwnd, int index);

		[DllImport("user32.dll")]
		extern private static int SetWindowLong(IntPtr hwnd, int index, int value);

		[DllImport("user32.dll")]
		extern private static bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter,
				   int x, int y, int width, int height, uint flags);

		[DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		/// <summary>
		/// Скрыть кнопки минимизации и максимизации для окна, у которого стоит режим Resizable
		/// </summary>
		public static void HideMinimizeAndMaximizeButtons(Window window)
		{
			var hwnd = new WindowInteropHelper(window).Handle;
			var currentStyle = GetWindowLong(hwnd, HideMinimizeMaximizeStyle);

			SetWindowLong(hwnd, HideMinimizeMaximizeStyle, (currentStyle & ~WsMaximizebox & ~WsMinimizebox));
		}

		/// <summary>
		/// Скрыть иконку в заголовке у окна
		/// </summary>
		/// <param name="window"></param>
		public static void RemoveIcon(Window window)
		{
			// Get this window's handle
			var hwnd = new WindowInteropHelper(window).Handle;

			// Change the extended window style to not show a window icon
			int extendedStyle = GetWindowLong(hwnd, RemoveIconstyle);
			SetWindowLong(hwnd, RemoveIconstyle, extendedStyle | WsExDlgmodalframe);
			
			// reset the icon, both calls important
			SendMessage(hwnd, WmSetIcon, (IntPtr)IconSmall, IntPtr.Zero);
			SendMessage(hwnd, WmSetIcon, (IntPtr)IconBig, IntPtr.Zero);

			// Update the window's non-client area to reflect the changes
			SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SwpNomove |
				  SwpNosize | SwpNozorder | SwpFramechanged);
		}
	}
}
