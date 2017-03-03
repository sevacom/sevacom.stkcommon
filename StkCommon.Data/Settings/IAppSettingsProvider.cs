using System;
using System.Configuration;

namespace StkCommon.Data.Settings
{
    public interface IAppSettingsProvider
    {
        /// <summary>
        /// Получить значение настроки
        /// </summary>
        /// <param name="name">Название настройки, не может быть пустым</param>
        /// <param name="defaultValue">Значение если нет настройки</param>
        /// <returns></returns>
        string GetValue(string name, string defaultValue);

        /// <summary>
        /// Получить значения переменной paramName, если не удалось то использовать defaultValue.
        /// convertationError - обработчик при возникновении ошибки
        /// </summary>
        T GetValue<T>(string paramName, T defaultValue, Action<Exception, string> convertationError = null)
            where T : IConvertible;
    }

    public class AppSettingsProvider : IAppSettingsProvider
    {
        /// <summary>
        /// Получить значение настроки
        /// </summary>
        /// <param name="name">Название настройки, не может быть пустым</param>
        /// <param name="defaultValue">Значение если нет настройки</param>
        /// <returns></returns>
        public string GetValue(string name, string defaultValue = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Название настройки не может быть null или пусто");

            return ConfigurationManager.AppSettings[name] ?? defaultValue;
        }

        /// <summary>
        /// Получить значения переменной paramName, если не удалось то использовать defaultValue.
        /// convertationError - обработчик при возникновении ошибки
        /// </summary>
        public T GetValue<T>(string paramName, T defaultValue, Action<Exception, string> convertationError = null)
            where T : IConvertible
        {
            var value = defaultValue;
            var valueStr = GetValue(paramName);
            if (!string.IsNullOrEmpty(valueStr))
            {
                try
                {
                    if (typeof(T).IsEnum)
                    {
                        value = (T)Enum.Parse(typeof(T), valueStr);
                    }
                    else
                    {
                        value = (T)Convert.ChangeType(valueStr, typeof(T));
                    }
                }
                catch (Exception ex)
                {
                    if (convertationError != null)
                        convertationError(ex, string.Format("Не удалось получить настройку '{0}'", paramName));
                }
            }
            return value;
        }
    }
}