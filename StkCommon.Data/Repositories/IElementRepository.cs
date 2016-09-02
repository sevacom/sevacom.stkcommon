using System.Collections.Generic;

namespace StkCommon.Data.Repositories
{
	/// <summary>
	/// Интерфейс репозитория, работающего на словаре
	/// </summary>
	/// <typeparam name="TValue">Значение</typeparam>
	/// <typeparam name="TKey">Ключ</typeparam>
	public interface IElementRepository<TValue, TKey> : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		/// <summary>
		/// Возвращает элемент по ключу
		/// </summary>
		/// <param name="key">Идентификатор элемента</param>
		/// <returns>Элемент коллекции</returns>
		TValue this[TKey key] { get; }

		/// <summary>
		/// Событие на изменение репозитория
		/// </summary>
		SimpleWeakEvent<IElementRepository<TValue, TKey>, IElementRepositoryChangedArg<TValue, TKey>> Changed { get; }

		/// <summary>
		/// Обьект синхронизации
		/// </summary>
		object SyncObject { get; }
	}
}
