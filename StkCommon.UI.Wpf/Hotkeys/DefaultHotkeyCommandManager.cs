using System;
using System.Windows;
using System.Windows.Input;
using StkCommon.Data.Diagnostics;
using StkCommon.UI.Wpf.Model;
#pragma warning disable 1591

namespace StkCommon.UI.Wpf.Hotkeys
{
	/// <summary>
	/// Менеджер обработки горячих клавиш через глобальный для приложения PreviewKeyDownEvent
	/// </summary>
	public class DefaultHotkeyCommandManager : HotkeyCommandManager
	{
		private readonly IDispatcher _dispatcher;
		private readonly IDiagnosticContainer _diagnosticContainer;

		public DefaultHotkeyCommandManager(IDispatcher dispatcher, IDiagnosticContainer diagnosticContainer)
		{
			if (dispatcher == null) throw new ArgumentNullException("dispatcher");
			if (diagnosticContainer == null) throw new ArgumentNullException("diagnosticContainer");
			_dispatcher = dispatcher;
			_diagnosticContainer = diagnosticContainer;

			//Глобальная регистрация обработчика всех нажатий во всех контролах
			EventManager.RegisterClassHandler(typeof(UIElement), UIElement.PreviewKeyDownEvent,
				new KeyEventHandler(KeyDownHandler));
		}

		/// <summary>
		/// Выполнение команды найденной при обработке через ProcessHotKey
		/// </summary>
		/// <param name="command"></param>
		protected override void ProcessHotkeyCommand(IHotkeyCommand command)
		{
			_diagnosticContainer.Logger.Debug(string.Format("Выполнение команды '{0}' (Id: {1})",
				command.CommandInfo.DisplayName, command.CommandInfo.Id));

			_diagnosticContainer.BeginSilentAndNotify(() => base.ProcessHotkeyCommand(command),
				string.Format("Ошибка при выполнении команды '{0}'.", command.CommandInfo.DisplayName));
		}

		private void KeyDownHandler(object sender, KeyEventArgs e)
		{
			var key = e.Key == Key.System ? e.SystemKey : e.Key;
			if (key == Key.LeftCtrl || key == Key.RightCtrl ||
			    key == Key.LeftShift || key == Key.RightShift ||
			    key == Key.LeftAlt || key == Key.RightAlt)
			{
				return;
			}

			if (!_dispatcher.CheckAccess())
				return;

			e.Handled = ProcessHotKey(new ShortCut(key, e.KeyboardDevice.Modifiers));
		}
	}
}