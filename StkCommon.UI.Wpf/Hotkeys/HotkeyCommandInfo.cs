using StkCommon.UI.Wpf.Model;
using System;

namespace StkCommon.UI.Wpf.Hotkeys
{
    /// <summary>
    /// Информация о команде
    /// </summary>
    public class HotkeyCommandInfo : IEquatable<HotkeyCommandInfo>
    {
        public HotkeyCommandInfo(string uniqueId, string displayName,
                                string category = null,
                                string description = null,
                                ShortCut defaultHotkey = new ShortCut())
        {
            if (string.IsNullOrWhiteSpace(uniqueId))
                throw new ArgumentException("Уникальный идентификатор команды не может быть пустым");

            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("DisplayName не может быть пустым");

            Id = uniqueId.Trim();
            DisplayName = displayName;
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            Category = string.IsNullOrWhiteSpace(category) ? null : category.Trim();
            DefaultHotkey = defaultHotkey;
        }

        public HotkeyCommandInfo(string uniqueId, string displayName,
            string category,
            string description,
            ShortCut defaultHotkey,
            bool isShared)
            : this(uniqueId, displayName, category, description, defaultHotkey)
        {
            IsShared = isShared;
        }

        public string Id { get; private set; }

        public string DisplayName { get; set; }

        public string Description { get; private set; }

        public string Category { get; private set; }

        public ShortCut DefaultHotkey { get; private set; }

        /// <summary>
        /// Команда используется несколькими компонентами, при регистрации не проверяется что такая
        /// команда уже есть
        /// </summary>
        public bool IsShared { get; set; }

        public bool Equals(HotkeyCommandInfo other)
        {
            if (other == null)
                return false;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is HotkeyCommandInfo))
                return false;

            return Equals((HotkeyCommandInfo)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("ID: {3}, Название: {0}, Категория: {1}, DaefaultHotkey: {2}", DisplayName, Category, DefaultHotkey, Id);
        }

        public bool EqualsByDefaultKey(HotkeyCommandInfo command)
        {
            return !DefaultHotkey.Equals(ShortCut.None) && DefaultHotkey.Equals(command.DefaultHotkey);
        }
    }
}