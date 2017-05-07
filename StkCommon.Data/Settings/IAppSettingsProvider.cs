using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace StkCommon.Data.Settings
{
    public interface IAppSettingsProvider
    {
        /// <summary>
        /// Получить значение настройки
        /// </summary>
        /// <param name="name">Название настройки, не может быть пустым</param>
        /// <param name="defaultValue">Значение если нет настройки</param>
        /// <param name="throwIfNoExist">Исключение если настройка не существует</param>
        /// <returns></returns>
        string GetValueStr(string name, string defaultValue = null, bool throwIfNoExist = false);

        /// <summary>
        /// Получить значения переменной paramName, если не удалось то использовать defaultValue.
        /// convertationError - обработчик при возникновении ошибки
        /// </summary>
        T GetValue<T>(string paramName, T defaultValue, Action<Exception, string> convertationError = null, 
            bool throwIfNoExist = false)
            where T : IConvertible;

        /// <summary>
        /// Возвращает строковое значение параметра конфигурации программы раскодируя его. 
        /// Если параметр не найден, подставляет значение по умолчанию. По умолчанию кодирует и сохраняет значение, если оно не закодировано.
        /// Используется для паролей
        /// </summary>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <param name="saveEncrypted">Если значение параметра не кодировано то оно кодируется и сохраняется в файл конфигурации</param>
        /// <param name="name">Название настройки, не может быть пустым</param>
        /// <param name="throwIfNoExist">Исключение если настройка не существует</param>
        string GetValueEncrypted(string name, string defaultValue, bool saveEncrypted = true, bool throwIfNoExist = false);
    }

    public class AppSettingsProvider : IAppSettingsProvider
    {
        /// <summary>
        /// Получить значение настроки
        /// </summary>
        /// <param name="name">Название настройки, не может быть пустым</param>
        /// <param name="defaultValue">Значение если нет настройки</param>
        /// <param name="throwIfNoExist">Исключение если настройка не существует</param>
        /// <returns></returns>
        public string GetValueStr(string name, string defaultValue = null, bool throwIfNoExist = false)
        {
            return GetValue(name, defaultValue, throwIfNoExist: throwIfNoExist);
        }

        /// <summary>
        /// Получить значения переменной paramName, если не удалось то использовать defaultValue.
        /// convertationError - обработчик при возникновении ошибки
        /// </summary>
        public T GetValue<T>(string name, T defaultValue, Action<Exception, string> convertationError = null, 
            bool throwIfNoExist = false)
            where T : IConvertible
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Название настройки не может быть null или пусто", name);

            var value = ConfigurationManager.AppSettings[name];
            if (value == null && throwIfNoExist)
                throw new ArgumentException("Настройка не найдена", name);

            if(value == null)
                return defaultValue;

            try
            {
                return typeof(T).IsEnum
                    ? (T) Enum.Parse(typeof(T), value)
                    : (T) Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex)
            {
                convertationError?.Invoke(ex, $"Не удалось получить настройку '{name}'");
                return defaultValue;
            } 
        }

        /// <summary>
        /// Возвращает строковое значение параметра конфигурации программы раскодируя его. 
        /// Если параметр не найден, подставляет значение по умолчанию. По умолчанию кодирует и сохраняет значение, если оно не закодировано.
        /// Используется для паролей
        /// </summary>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <param name="saveEncrypted">Если значение параметра не кодировано то оно кодируется и сохраняется в файл конфигурации</param>
        /// <param name="name">Название настройки, не может быть пустым</param>
        /// <param name="throwIfNoExist">Исключение если настройка не существует</param>
        public string GetValueEncrypted(string name, string defaultValue, bool saveEncrypted = true, bool throwIfNoExist = false)
        {
            var value = GetValueStr(name, null, throwIfNoExist);
            if (value == null)
                return defaultValue;

            bool needDecrypt;
            if (IsEncrypt(value, out needDecrypt))
            {
                var res = Decrypt(value);
                if (needDecrypt && saveEncrypted)
                {
                    // Указано что нужно расшифровать пароль, и при этом разрешена авто модификация конфига
                    SaveValue(name, res);
                }

                return res;
            }
            if (saveEncrypted)
            {
                SaveValue(name, Encrypt(value));
            }
            return value;
        }

        private static void SaveValue(string name, string value)
        {
            var appSettingsFilePath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            UpdateAppSettingsKey(appSettingsFilePath, name, value);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private static void UpdateAppSettingsKey(string file, string key, string encrypt)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(file);
                if(xmlDoc.DocumentElement == null)
                    throw new Exception($"Не удалось прочитать файл xml: {file}");

                var element = xmlDoc.DocumentElement.Cast<XmlElement>().FirstOrDefault(e => e.Name.Equals("appSettings"));
                var node = element?.ChildNodes
                    .Cast<XmlNode>()
                    .FirstOrDefault(n => null != n.Attributes && 2 == n.Attributes.Count && n.Attributes[0].Value.Equals(key));

                if (node?.Attributes != null)
                {
                    node.Attributes[1].Value = encrypt;
                }

                xmlDoc.Save(file);
            }
            catch
            {
                // ignored
            }
        }

        private static string Encrypt(string value)
        {
            return "ess:" + CryptoUtility.EncryptToString(value);
        }

        private static string Decrypt(string value)
        {
            try
            {
                var str = value.Split(':')[1];
                return CryptoUtility.DecryptToString(str);
            }
            catch
            {
                return value;
            }
        }

        private static bool IsEncrypt(string value, out bool needDecrypt)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var parts = value.Split(':');
                needDecrypt = 2 < parts.Length;
                return 0 < parts.Length && ("ess" == parts[0]);
            }
            needDecrypt = false;
            return false;
        }

        private static class CryptoUtility
        {
            private const string Key = @"8LNw8uawyfE=";
            private const string Iv = @"mTsMJqB1/Zs=";

            public static string EncryptToString(string value)
            {
                value = value ?? "";
                // Добавление немного мусора в строку чтобы она получилась пооригинальнее
                var str = (DateTime.Now.Hour % 10).ToString(CultureInfo.InvariantCulture)
                    + (DateTime.Now.Minute % 10).ToString(CultureInfo.InvariantCulture)
                    + (DateTime.Now.Second % 10).ToString(CultureInfo.InvariantCulture)
                    + value + value + value;
                str = StrToStr(str
                    , Encoding.UTF8.GetBytes
                    , (des, key, iv) => des.CreateEncryptor(key, iv)
                    , Convert.ToBase64String);
                var index1 = str[0] % 10;
                index1 = index1 < 5 ? 8 : index1;

                var str2 = StrToStr(str
                    , Encoding.UTF8.GetBytes
                    , (des, key, iv) => des.CreateEncryptor(key, iv)
                    , Convert.ToBase64String);
                var index2 = str2[0] % 20;
                index2 = index2 < 5 ? 9 : index2;

                return str.Insert(index1, str2.Substring(0, index2));
            }

            public static string DecryptToString(string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }
                var index1 = value[0] % 10;
                index1 = index1 < 5 ? 8 : index1;

                var index2 = value[index1] % 20;
                index2 = index2 < 5 ? 9 : index2;

                value = value.Remove(index1, index2); // Убран мусор

                value = StrToStr(value
                    , Convert.FromBase64String
                    , (des, key, iv) => des.CreateDecryptor(key, iv)
                    , Encoding.UTF8.GetString);
                return value.Substring(3, (value.Length - 3) / 3);
            }

            private static string StrToStr(string value, Func<string, byte[]> strToByte, Func<DES, byte[], byte[], ICryptoTransform> transform, Func<byte[], string> byteToStr)
            {
                using (var fout = new MemoryStream())
                {
                    using (var fin = new MemoryStream(strToByte(value)))
                    {
                        var bin = new byte[100];
                        long rdlen = 0;
                        var totlen = fin.Length;

                        DES des = new DESCryptoServiceProvider();
                        byte[] key = Convert.FromBase64String(Key);
                        byte[] iv = Convert.FromBase64String(Iv);
                        using (var encStream = new CryptoStream(fout, transform(des, key, iv), CryptoStreamMode.Write))
                        {
                            while (rdlen < totlen)
                            {
                                var len = fin.Read(bin, 0, 100);
                                encStream.Write(bin, 0, len);
                                rdlen = rdlen + len;
                            }

                            encStream.Flush();
                            encStream.FlushFinalBlock();
                            fout.Position = 0;
                            return byteToStr(fout.ToArray());
                        }

                    }
                }
            }
        }
    }
}