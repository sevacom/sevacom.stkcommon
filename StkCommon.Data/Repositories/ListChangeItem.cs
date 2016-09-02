namespace StkCommon.Data.Repositories
{
	/// <summary>
	/// Информация об изменении списка элементов.
	/// </summary>
	public class ListChangeItem<T>
	{
		/// <summary>
		/// Элемент изменения.
		/// </summary>
		public T ChangedItem { get; set; }

		/// <summary>
		/// Тип изменения.
		/// </summary>
		public ListChangeType ListChangeType { get; set; }
	}

	/// <summary>
	/// Тип изменения списка объектов.
	/// </summary>
	public enum ListChangeType
	{
		/// <summary>
		/// Добавить элемент в список.
		/// </summary>
		Add = 0,

		/// <summary>
		/// Обновить элемент списка.
		/// </summary>
		Update = 1,

		/// <summary>
		/// Удалить элемент из списка.
		/// </summary>
		Delete = 2
	}
}