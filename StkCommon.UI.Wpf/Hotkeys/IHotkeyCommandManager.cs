using StkCommon.UI.Wpf.Model;
using System.Collections.Generic;

namespace StkCommon.UI.Wpf.Hotkeys
{
    /// <summary>
    /// Интерфес для управления командами горячих клавишь
    /// </summary>
    public interface IHotkeyCommandManager : IHotkeyCommandRegistration
    {
        /// <summary>
        /// Признак обрабатывать сочетания горячих клавиш
        /// </summary>
        bool IsEnabledProcessHotKeys { get; set; }

        /// <summary>
        /// Обработать сочетание клавиш
        /// </summary>
        /// <param name="shortCut"></param>
        /// <returns>true - обработана, false - ни одна команда не найдена</returns>
        bool ProcessHotKey(ShortCut shortCut);

        /// <summary>
        /// Найти команду по Id
        /// </summary>
        /// <returns>null - если не найдена</returns>
        IHotkeyCommand FindHotKeyCommandById(string commandId);

	    /// <summary>
	    /// Установить настройки горячих клавиш
	    /// </summary>
		void ApplyCommandData(IDictionary<string, ShortCut> commandsData);

        /// <summary>
        /// получить настройки
        /// </summary>
        /// <returns></returns>
		IDictionary<string, ShortCut> GetCommandData();

	    /// <summary>
	    /// Найти команды по горячей клавише
	    /// </summary>
	    /// <param name="shortCut"></param>
	    /// <returns></returns>
	    IEnumerable<IHotkeyCommand> FindCommandsByHotkey(ShortCut shortCut);
    }
}