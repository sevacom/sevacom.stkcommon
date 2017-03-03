using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using BLToolkit.DataAccess;

namespace DbTestFramework.DemoApp.Model
{
	/// <summary>
	/// Фабрика дата-аксессоров.
	/// 
	/// </summary>
	public sealed class DataAccessorFactory
	{
		private string ConnectionString { get; set; }

		/// <summary>
		/// для случая, когда строка подключения настраивется под пользователя
		/// </summary>
		/// <param name="connectionString"/>
		public DataAccessorFactory(string connectionString)
		{
			ConnectionString = connectionString;
		}

		/// <summary>
		/// Получить аксесор к базе данных
		/// 
		/// </summary>
		public IStkTestDbAccessor GetAccessor()
		{
			return DataAccessor.CreateInstance<StkTestDbAccessor>(CreateDbManager(ConnectionString));
		}

		/// <summary>
		/// Создание кастомго менеджера, чтобы работать с таймаутами
		/// </summary>
		private static DbManager CreateDbManager(string connectionString)
		{
			return new DbManager(new SqlDataProvider(), connectionString);
		}
	}
}