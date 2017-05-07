using System;
using System.Globalization;
using System.IO;

namespace StkCommon.TestUtils
{
    public static class FileUtils
    {
        public static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
        /// <summary>
        /// Получить информацию по файлу
        /// </summary>
        /// <param name="filePath">абсолютный или относительный путь</param>
        /// <returns></returns>
        public static FileInfo GetFileInfo(string filePath)
        {
            return new FileInfo(GetPath(filePath));
        }

        /// <summary>
        /// Получить абсолютный путь до файла
        /// Если передать null - то возвращает путь до базовой директории
        /// </summary>
        public static string GetPath(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
                return AppDomain.CurrentDomain.BaseDirectory;

            return !Path.IsPathRooted(filePath) ?
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath) :
                filePath;
        }

        /// <summary>
        /// Получить размер файла в байтах
        /// </summary>
        /// <param name="filePath">абсолютный или относительный путь</param>
        /// <returns>если файл не существует то 0</returns>
        public static long GetFileSize(string filePath)
        {
            var fileInfo = GetFileInfo(filePath);
            return !fileInfo.Exists ? 0 : fileInfo.Length;
        }

        public static string FormatFileInfo(FileInfo file, string description = "FileInfo")
        {
            return $"{description}: {Environment.NewLine}  FilePath: {file}{Environment.NewLine}  FileSize: {FormatFileSize(file.Length)}";
        }

        public static string FormatFileInfo(string filePath, string description = "FileInfo")
        {
            var fileInfo = GetFileInfo(filePath);
            return FormatFileInfo(fileInfo, description);
        }

        public static string FormatFileSize(string filePath)
        {
            return FormatFileSize(GetFileSize(filePath));
        }

        public static string FormatFileSize(long byteCount)
        {
            if (byteCount == 0)
                return "0" + SizeSuffixes[0];
            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            var sizeStr = (Math.Sign(byteCount) * num).ToString(CultureInfo.InvariantCulture) + SizeSuffixes[place];

            return $"{sizeStr} ({byteCount}byte)";
        }

        public static bool IsSystemFile(string file)
        {
            if (File.GetAttributes(file).HasFlag(FileAttributes.Hidden))
                return true;

            return Path.GetFileName(file) == "Thumbs.db";
        }

        /// <summary>
        /// Удалить файл если существует
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(string filePath)
        {
            var fileInfo = GetFileInfo(filePath);
            if (fileInfo.Exists)
                fileInfo.Delete();
        }
    }
}
