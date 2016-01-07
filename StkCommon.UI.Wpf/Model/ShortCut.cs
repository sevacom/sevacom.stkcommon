using System;
using System.Windows.Input;

namespace StkCommon.UI.Wpf.Model
{
	/// <summary>
	/// Сочетание клавиш
	/// </summary>
	[Serializable]
	public struct ShortCut : IEquatable<ShortCut>
	{
		public static readonly ShortCut None = new ShortCut(Key.None, ModifierKeys.None);

		public ShortCut(Key key)
			: this()
		{
			Key = key;
			Modifier = ModifierKeys.None;
		}

		public ShortCut(Key key, ModifierKeys modifier)
			: this()
		{
			Key = key;
			Modifier = modifier;
		}

		public ShortCut(int key, int modifier)
			: this()
		{
			Key = (Key)Enum.ToObject(typeof(Key), key);
			Modifier = (ModifierKeys)Enum.ToObject(typeof(ModifierKeys), modifier);
		}

		public Key Key { get; set; }

		public ModifierKeys Modifier { get; set; }

		public bool Equals(ShortCut other)
		{
			return Key == other.Key && Modifier == other.Modifier;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is ShortCut && Equals((ShortCut)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((int)Key * 397) ^ (int)Modifier;
			}
		}
	}
}