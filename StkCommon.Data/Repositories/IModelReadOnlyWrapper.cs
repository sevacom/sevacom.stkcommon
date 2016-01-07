namespace StkCommon.Data.Repositories
{
	/// <summary>
	/// Интерфейс для вьюмоделей, являющихся элементами репозитариев вью моделей
	/// </summary>
	/// <typeparam name="TM">Тип модели</typeparam>
	/// <typeparam name="TId">Тип идентификатора</typeparam>
	public interface IModelReadOnlyWrapper<in TM, out TId>
	{
		/// <summary>
		/// Заполнение состояния вьюмодели на основе модели
		/// </summary>
		/// <param name="model"></param>
		void Parse(TM model);

        /// <summary>
        /// Возвращает идентификатор модели
        /// </summary>
        TId ModelId { get; }
	}
}