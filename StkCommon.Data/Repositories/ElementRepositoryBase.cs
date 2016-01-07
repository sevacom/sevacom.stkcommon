using System;
using System.Collections;
using System.Collections.Generic;

namespace StkCommon.Data.Repositories
{
	/// <summary>
	/// Базовая реализация IElementRepository
	/// </summary>
	public abstract class ElementRepositoryBase<TValue, TKey> : IElementRepository<TValue, TKey>
	{
		private readonly SimpleWeakEvent<IElementRepository<TValue, TKey>, IElementRepositoryChangedArg<TValue, TKey>> _changed;
		private readonly object _syncObject;
		private Action<IElementRepository<TValue, TKey>, IElementRepositoryChangedArg<TValue, TKey>> _raiseStarter;
		protected IDictionary<TKey, TValue> Items;

		protected ElementRepositoryBase()
		{
			_syncObject = new object();
			_changed = new SimpleWeakEvent<IElementRepository<TValue, TKey>, IElementRepositoryChangedArg<TValue, TKey>>(out _raiseStarter);
		}

		public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			LoadDataIfNeed();
			return Items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public virtual TValue this[TKey key]
		{
			get
			{
				LoadDataIfNeed();
				return Items[key];
			}
		}

		public SimpleWeakEvent<IElementRepository<TValue, TKey>, IElementRepositoryChangedArg<TValue, TKey>> Changed
		{
			get { return _changed; }
		}

		public object SyncObject
		{
			get { return _syncObject; }
		}

		protected void RaiseElementsChanged(params ChangedElement<TValue, TKey>[] elements)
		{
			_raiseStarter(this, new ElementRepositoryChangedArg<TValue, TKey> { Elements = elements });
		}

		protected void RaiseElementChanged(ChangedElementType changedType, TValue value, TValue oldValue, TKey id)
		{
			RaiseElementsChanged(new ChangedElement<TValue, TKey> { ChangedType = changedType, Value = value, OldValue = oldValue, ValueId = id });
		}

		/// <summary>                                  
		/// Загрузка данных. Инициализация репозитория
		/// </summary>
		protected abstract void LoadDataIfNeed();
	}
}