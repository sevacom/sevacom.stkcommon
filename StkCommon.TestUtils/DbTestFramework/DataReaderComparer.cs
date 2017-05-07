using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace StkCommon.TestUtils.DbTestFramework
{
    public static class DataReaderComparer
    {
        private static readonly string ResultMessageTemplateFromExpected = "Строка:" + Environment.NewLine + "{0}" + Environment.NewLine + "Не найдена в результатах:{1}";
        private static readonly string ResultMessageTemplateFromDataReader = "Строка результата:" + Environment.NewLine + "{0}" + Environment.NewLine + "Не найдена в:{1}";
        private const string ValueSeparator = ", ";

        /// <summary>
        /// Сравнить данные из resultData, считанные по полям fields с ожидаемыми данными expectedData
        /// </summary>
        /// <param name="resultData">Источник данных, которые надо проверить</param>
        /// <param name="fields">Список полей в источнике, которые проверяем</param>
        /// <param name="expectedData">
        /// Ожидаемые данные (допускается, что порядок записей может отличаться)
        /// </param>
        /// <returns></returns>
        public static string Compare(this IDataReader resultData, string[] fields, IEnumerable<string[]> expectedData)
        {
            var expectedDataArray = expectedData as string[][] ?? expectedData.ToArray();
            if (expectedDataArray.Any(x => x.Length != fields.Length))
                throw new Exception(string.Format("Lengths of fields and one or more expectedData differ: {0} vs {1}",
                    fields.Length, string.Join(",", expectedDataArray.Select(x => x.Length).Distinct())));

            var result = FormatData(ExtractResultData(resultData, fields));
            var expected = FormatData(expectedDataArray);

            return ComapareInternal(result, expected);
        }

        private static string[][] FormatData(IEnumerable<string[]> extractResultData)
        {
            return extractResultData.Select(row => row.Select(FormatString).ToArray()).ToArray();
        }

        private static string FormatString(string s)
        {
            DateTime dt;
            return DateTime.TryParse(s, out dt)
                ? dt.ToString(CultureInfo.CurrentCulture)
                : s.ToLower();
        }

        /// <summary>
        /// Сравнение строк результата с ожидаемыми строками без учета порядка следования строк
        /// </summary>
        private static string ComapareInternal(IList<string[]> result, IList<string[]> expectedData)
        {
            var matches = new Dictionary<int, int>(); 
            // прямая проверка
            for (var i = 0; i < result.Count; i++)
            {
                var resultRow = result[i];
                var expectedRowsCompareResult = GetCompareResult(resultRow, expectedData);
                if (expectedRowsCompareResult.All(c => c.Value != null))
                {
                    return FormatExceptionMessage(resultRow, expectedData, expectedRowsCompareResult, true);
                }

                if (expectedRowsCompareResult.Any(c => c.Value == null && !matches.ContainsValue(c.Key)))
                {
                    var successMatchId = expectedRowsCompareResult.First(c => c.Value == null && !matches.ContainsValue(c.Key)).Key;

                    matches.Add(i, successMatchId);
                }
                else
                {
                    return FormatExceptionMessage(resultRow, expectedData, expectedRowsCompareResult, true);
                }
                
            }
            //обратная проверка. Поиск первого не найденного ожидаемого результата
            var notMatchedExpectedDataRow = expectedData.Where((_, i) => !matches.ContainsValue(i)).FirstOrDefault();

            if (notMatchedExpectedDataRow == null)
                return null; //все успешно сматчено

            var compareResult = GetCompareResult(notMatchedExpectedDataRow, result);
            return FormatExceptionMessage(notMatchedExpectedDataRow, result, compareResult, false);
        }

        /// <summary>
        /// Формирование текста исключения
        /// </summary>
        /// <param name="targetRow">строка поиск</param>
        /// <param name="expectedRows">набор, в котром производился поиск</param>
        /// <param name="compareResult">результат поиска</param>
        /// <param name="forwardDirection">
        /// признак направления поиска true - поиск строки из результата в ожидаемых строках false -
        /// поиск ожидаемой строки в результатах
        /// </param>
        /// <returns>текст исключения</returns>
        private static string FormatExceptionMessage(string[] targetRow, IList<string[]> expectedRows, IDictionary<int, ArrayDiff> compareResult, bool forwardDirection)
        {
            var result = forwardDirection
                ? ResultMessageTemplateFromDataReader
                : ResultMessageTemplateFromExpected;
            var row = string.Join(ValueSeparator, targetRow);
            var details = new StringBuilder();
            for (var i = 0; i < expectedRows.Count; i++)
            {
                if (!compareResult.ContainsKey(i)) continue;

                var detailRow = string.Join(ValueSeparator, expectedRows[i]);

                var pointerRow = compareResult[i] == null
                    ? "^Сопоставлена с другой строкой!^"
                    : CreatePointerRow(expectedRows[i], compareResult[i]);
                details
                    .AppendLine()
                    .AppendLine(detailRow)
                    .Append(pointerRow);
            }
            return string.Format(result, row, details);
        }

        private static string CreatePointerRow(ICollection<string> expectedRow, ArrayDiff arrayDiff)
        {
            const string leftCursor = "---^";
            const string rightCursor = "^---";

            var index = expectedRow
                .Take(Math.Min(expectedRow.Count, arrayDiff.Index))
                .Sum(s => s.Length + ValueSeparator.Length);
            index += arrayDiff.Offset;
            return index > leftCursor.Length
                ? "".PadLeft(index - leftCursor.Length + 1) + leftCursor
                : "".PadLeft(index) + rightCursor;
        }

        private static Dictionary<int, ArrayDiff> GetCompareResult(IList<string> resultRow, IList<string[]> expectedData)
        {
            var result = new Dictionary<int, ArrayDiff>();
            for (var i = 0; i < expectedData.Count; i++)
            {
                var rowCompareResult = Compare(resultRow, expectedData[i]);

                result.Add(i, rowCompareResult);
            }
            return result;
        }

        private static IEnumerable<string[]> ExtractResultData(IDataReader resultData, ICollection<string> fields)
        {
            var result = new List<string[]>();
            var fieldsCount = fields.Count;
            var remap = fields.Select((f, indx) => new { indx, dataIndex = resultData.GetOrdinal(f) })
                              .ToDictionary(r => r.dataIndex, r => r.indx);
            while (resultData.Read())
            {
                var row = new string[fieldsCount];
                for (var i = 0; i < resultData.FieldCount; i++)
                {
                    if (!remap.ContainsKey(i)) continue;
                    row[remap[i]] = resultData[i].NativeValueToDbString();
                }
                result.Add(row);
            }
            return result;
        }

        private static ArrayDiff Compare(IList<string> test, IList<string> expected)
        {
            if (test == null && expected == null) return null;
            if (test == null || expected == null)
                return new ArrayDiff(0, 0);
            var testDiffIndex = FindDiff(test, expected);
            if (testDiffIndex == null)
                return null;
            if (testDiffIndex.Value >= expected.Count)
                return new ArrayDiff(testDiffIndex.Value, 0);
            if (testDiffIndex.Value >= test.Count)
                return new ArrayDiff(testDiffIndex.Value, 0);
            var t = test[testDiffIndex.Value];
            var e = expected[testDiffIndex.Value];
            var offset = FindDiff(AsArray(t), AsArray(e));
            return new ArrayDiff(testDiffIndex.Value, offset ?? 0);
        }

        private static char[] AsArray(string s)
        {
            return s == null ? null : s.ToCharArray();
        }

        /// <summary>
        /// Сравнение списков значений
        /// </summary>
        /// <returns>
        /// Индекс первого отличающегося элемента в test (включая test.Count), null если списки одинаковы
        /// </returns>
        private static int? FindDiff<T>(IList<T> test, IList<T> expected)
        {
            if (test == null && expected == null) return null;
            if (test == null || expected == null) return 0;

            for (var i = 0; i < test.Count; i++)
            {
                if (i >= expected.Count)
                {
                    return i;
                }
                var t = test[i];
                var e = expected[i];
                if (!t.Equals(e))
                    return i;
            }
            if (expected.Count > test.Count)
                return test.Count;
            return null;
        }

        private class ArrayDiff
        {
            public ArrayDiff(int index, int offset)
            {
                Index = index;
                Offset = offset;
            }

            /// <summary>
            /// Индекс отличающейся колонки
            /// </summary>
            public int Index { get; private set; }

            /// <summary>
            /// Индекс первого отличающегося текстового символа внутри ячейки таблицы
            /// </summary>
            public int Offset { get; private set; }
        }
    }
}