using System.Windows;
using System.Windows.Controls;

namespace StkCommon.UI.Wpf.Views.Attached
{
    /// <summary>
    /// Класс добавляющий функциональность биндинга к полю Password
    /// объектов класса PasswordBox
    /// </summary>
    public static class PassworBoxHelper
    {
        #region Public properties

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached("Password",
			typeof(string), typeof(PassworBoxHelper),
			new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

		public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached("Attach",
            typeof(bool), typeof(PassworBoxHelper), new PropertyMetadata(false, Attach));

        #endregion

        #region Private properties

        private static readonly DependencyProperty IsUpdatingProperty =
		   DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),
           typeof(PassworBoxHelper));

        #endregion

        #region Public methods

        public static void SetAttach(DependencyObject dp, bool value)
		{
			dp.SetValue(AttachProperty, value);
		}

		public static bool GetAttach(DependencyObject dp)
		{
			return (bool)dp.GetValue(AttachProperty);
		}

		public static string GetPassword(DependencyObject dp)
		{
			return (string)dp.GetValue(PasswordProperty);
		}

		public static void SetPassword(DependencyObject dp, string value)
		{
			dp.SetValue(PasswordProperty, value);
		}

        #endregion

        #region Private methods

        private static bool GetIsUpdating(DependencyObject dp)
		{
			return (bool)dp.GetValue(IsUpdatingProperty);
		}

		private static void SetIsUpdating(DependencyObject dp, bool value)
		{
			dp.SetValue(IsUpdatingProperty, value);
		}

		private static void OnPasswordPropertyChanged(DependencyObject sender,
			DependencyPropertyChangedEventArgs e)
		{
			var passwordBox = sender as PasswordBox;
			if (passwordBox == null)
				return;
			passwordBox.PasswordChanged -= PasswordChanged;

			if (!GetIsUpdating(passwordBox))
			{
				passwordBox.Password = (string)e.NewValue;
			}
			passwordBox.PasswordChanged += PasswordChanged;
		}

		private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var passwordBox = sender as PasswordBox;

			if (passwordBox == null)
				return;

			if ((bool)e.OldValue)
			{
				passwordBox.PasswordChanged -= PasswordChanged;
			}

			if ((bool)e.NewValue)
			{
				passwordBox.PasswordChanged += PasswordChanged;
			}
		}

		private static void PasswordChanged(object sender, RoutedEventArgs e)
		{
			var passwordBox = sender as PasswordBox;
			if (passwordBox == null)
				return;
			SetIsUpdating(passwordBox, true);
			SetPassword(passwordBox, passwordBox.Password);
			SetIsUpdating(passwordBox, false);
        }

        #endregion
    }
}