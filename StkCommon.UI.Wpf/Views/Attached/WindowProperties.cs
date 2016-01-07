using System.Windows;

namespace StkCommon.UI.Wpf.Views.Attached
{
	/// <summary>
	/// Dependency Property для Window
	/// </summary>
	public static class WindowProperties
	{
		public static readonly DependencyProperty DialogResultProperty =
			DependencyProperty.RegisterAttached(
			"DialogResult",
			typeof(bool?),
			typeof(WindowProperties),
			new PropertyMetadata(DialogResultChanged));

		private static void DialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var window = d as Window;
			if (window != null)
				window.DialogResult = e.NewValue as bool?;
		}

		public static void SetDialogResult(Window target, bool? value)
		{
			if (target != null)
				target.SetValue(DialogResultProperty, value);
		}

		public static bool? GetDialogResult(Window target)
		{
			if (target == null)
				return null;
			return target.GetValue(DialogResultProperty) as bool?;
		}
	}
}
