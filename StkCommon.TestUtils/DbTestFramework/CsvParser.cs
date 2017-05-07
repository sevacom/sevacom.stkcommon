using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StkCommon.TestUtils.DbTestFramework
{
    public static class CsvParser
    {
        private const string DoubleQuotePlaceHolder = "97CBBB0D-997D-4DFE-8474-C1709E6B8308";
        private const char Quote = '\"';
        private const string QuoteStr = "\"";
        private const string DoubleQuote = "\"\"";
        private const char Semicolon = ';';

        public static string[][] Parse(string fileName, Encoding encoding = null)
        {
            var res = new List<string[]>();

            // Считывание CSV
            using (var sr = new StreamReader(fileName, encoding ?? Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = (sr.ReadLine() ?? string.Empty).Replace(DoubleQuote, DoubleQuotePlaceHolder);
                    var parts = new List<string>();
                    var currentStr = string.Empty;
                    var inQuote = false;
                    foreach (var c in line)
                    {
                        switch (c)
                        {
                            case Semicolon:
                                if (inQuote)
                                {
                                    currentStr += c;
                                }
                                else
                                {
                                    parts.Add(currentStr);
                                    currentStr = string.Empty;
                                }
                                break;

                            case Quote:
                                inQuote = !inQuote;
                                break;

                            default:
                                currentStr += c;
                                break;
                        }
                    }
                    parts.Add(currentStr);
                    res.Add(parts.Select(s => s.Replace(DoubleQuotePlaceHolder, QuoteStr)).ToArray());
                }
            }

            // Нормализация
            var isPrevZero = true;
            for (var i = res.Count - 1; i >= 0; i--)
            {
                var index = 0;
                var parts = res[i];
                for (var j = parts.Length - 1; j >= 0; j--)
                {
                    if (string.IsNullOrEmpty(parts[j])) continue;
                    index = j + 1;
                    break;
                }
                res[i] = parts.Take(index).ToArray();
                if (res[i].Length == 0)
                {
                    if (isPrevZero)
                    {
                        res.RemoveAt(i);
                    }
                    else
                    {
                        isPrevZero = true;
                    }
                }
                else
                {
                    isPrevZero = false;
                }
            }

            return res.ToArray();
        }
    }
}