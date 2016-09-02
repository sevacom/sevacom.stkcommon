using System;
using System.Collections.Generic;

namespace StkCommon.Data.Repositories
{
	/// <summary>
	/// Базовый интерфейс репозитариев
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IObservableRepository<T> : IDisposable
	{
		/// <summary>
		/// Получить коллекцию объектов и подписаться на оповещения
		/// </summary>
		/// <param name="subscriber"></param>
		/// <returns></returns>
		IEnumerable<T> GetCollection(IAsyncRepositorySubscriber<T> subscriber = null);

		/// <summary>
		/// Отписаться от оповещений
		/// </summary>
		/// <param name="subscriber"></param>
		void Unsubscribe(IAsyncRepositorySubscriber<T> subscriber);
	}

	/// <summary>
	/// Интерфейс подписчика на асинхронное сообщение.
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	public interface IAsyncRepositorySubscriber<TMessage>
	{
		/// <summary>
		/// Обработать ассинхронные сообщения
		/// </summary>
		/// <param name="messages"></param>
		void HandleAsyncMessage(IEnumerable<ListChangeItem<TMessage>> messages);
	}
}
