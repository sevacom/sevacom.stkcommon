namespace StkCommon.Data.Repositories
{
	/// <summary>
	/// Аргумент для события изменения состава репозитория
	/// </summary>
	/// <typeparam name="TValue">Тип данных репозитория</typeparam>
	/// <typeparam name="TKey">Тип идентификатора репозитория</typeparam>
	public class ElementRepositoryChangedArg<TValue, TKey> : IElementRepositoryChangedArg<TValue, TKey>
	{
		/// <summary>
		/// Список элементов измененных в коллекции
		/// </summary>
		public ChangedElement<TValue, TKey>[] Elements { get; set; }
	}
}