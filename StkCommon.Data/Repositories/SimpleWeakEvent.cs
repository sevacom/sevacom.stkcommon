using System;
using System.Collections.Generic;

namespace StkCommon.Data.Repositories
{
	/// <summary>
	/// Простая реализация событий через легкие ссылки
	/// </summary>
	/// <typeparam name="TA">Тип аргумента, передаваемого при вызове события</typeparam>
	/// <typeparam name="TO">Тип владельца события</typeparam>
	public class SimpleWeakEvent<TO, TA>
	{
		private readonly Action _addHandler;
		private List<WeakReference> _targets = new List<WeakReference>();
		private List<Action<object, TO, TA>> _handlers = new List<Action<object, TO, TA>>();

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="raiseStarter">Только через это и можно "стартовать" события</param>
		/// <param name="addHandler">Дополнительный обработчик добавления нового подписчика</param>
		public SimpleWeakEvent(out Action<TO, TA> raiseStarter, Action addHandler = null)
		{
			_addHandler = addHandler;
			raiseStarter = Raise;
		}

		/// <summary>
		/// Добавить обработчик
		/// </summary>
		/// <typeparam name="T">Тип объекта обработчика</typeparam>
		/// <param name="target">Объект, в котором находится обработчик</param>
		/// <param name="handler">Обработчик события</param>
		public void Add<T>(T target, Action<T, TO, TA> handler)
		{
			_targets.Add(new WeakReference(target));
			_handlers.Add((o, sender, args) => handler((T)o, sender, args));
			if (null != _addHandler)
			{
				_addHandler();
			}

		}

		/// <summary>
		/// Удалить все подписки на событие для заданного объекта
		/// </summary>
		/// <typeparam name="T">Тип объекта обработчика</typeparam>
		/// <param name="target">Объект, в котором находится обработчик</param>
		public void Remove<T>(T target)
		{
			for (int i = _targets.Count - 1; i >= 0; i--)
			{
				if (!_targets[i].IsAlive || _targets[i].Target.Equals(target))
				{
					_targets.RemoveAt(i);
					_handlers.RemoveAt(i);
				}
			}
		}

		private void Raise(TO sender, TA args)
		{
			for (int i = _targets.Count - 1; i >= 0; i--)
			{
				if (!_targets[i].IsAlive)
				{
					_targets.RemoveAt(i);
					_handlers.RemoveAt(i);
					continue;
				}
				_handlers[i](_targets[i].Target, sender, args);
			}
		}
	}
}