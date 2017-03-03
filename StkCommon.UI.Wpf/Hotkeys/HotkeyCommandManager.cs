using StkCommon.UI.Wpf.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace StkCommon.UI.Wpf.Hotkeys
{
	/// <summary>
	/// Менеджер обработки горячих клавиш
	/// </summary>
	public class HotkeyCommandManager : IHotkeyCommandManager
    {
        private readonly IDictionary<HotkeyCommandInfo, HotkeyCommand> _hotKeyCommands;
        private bool _isProcessing;
		private readonly IDictionary<string, ShortCut> _commandsData = new Dictionary<string, ShortCut>();

        public HotkeyCommandManager()
        {
            IsEnabledProcessHotKeys = true;
			_hotKeyCommands = new Dictionary<HotkeyCommandInfo, HotkeyCommand>();
        }

        public IEnumerable<IHotkeyCommand> HotKeyCommands
        {
            get { return _hotKeyCommands.Values; }
        }

        /// <summary>
        /// Признак обрабатывать сочетания горячих клавиш
        /// </summary>
        public bool IsEnabledProcessHotKeys { get; set; }

        public IHotkeyCommand RegisterCommand(HotkeyCommandInfo commandInfo)
        {
            return RegisterCommand(commandInfo, null);
        }

        public IHotkeyCommand RegisterCommand(HotkeyCommandInfo commandInfo, ICommand executeCommand)
        {
            var command = FindSharedEqualCommand(commandInfo);
            if (command != null)
                return command;

			command = FindSameCommand(commandInfo);
            if (command != null)
				throw new SameHotkeyCommandException(command.CommandInfo);

            command = new HotkeyCommand(commandInfo, commandInfo.DefaultHotkey, executeCommand);
            UpdateCommand(command);
			_hotKeyCommands.Add(command.CommandInfo, command);

            return command;
        }

        public bool UnRegisterCommand(HotkeyCommandInfo commandInfo)
        {
	        return _hotKeyCommands.Remove(commandInfo);
        }

        public void AttachCommand(HotkeyCommandInfo commandInfo, ICommand executeCommand, object commandArgument = null)
        {
            if (commandInfo == null) throw new ArgumentNullException("commandInfo");
            if (executeCommand == null) throw new ArgumentNullException("executeCommand");

			var command = _hotKeyCommands[commandInfo];
            if (command != null)
            {
                command.AttachCommand(executeCommand, commandArgument);
            }
        }

        public bool DetachCommand(HotkeyCommandInfo commandInfo, ICommand executeCommand = null)
        {
            if (commandInfo == null) throw new ArgumentNullException("commandInfo");

			var command = _hotKeyCommands[commandInfo];
            return command != null && command.DetachCommand(executeCommand);
        }
        /// <summary>
        /// Найти команду по Id
        /// </summary>
        /// <returns>null - если не найдена</returns>
        public IHotkeyCommand FindHotKeyCommandById(string commandId)
        {
            return _hotKeyCommands.Values.FirstOrDefault(p => p.CommandInfo.Id == commandId);
        }

		/// <summary>
		/// Найти команды по горячей клавише
		/// </summary>
		/// <param name="shortCut"></param>
		/// <returns></returns>
		public IEnumerable<IHotkeyCommand> FindCommandsByHotkey(ShortCut shortCut)
		{
			return _hotKeyCommands.Values.Where(cmd => cmd.HotKey.Equals(shortCut));
		}

        public bool ProcessHotKey(ShortCut shortCut)
        {
            if (_isProcessing || !IsEnabledProcessHotKeys)
                return false;

            _isProcessing = true;
            try
            {
                var commands = FindCommandsByHotkey(shortCut).ToArray();
                foreach (var hotKeyCommand in commands)
                {
					ProcessHotkeyCommand(hotKeyCommand);
                }

                return commands.Length > 0;
            }
            finally
            {
                _isProcessing = false;
            }
        }

        public void ApplyCommandData(IDictionary<string, ShortCut> commandsData)
        {
			if (commandsData == null) throw new ArgumentNullException("commandsData");

	        foreach (var data in commandsData)
	        {
		        _commandsData[data.Key] = data.Value;
	        }

            foreach (var command in _hotKeyCommands.Values)
            {
                UpdateCommand(command);
            }
        }

        public IDictionary<string, ShortCut> GetCommandData()
        {
	        return _hotKeyCommands.ToDictionary(k => k.Key.Id, v => v.Value.HotKey);
        }

		/// <summary>
		/// Выполнение команды найденной при обработке через ProcessHotKey
		/// </summary>
		/// <param name="command"></param>
		protected virtual void ProcessHotkeyCommand(IHotkeyCommand command)
		{
			command.RiseHotkeyCommand();
		}

        private void UpdateCommand(IHotkeyCommand command)
        {
	        ShortCut shortCut;
	        if (_commandsData.TryGetValue(command.CommandInfo.Id, out shortCut))
	        {
				command.HotKey = shortCut;
	        }
        }

		private HotkeyCommand FindSameCommand(HotkeyCommandInfo commandInfo)
        {
	        HotkeyCommand hotkeyCommand;
			if (_hotKeyCommands.TryGetValue(commandInfo, out hotkeyCommand))
				return hotkeyCommand;

			return _hotKeyCommands.Values.FirstOrDefault(cmd => 
				cmd.CommandInfo.EqualsByDefaultKey(commandInfo));
        }

		private HotkeyCommand FindSharedEqualCommand(HotkeyCommandInfo commandInfo)
		{
			HotkeyCommand command;
			if (!_hotKeyCommands.TryGetValue(commandInfo, out command))
				return null;

			if (commandInfo.IsShared
			    && command.CommandInfo.IsShared
			    && command.CommandInfo.EqualsByDefaultKey(commandInfo))
				return command;
			return null;
		}
    }

	public class SameHotkeyCommandException : ApplicationException
    {
        public SameHotkeyCommandException(HotkeyCommandInfo commandInfo)
        {
            HotkeyCommandInfo = commandInfo;
        }

        public HotkeyCommandInfo HotkeyCommandInfo { get; private set; }

        public override string Message
        {
            get { return "Найдена похожая команда: " + HotkeyCommandInfo; }
        }
    }
}