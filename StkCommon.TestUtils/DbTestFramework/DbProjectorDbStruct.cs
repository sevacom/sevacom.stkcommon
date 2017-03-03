using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StkCommon.TestUtils.DbTestFramework
{
    /// <summary>
    /// Интерфейс получения исходного текста декларирования SQL-объектов 
    /// </summary>
	public interface IExternalDbStruct
	{
        /// <summary>
        /// Получить текст декларирования пользовательского типа данных
        /// </summary>
		string GetUdtBody(string udtName);

        /// <summary>
        /// Получить текст создания хранимой процедуры 
        /// </summary>
		string GetSpBody(string spName);

        /// <summary>
        /// Получить тип данных колонки таблицы
        /// </summary>
		string GetTableColumnType(string tableName, string fieldName);

        /// <summary>
        /// Получить тип данных колонки таблицы
        /// </summary>
		IEnumerable<TestDbTableConstraint> GetTableConstraints(string tableName);
	}

	public class DbProjectorDbStruct : IExternalDbStruct
	{
		private readonly string _dbpPath;
		private const string XmlPartSpliter = "//<!-----------------------------------!>";
		private const string UdtDirectory = "UDT";
		private const string ProcedureDirectory = "PROCEDURE";
		private const string FunctionDirectory = "FUNCTION";
		private const string TableDirectory = "TABLE";

		public DbProjectorDbStruct(string dbpPath)
		{
			if (dbpPath == null) throw new ArgumentNullException("dbpPath");
			if (!Directory.Exists(dbpPath)) throw new DirectoryNotFoundException("Каталог с проектом DbProject не найден: " + dbpPath);

			_dbpPath = dbpPath;
		}

		public string GetUdtBody(string udtName)
		{
			if (udtName == null) throw new ArgumentNullException("udtName");
			return GetBody(GetFileName(udtName, UdtDirectory));
		}

		public string GetSpBody(string spName)
		{
			if (spName == null) throw new ArgumentNullException("spName");
			return GetBody(GetFileName(spName, ProcedureDirectory, FunctionDirectory));
		}

		public string GetTableColumnType(string tableName, string fieldName)
		{
			if (tableName == null) throw new ArgumentNullException("tableName");
			if (fieldName == null) throw new ArgumentNullException("fieldName");

			var fileName = GetFileName(tableName, TableDirectory);
			if (!fieldName.StartsWith("[") && !fieldName.EndsWith("]")) fieldName = "[" + fieldName + "]";

			string text;
			using (var sr = new StreamReader(fileName))
			{
				text = sr.ReadToEnd();
			}
			var index = text.IndexOf(@"(", StringComparison.Ordinal);
			text = text.Substring(index + 1);
			index = text.IndexOf("GO", StringComparison.Ordinal);
			text = text.Substring(0, index);
			index = text.LastIndexOf(")", StringComparison.Ordinal);
			text = text.Substring(0, index);
			foreach (var fullColumn in text.Split(','))
			{
				var parts = fullColumn.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (parts[0] == fieldName)
				{
					return parts[1];
				}
			}
			throw new Exception(string.Format("Тип для колонки {0} не найден", tableName + "." + fieldName));
		}

		public IEnumerable<TestDbTableConstraint> GetTableConstraints(string tableName)
		{
			var fileName = GetFileName(tableName, TableDirectory);

			var rawText = File.ReadAllText(fileName);
			var splitIndex = rawText.IndexOf(XmlPartSpliter, StringComparison.CurrentCultureIgnoreCase);
			var xmlStrings = rawText.Substring(splitIndex)
									.Split(new[] { XmlPartSpliter }, StringSplitOptions.RemoveEmptyEntries)
									.Select(x => x.Trim());
			return xmlStrings.Select(x => new DbProjectTestDbTableConstraint(x)).ToList();
		}

		private static string GetBody(string fileName)
		{
			string text;
			using (var sr = new StreamReader(fileName))
			{
				text = sr.ReadToEnd();
			}
			var index = text.IndexOf(XmlPartSpliter, StringComparison.Ordinal);
			text = text.Substring(0, index);
			index = text.LastIndexOf("GO", StringComparison.Ordinal);
			return text.Substring(0, index);
		}

		private string GetFileName(string name, params string[] types)
		{
			var fileName = types
				.Select(t => Directory.GetFiles(Path.Combine(_dbpPath, t), string.Format("*{0}].b1", name)).SingleOrDefault())
				.FirstOrDefault(t => null != t);
			if (null == fileName) throw new Exception(string.Format("Файл с исходным кодом для '{0}' не найден", name));
			return fileName;
		}
	}
}