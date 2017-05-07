using System;

namespace StkCommon.Data
{
	/// <summary>
	/// Проверка по ключу и поддержка NotifyPropertyChanged
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public abstract class EquatableByKey<T, TKey> : NotifyPropertyChangedBase, IEquatable<T>, IKeyModelObject<TKey>
		where T : class, IKeyModelObject<TKey>
		where TKey : IEquatable<TKey>
	{
	    protected EquatableByKey(TKey key)
	    {
            if(!typeof(TKey).IsValueType && ReferenceEquals(key, null))
                throw new ArgumentNullException(nameof(key));

	        Key = key;
	    }

        public TKey Key { get; }

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