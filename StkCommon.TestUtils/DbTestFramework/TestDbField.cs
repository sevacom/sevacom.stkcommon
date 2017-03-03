using System;
using System.Collections.Generic;

namespace StkCommon.TestUtils.DbTestFramework
{
    public class TestDbField
    {
        public TestDbField(string fieldStr)
        {
            if (fieldStr == null) throw new ArgumentNullException("fieldStr");

            var parts = fieldStr.Split(':');
            Parse(parts);
        }

        public string Name { get; private set; }

        public bool IsAutoInc { get; set; }

        public bool IsForeignKey { get; set; }

        public string Type { get; set; }

        public override string ToString()
        {
            return (IsAutoInc ? "+" : string.Empty)
                + (IsForeignKey ? "&" : string.Empty)
                + Name
                + (string.IsNullOrEmpty(Type) ? string.Empty : ":" + Type);
        }

        private void Parse(IList<string> parts)
        {
            var fieldName = parts[0].Trim().Replace("[", "").Replace("]", "");
            IsAutoInc = fieldName.StartsWith("+") || fieldName.EndsWith("+");
            IsForeignKey = fieldName.StartsWith("&") || fieldName.EndsWith("&");
            Name = fieldName.Replace("+", string.Empty).Replace("&", string.Empty);

            if (parts.Count == 1) return;
            Type = parts[1].Trim().Replace("[", "").Replace("]", "").ToLower();
        }
    }
}