using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace StkCommon.UI.Wpf.Repositories
{
	/// <summary>
	/// Интерфейс коллекции с настраиваемым CollectionView
	/// </summary>
	/// <typeparam name="T">Вьюмодель или ее интерфейс</typeparam>
	public interface IViewedObservableCollection<out T> : IReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        /// Представление которое будет использоваться для сортировки и группировки элементов
        /// </summary>
        ICollectionView CollectionView { get; }

        /// <summary>
        /// Сортирующая функция
        /// </summary>
	    Func<T, T, int> CustomSort { set; }

        /// <summary>
        /// Группировка
        /// </summary>
        GroupDescription GroupDescription { get; set; }

        /// <summary>
        /// Возвращает элементы входящие в конкретную группу, если есть группировка
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
	    IEnumerable<T> GetGroupItems(string groupName);
    }
}