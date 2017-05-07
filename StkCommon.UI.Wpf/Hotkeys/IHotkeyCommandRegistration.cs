using System.Collections.Generic;
using System.Windows.Input;

namespace StkCommon.UI.Wpf.Hotkeys
{

    /// <summary>
    /// Интерфейс для регистрации и привязки команд
    /// </summary>
    public interface IHotkeyCommandRegistration
    {
        /// <summary>
        /// Список всех команд
        /// </summary>
        IEnumerable<IHotkeyCommand> HotKeyCommands { get; }

	    /// <summary>
	    /// Зарегистрировать описание команды
	    /// </summary>
	    /// <param name="commandInfo"></param>
	    IHotkeyCommand RegisterCommand(HotkeyCommandInfo commandInfo);

	    /// <summary>
	    /// Зарегистрировать описание команды
	    /// </summary>
	    /// <param name="commandInfo"></param>
	    /// <param name="command">Сама команда</param>
	    IHotkeyCommand RegisterCommand(HotkeyCommandInfo commandInfo, ICommand command);

	    /// <summary>
	    /// Разрегистрировать описание команды
	    /// </summary>
	    /// <param name="commandInfo"></param>
	    /// <returns>true - найдена, false - не найдена</returns>
	    bool UnRegisterCommand(HotkeyCommandInfo commandInfo);

        /// <summary>
        /// Прикрепить обработчик к команде
        /// </summary>
        /// <param name="commandInfo"></param>
        /// <param name="executeCommand"></param>
        /// <param name="commandArgument">Аргумент который будет подаватся команде</param>
        void AttachCommand(HotkeyCommandInfo commandInfo, ICommand executeCommand, object commandArgument = null);

        /// <summary>
        /// Отвязать обработчик от команды
        /// </summary>
        /// <param name="commandInfo"></param>
        /// <param name="executeCommand">Если null то отвязать все команды</param>
        /// <returns></returns>
        bool DetachCommand(HotkeyCommandInfo commandInfo, ICommand executeCommand = null);
    }
}