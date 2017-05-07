using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace StkCommon.TestUtils
{
    public static class ExecuteUtils
    {
        /// <summary>
        /// Выполнить action для каждого файла с указанием информации о файле, вывод в консоль
        /// </summary>
        public static void ExecuteByFilesInFolder(string folderPath, Action<string> action,
            bool printFileInfo = true, string searchPattern = "*.*",
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            var testFiles = Directory
                .GetFiles(FileUtils.GetPath(folderPath), searchPattern, searchOption)
                .Where(testFile => !FileUtils.IsSystemFile(testFile));

            foreach (var testFile in testFiles)
            {
                if (printFileInfo)
                    Console.WriteLine(FileUtils.FormatFileInfo(testFile, "Execute by File"));
                action(testFile);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Выполнить action для каждого файла с указанием времени выполнения и названия операции, вывод в консоль
        /// </summary>
        /// <param name="folderPath">абсолютный или относительный путь</param>
        /// <param name="action"></param>
        /// <param name="opearionName"></param>
        /// <param name="printFileInfo"></param>
        /// <param name="searchPattern"></param>
        /// <param name="searchOption"></param>
        public static void ExecuteByFilesInFolderWithElapsed(string folderPath, Action<string> action, 
            string opearionName, bool printFileInfo = true, string searchPattern = "*.*",
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            ExecuteByFilesInFolder(folderPath, p => ExecuteWithElapsed(() => action(p), opearionName),
                printFileInfo, searchPattern, searchOption);
        }

        /// <summary>
        /// Выполнить action с указанием времени выполнения и операции, вывод в консоль
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="operationName"></param>
        /// <param name="elapsed"></param>
        /// <returns></returns>
        public static T ExecuteWithElapsed<T>(Func<T> action, string operationName, out TimeSpan elapsed)
        {
            var watch = Stopwatch.StartNew();
            var res = action();
            watch.Stop();
            elapsed = watch.Elapsed;
            ElapsedUtils.FormatOperationWithElapsedTime(watch, operationName);
            return res;
        }

        public static T ExecuteWithElapsed<T>(Func<T> action, string operationName)
        {
            TimeSpan elapsed;
            return ExecuteWithElapsed(action, operationName, out elapsed);
        }

        public static void ExecuteWithElapsed(Action action, string operationName, out TimeSpan elapsed)
        {
            ExecuteWithElapsed<object>(() =>
            {
                action();
                return null;
            }, operationName, out elapsed);
        }

        public static void ExecuteWithElapsed(Action action, string operationName)
        {
            TimeSpan elapsed;
            ExecuteWithElapsed(action, operationName, out elapsed);
        }
    }
}