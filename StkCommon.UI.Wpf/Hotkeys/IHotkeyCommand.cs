using StkCommon.UI.Wpf.Model;

namespace StkCommon.UI.Wpf.Hotkeys
{
    /// <summary>
    /// Объект команды с описанием, горячей клавишей, и исполняемым обработчиков
    /// </summary>
    public interface IHotkeyCommand
    {
        /// <summary>
        /// Информация о команде
        /// </summary>
        HotkeyCommandInfo CommandInfo { get; }

        /// <summary>
        /// Сочетание клавишь, при котором выполняется команда 
        /// </summary>
        /// <remarks>НЕ исользуйте setter! Он будет удален в следующей версии!</remarks>
        ShortCut HotKey { get; set; }

        /// <summary>
        /// Принудительно вызвать срабатывание команды
        /// </summary>
        /// <param name="param"></param>
        void RiseHotkeyCommand(object param = null);
 
    }
}