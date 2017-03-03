using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace StkCommon.TestUtils.DbTestFramework
{
    public class TestDbResult
    {
        private readonly object[] _parameters;

        public TestDbResult(string[] fields, object[] parameters)
        {
            _parameters = parameters;
            Fields = fields;
            ExpectedValues = new List<string[]>();
        }

        /// <summary>
        /// Список полей <see cref="IDataReader"/>, для которых надо осуществлять проверку
        /// </summary>
        public string[] Fields { get; private set; }

        /// <summary>
        /// Строки со значениями ожидаемого результата, с которыми будем сверять считанные значения из БД. Порядок следования не учитывается!
        /// </summary>
        private List<string[]> ExpectedValues { get; set; }

        [Obsolete("Использовать метод AddRow")]
        public TestDbResult Row(params string[] values)
        {
            return AddRow(values);
        }

        /// <summary>
        /// Сформировать строку ожидаемых результатов из переданных значений и подстановочных параметров
        /// </summary>
        /// <param name="values">Список значений, включая шаблоны для подстановки </param>
        public TestDbResult AddRow(params string[] values)
        {
            ExpectedValues.Add(values.Select(Unquote).ToParametersRow(_parameters));
            return this;
        }

        private static string Unquote(string arg)
        {
            if (string.IsNullOrEmpty(arg))
                return arg;
            if (arg.StartsWith("'") && arg.EndsWith("'"))
                return arg.Substring(1, arg.Length - 2);
            return arg;
        }

        /// <summary>
        /// Проверить данные в dataReader, сравнив с ранее заданными ExpectedValues (порядок следования не учитывается).
        /// В случае отличия кидается исключение с описанием отличия.
        /// </summary>
        public void Verify(IDataReader dataReader)
        {
            var result = dataReader.Compare(Fields, ExpectedValues);
            if (string.IsNullOrEmpty(result)) return;
            throw new Exception(result);
        }
    }
}