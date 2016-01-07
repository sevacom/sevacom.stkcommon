namespace StkCommon.Data.Repositories
{
	/// <summary>
	/// Измененная еденица модели для оповещения
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TId"></typeparam>
	public class ChangedElement<T, TId>
	{
		/// <summary>
		/// Модель
		/// </summary>
		public T Value { get; set; }

		/// <summary>
		/// Старое состояние модели
		/// </summary>
		public T OldValue { get; set; }

		/// <summary>
		/// ИД модели (при удалении только он и есть)
		/// </summary>
		public TId ValueId { get; set; }

		/// <summary>
		/// Тип изменения модели
		/// </summary>
		public ChangedElementType ChangedType { get; set; }
	}
}