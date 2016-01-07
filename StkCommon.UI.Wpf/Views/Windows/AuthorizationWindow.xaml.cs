using System.ComponentModel;
using System.Windows;
using StkCommon.UI.Wpf.ViewModels;
using StkCommon.UI.Wpf.Views.Attached;

namespace StkCommon.UI.Wpf.Views.Windows
{
    /// <summary>
    /// Окно входа в систему с использованием логина и пароля
    /// </summary>
	public partial class AuthorizationWindow : BaseDialogWindow
    {
	    #region Constructor

	    public AuthorizationWindow()
	    {
		    InitializeComponent();
	    }

	    #endregion

	    #region Private methods

	    private void UserNameTextBoxDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
	    {
		    var authorizationViewModel = (AuthorizationViewModelBase) e.NewValue;
		    if (string.IsNullOrEmpty(authorizationViewModel.UserName))
			    UserNameTextBox.Focus();
		    else
			    PasswordTextBox.Focus();
	    }

	    private void OkButtonClick(object sender, RoutedEventArgs e)
	    {
		    UserNameTextBox.Focus();
	    }

	    private void AuthorizationWindowClosing(object sender, CancelEventArgs cancelEventArgs)
	    {
		    WindowProperties.SetDialogResult(this, DialogResult.HasValue && DialogResult.Value);
	    }

	    #endregion

    }
}
