using System;
using System.Collections.Generic;
using System.Linq;

namespace StkCommon.TestUtils.DbTestFramework
{
    internal static class TestDbHelper
    {
        public static string[] ToParametersRow(this IEnumerable<string> values, object[] parameters)
        {
            return values.Select(v =>
            {
                try
                {
                    return string.Format(v, parameters);
                }
                catch (FormatException)
                {
                    return v;
                }
            }).ToArray();
        }

        public static string GetFieldType(this string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                return null;
            var parts = fieldName.Split(':');
            return parts.Length > 1 ? parts[1].Trim().Replace("[", "").Replace("]", "") : null;
        }

      
        public static string NativeValueToDbString(this object dbValue)
        {
            if (dbValue is DBNull)
            {
                return "null";
            }
            if (dbValue is bool)
            {
                return (bool)dbValue ? "1" : "0";
            }
            var bytes = dbValue as byte[];
            return bytes != null
                ? "0x" + string.Join("", bytes.Select(b => b.ToString("X2")))
                : dbValue.ToString();
        }
    }
}