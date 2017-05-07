using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace StkCommon.TestUtils.DbTestFramework
{
    public class TestDbFramework : IDisposable
    {
        public const string SectionNamePrefix = "#";
        public const string SetupUdtSection = "#SetupUdt";
        /// <summary>
        ///  Секция задания начальных значений одной или нескольких таблиц
        /// </summary>
        public const string SetupTableSection = "#SetupTable";
        /// <summary>
        /// Секция #ResultState позволяет описать состояние базы после выполения тестов
        /// </summary>
        public const string ResultStateSection = "#ResultState";

        [Obsolete("Следует использовать CommentsSection (#Comments)")]
        private const string ParametersSection = "#Parameters";
        /// <summary>
        /// Секция комментариев
        /// </summary>
        public const string CommentsSection = "#Comments";
        /// <summary>
        /// Секция описания набора(-ов) данных, которые ожидаются на выходе. Если наборов несколько их нужно разделять пробельной строкой
        /// </summary>
        public const string ResultQuerySection = "#ResultQuery";
        /// <summary>
        /// Секция задания списка процедур, которые нужно создать в базе. Каждая процедура на отдельной строке
        /// </summary>
        public const string SetupProcSection = "#SetupProc";

        public static readonly string[] FalseStrings =
		{
			string.Empty,
			"0",
			"false",
			"no"
		};

        private readonly string _connectionString;
        private readonly IExternalDbStruct _externalDbStruct;
        private TestDbSetup _setup;
        private readonly List<TestDbResult> _results = new List<TestDbResult>();

        private TestDbSetup _state;

        private object[] _parameters = new object[0];

        /// <param name="connectionString">Строка подклчения к БД</param>
        /// <param name="externalDbStruct">Механизм, предоставляющий текст декларирования SQL-объектов</param>
        public TestDbFramework(string connectionString, IExternalDbStruct externalDbStruct)
        {
            if (connectionString == null) throw new ArgumentNullException("connectionString");
            _connectionString = connectionString;
            _externalDbStruct = externalDbStruct;
        }

        /// <summary>
        /// Инициализация списка параметров, которые потом можно использовать в виде "{0};{1}" и т.п. в секциях #ResultQuery и #ResultState
        /// </summary>
        public TestDbFramework Parameters(params object[] parameters)
        {
            _parameters = parameters.Select(x => x.NativeValueToDbString()).Cast<object>().ToArray();
            return this;
        }
        
        /// <summary>
        /// Указание файла CSV для настройки теста
        /// </summary>
        /// <param name="fileName">Путь до CSV-файла теста</param>
        /// <param name="encoding">Кодировка файла теста</param>
        public void SetupFromFile(string fileName, Encoding encoding = null)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (!File.Exists(fileName)) throw new FileNotFoundException("Файл настроек не найден", fileName);

            var csv = CsvParser.Parse(fileName, encoding);
            var section = string.Empty;
            string tableName = null;
            TestDbResult result = null;
            string[] fields = null;
            foreach (var str in csv)
            {
                if (str.Length == 1 && str[0].StartsWith(SectionNamePrefix))
                {
                    section = str[0];
                    tableName = null;
                    result = null;
                    fields = null;
                    continue;
                }
                switch (section)
                {
                    case SetupUdtSection:
                        if (str.Length > 0)
                        {
                            if (null == _externalDbStruct) throw new NotSupportedException("Не задана зависимость IExternalDbStruct");
                            Setup().Udt(str[0], _externalDbStruct.GetUdtBody(str[0]));
                        }
                        break;

                    case SetupTableSection:
                    case ResultStateSection:
                        var isSetup = section == SetupTableSection;
                        var setup = isSetup ? Setup() : ResultState();
                        if (str.Length == 0)
                        {
                            tableName = null;
                            fields = null;
                        }
                        else if (tableName == null)
                        {
                            tableName = str[0];
                        }
                        else if (fields == null)
                        {
                            fields = str;
                            ResolveFieldTypes(tableName, fields);
                            setup.Table(tableName, fields);
                            LoadConstraints(setup, tableName);
                        }
                        else
                        {
                            string[] values;
                            if (!TryGetValues(str, fields, isSetup, out values))
                            {
                                continue;
                            }
                            setup.Row(values);
                        }
                        break;

                    case SetupProcSection:
                        if (str.Length > 0)
                        {
                            if (null == _externalDbStruct) throw new NotSupportedException("Не задана зависимость IExternalDbStruct");
                            Setup().Sp(str[0], _externalDbStruct.GetSpBody(str[0]));
                        }
                        break;

                    case ResultQuerySection:
                        if (str.Length == 0)
                        {
                            fields = null;
                        }
                        else if (result == null)
                        {
                            result = ResultQuery(str);
                        }
                        else
                        {
                            string[] values;
                            if (!TryGetValues(str, result.Fields, false, out values))
                            {
                                continue;
                            }
                            result.AddRow(values);
                        }
                        break;

                    case CommentsSection:
                    case ParametersSection:
                        break;

                    default:
                        throw new NotSupportedException("Неизвестная группа: " + string.Join(", ", str));
                }
            }
            Setup().Apply();
        }

        public TestDbSetup Setup()
        {
            return _setup ?? (_setup = new TestDbSetup(_connectionString, _parameters, true));
        }

        public TestDbResult ResultQuery(params string[] fields)
        {
            _results.Add(new TestDbResult(fields, _parameters));
            return _results.Last();
        }

        public TestDbSetup ResultState()
        {
            return _state ?? (_state = new TestDbSetup(_connectionString, _parameters, false));
        }

        public void VerifyResultQuery(IDataReader dataReader, bool dispose = true)
        {
            if (dataReader == null) throw new ArgumentNullException("dataReader");

            try
            {
                var i = 0;
                do
                {
                    try
                    {
                        _results[i].Verify(dataReader);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            string.Format("Получен неожиданный результат.{0}DataSet[{1}]{0}{2}", Environment.NewLine, i, e.Message), e);
                    }
                    i++;
                } while (dataReader.NextResult());
            }
            finally
            {
                if (dispose)
                {
                    dataReader.Dispose();
                }
            }
        }

        public void VerifyAll(IDataReader dataReader = null, bool dispose = true)
        {
            if (null != dataReader)
                VerifyResultQuery(dataReader, dispose);

            if (null != _state)
                ResultState().Verify();
        }

        public void Dispose()
        {
            if (_setup != null)
            {
                _setup.Dispose();
                _setup = null;
            }
            if (_state != null)
            {
                _state.Dispose();
                _state = null;
            }
        }

        private void LoadConstraints(TestDbSetup setup, string tableName)
        {
            setup.SetTableConstraints(tableName,
                _externalDbStruct.GetTableConstraints(tableName).Where(x => x.ConstraintType != TestDbConstraintType.Unknown));
        }

        private void ResolveFieldTypes(string tableName, IList<string> fields)
        {
            if (null == _externalDbStruct)
                return;

            for (var i = 0; i < fields.Count; i++)
            {
                var field = new TestDbField(fields[i]);
                if (!string.IsNullOrEmpty(field.Type))
                    continue;

                field.Type = _externalDbStruct.GetTableColumnType(tableName, field.Name);
                fields[i] = field.ToString();
            }
        }

        private bool TryGetValues(string[] sourceValues, IList<string> fields, bool isSetup, out string[] values)
        {
            var fieldsLength = fields.Count;
            values = Enumerable.Repeat("", fieldsLength).ToArray();
            Array.Copy(sourceValues, values, Math.Min(sourceValues.Length, fieldsLength));

            if (sourceValues.Length > fieldsLength)
            {
                var visibility = string.Format(sourceValues[fieldsLength], _parameters);
                if (IsFalseString(visibility))
                    return false;
            }
            for (var i = 0; i < fieldsLength; i++)
            {
                var currentValue = values[i] ?? "";
                if (!isSetup || currentValue.StartsWith("'") || currentValue.StartsWith("N'") || currentValue.ToLower() == "null")
                    continue;

                var fieldType = fields[i].GetFieldType();
                if (null == fieldType)
                    continue;

                if (fieldType == "uniqueidentifier" || fieldType.EndsWith("time") || fieldType.StartsWith("var") ||
                    fieldType.StartsWith("nvar"))
                {
                    values[i] = "'" + currentValue + "'";
                }
            }
            return true;
        }

        private static bool IsFalseString(string s)
        {
            if (s == null)
                return false;
            var lower = s.ToLower();
            return FalseStrings.Contains(lower);
        }
    }
}