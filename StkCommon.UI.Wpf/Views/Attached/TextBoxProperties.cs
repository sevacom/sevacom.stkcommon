using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StkCommon.UI.Wpf.Helpers;

namespace StkCommon.UI.Wpf.Views.Attached
{
	/// <summary>
	/// Attached property 'SelectAllTextWhenGotFocus' для TextBox, выделяет текст при получении фокуса  
	/// </summary>
    public static class TextBoxProperties
    {
        public static readonly DependencyProperty SelectAllTextWhenGotFocusProperty = DependencyProperty.RegisterAttached(
            "SelectAllTextWhenGotFocus", typeof(bool), typeof(TextBoxProperties), new PropertyMetadata(SelectAllTextWhenGotFocusPropertyChanged));

        public static void SetSelectAllTextWhenGotFocus(UIElement element, bool value)
        {
            element.SetValue(SelectAllTextWhenGotFocusProperty, value);
        }

        public static bool GetSelectAllTextWhenGotFocus(UIElement element)
        {
            return (bool)element.GetValue(SelectAllTextWhenGotFocusProperty);
        }

        private static void SelectAllTextWhenGotFocusPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            var selectAllTextWhenGotFocus = (bool)e.NewValue;
            if (selectAllTextWhenGotFocus)
            {
                textBox.GotFocus += TextBoxOnGotFocus;
                textBox.PreviewMouseLeftButtonDown += TextBoxOnPreviewMouseLeftButtonDown;
            }
        }

        private static void TextBoxOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
			var textBox = UiSearchHelper.FindAncestor<TextBox>((DependencyObject)e.OriginalSource);
            if (textBox == null)
                return;

            if (!textBox.IsKeyboardFocusWithin)
            {
                textBox.Focus();
                e.Handled = true;
            }
        }

        private static void TextBoxOnGotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            textBox.SelectAll();
        }
    }
}
