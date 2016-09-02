using System;

namespace StkCommon.Data
{
	/// <summary>
	/// Проверка по ключу и поддержка NotifyPropertyChanged
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public abstract class EqutableByKey<T, TKey> : NotifyPropertyChangedBase, IEquatable<T>, IKeyModelObject<TKey>
		where T : class, IKeyModelObject<TKey>
		where TKey : struct, IEquatable<TKey>
	{
		public TKey Key { get; set; }

		public override bool Equals(object obj)
		{
			if (!(obj is T))
				return false;
			return Equals((T)obj);
		}

		public override int GetHashCode()
		{
			return Key.GetHashCode();
		}

		public bool Equals(T other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;

			return Key.Equals(other.Key);
		}
	}
}