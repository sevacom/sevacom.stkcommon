namespace StkCommon.Data
{
	/// <summary>
	/// Базовый интерфейс для объектов с ключом
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	public interface IKeyModelObject<out TKey>
	{
		TKey Key { get; }
	}
}