using System;
using System.Collections.Generic;

namespace StkCommon.Data.Repositories
{
	/// <summary>
	/// Базовая реализация подписанта с накопление оповещений пока не выполнят StartHandleMessages
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	public class AsyncRepositorySubscriberBase<TMessage> : IAsyncRepositorySubscriber<TMessage>
	{
		private readonly Action<Action> _syncAction;
		private readonly Action<TMessage> _addAction;
		private readonly Action<TMessage> _updateAction;
		private readonly Action<TMessage> _deleteAction;
		private readonly List<ListChangeItem<TMessage>> _notifyMessagesCache = new List<ListChangeItem<TMessage>>();
		private bool _isStarted;
		private readonly object _syncLock = new object();

		public AsyncRepositorySubscriberBase(Action<Action> syncAction, Action<TMessage> addAction, 
			Action<TMessage> updateAction, Action<TMessage> deleteAction)
		{
			if (syncAction == null) throw new ArgumentNullException("syncAction");
			if (null == addAction) throw new ArgumentNullException("addAction");
			if (null == updateAction) throw new ArgumentNullException("updateAction");
			if (null == deleteAction) throw new ArgumentNullException("deleteAction");

			_syncAction = syncAction;
			_addAction = addAction;
			_updateAction = updateAction;
			_deleteAction = deleteAction;
		}

		/// <summary>
		/// Начать отправку оповещений через метод add, update, delete
		/// </summary>
		public void StartHandleMessages()
		{
			ListChangeItem<TMessage>[] notifyMessagesCopy = null;
			lock (_syncLock)
			{
				_isStarted = true;
				if (_notifyMessagesCache.Count > 0)
				{
					notifyMessagesCopy = _notifyMessagesCache.ToArray();
					_notifyMessagesCache.Clear();
				}
			}

			if (notifyMessagesCopy != null)
			{
				HandleAsyncMessageInternal(notifyMessagesCopy);
			}
		}

		public void HandleAsyncMessage(IEnumerable<ListChangeItem<TMessage>> messages)
		{
			lock (_syncLock)
			{
				if (!_isStarted)
				{
					_notifyMessagesCache.AddRange(messages);
					return;
				}
			}

			HandleAsyncMessageInternal(messages);
		}

		protected virtual void HandleAsyncMessageInternal(IEnumerable<ListChangeItem<TMessage>> messages)
		{
			_syncAction(() =>
			{
				foreach (var msg in messages)
				{
					if (msg.ListChangeType == ListChangeType.Delete)
						_deleteAction(msg.ChangedItem);
					else if (msg.ListChangeType == ListChangeType.Update)
						_updateAction(msg.ChangedItem);
					else if (msg.ListChangeType == ListChangeType.Add)
						_addAction(msg.ChangedItem);
				}
			});
		}
	}
}