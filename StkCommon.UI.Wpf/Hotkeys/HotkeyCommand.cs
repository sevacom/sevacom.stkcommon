using StkCommon.UI.Wpf.Commands;
using StkCommon.UI.Wpf.Model;
using StkCommon.UI.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
#pragma warning disable 1591

namespace StkCommon.UI.Wpf.Hotkeys
{
    /// <summary>
    /// Команда вместе с описанием, привязанной ICommand и сочетанием клавишь
    /// </summary>
	public class HotkeyCommand : IHotkeyCommand
    {
        private readonly List<ICommand> _attachedCommandList = new List<ICommand>();
	    private ShortCut _hotKey;

        public HotkeyCommand(HotkeyCommandInfo commandInfo, ShortCut hotKey, ICommand command = null)
        {
            CommandInfo = commandInfo;

            if (command != null)
                _attachedCommandList.Add(command);

	        _hotKey = hotKey;

            ApplyHotKeyToAttachedCommand(hotKey);
        }

        public HotkeyCommandInfo CommandInfo { get; private set; }

        public ShortCut HotKey
        {
            get { return _hotKey; }
            set
            {
	            _hotKey = value;
				ApplyHotKeyToAttachedCommand(_hotKey);
            }
        }

        /// <summary>
        /// Привязать команду которая будет вызываться при срабатывании
        /// </summary>
        /// <param name="command"></param>
        /// <param name="argument"></param>
        public void AttachCommand(ICommand command, object argument = null)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (_attachedCommandList.Contains(command)) throw new ArgumentException("command already attached");

            _attachedCommandList.Add(argument == null ? command : new CommandWithArgument(command, argument));
            ApplyHotKeyToAttachedCommand(HotKey);
        }

        /// <summary>
        /// Убарть привязанную команду
        /// </summary>
        /// <param name="command">Если NULL то отвязать все команды</param>
        /// <returns></returns>
        public bool DetachCommand(ICommand command)
        {
            if (command == null)
            {
                _attachedCommandList.Clear();
                return true;
            }

            return _attachedCommandList.Remove(command);
        }

        /// <summary>
        /// Принудительно вызвать срабатывание команды
        /// </summary>
        /// <param name="param"></param>
        public void RiseHotkeyCommand(object param = null)
        {
            foreach (var command in _attachedCommandList)
            {
                if (command.CanExecute(param))
                {
                    command.Execute(param);
                }
            }
        }

        private void ApplyHotKeyToAttachedCommand(ShortCut hotKey)
        {
            foreach (var command in _attachedCommandList)
            {
                var delegateCommand = command as IDelegateCommand;
                if (delegateCommand != null)
                    delegateCommand.HotKey = hotKey;
            }
        }
    }

    public class CommandWithArgument : ViewModelBase, IDelegateCommand
    {
        private readonly object _argument;
        private readonly IDelegateCommand _delegateCommand;
        private readonly ICommand _command;

        public CommandWithArgument(ICommand command, object argument)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            _command = command;
            _delegateCommand = command as IDelegateCommand;
            _argument = argument;
        }

        public string CommandName
        {
            get
            {
                return _delegateCommand != null ? _delegateCommand.CommandName : string.Empty;
            }
        }

        public ShortCut HotKey
        {
            get
            {
                return _delegateCommand != null ? _delegateCommand.HotKey : ShortCut.None;
            }
            set
            {
                if (_delegateCommand != null)
                    _delegateCommand.HotKey = value;
            }
        }

        public override event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (_delegateCommand != null)
                    _delegateCommand.PropertyChanged += value;
            }
            remove
            {
                if (_delegateCommand != null)
                    _delegateCommand.PropertyChanged -= value;
            }
        }

        public void RaiseCanExecuteChanged()
        {
            if (_delegateCommand != null)
                _delegateCommand.RaiseCanExecuteChanged();
        }

        public event EventHandler CanExecuteChanged
        {
            add { _command.CanExecuteChanged += value; }
            remove { _command.CanExecuteChanged -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _command.CanExecute(_argument);
        }

        public void Execute(object parameter)
        {
            _command.Execute(_argument);
        }
    }
}