using System;
using System.Diagnostics;

namespace StkCommon.TestUtils
{
    public static class ElapsedUtils
    {
        /// <summary>
        /// Навание операции, время
        /// </summary>
        /// <param name="elapsed"></param>
        /// <param name="operationName"></param>
        /// <param name="maxMillisec">если меньше то распечатывает милисекунды, если больше то TimeSpan.ToString()</param>
        public static void FormatOperationWithElapsedTime(TimeSpan elapsed, string operationName, int maxMillisec = 3000)
        {
            var elapsedStr = FormatElapsedTime(elapsed, maxMillisec);
            Console.WriteLine(@"Operation = {0}, Elapsed = {1}", operationName, elapsedStr);
        }

        /// <summary>
        /// Навание операции, время
        /// </summary>
        /// <param name="watch"></param>
        /// <param name="operationName"></param>
        /// <param name="maxMillisec">если меньше то распечатывает милисекунды, если больше то TimeSpan.ToString()</param>
        public static void FormatOperationWithElapsedTime(Stopwatch watch, string operationName, int maxMillisec = 3000)
        {
            FormatOperationWithElapsedTime(watch.Elapsed, operationName, maxMillisec);
        }

        private static string FormatElapsedTime(TimeSpan elapsed, int maxMilliseconds = 3000)
        {
            return elapsed.TotalMilliseconds < maxMilliseconds ?
                (int)elapsed.TotalMilliseconds + "мс." :
                elapsed.ToString();
        }
    }
}