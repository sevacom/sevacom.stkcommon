using System;
using System.Collections.Generic;

namespace StkCommon.TestUtils.DbTestFramework
{
    public class TestDbTableConstraint
    {
        private const string ForeignTemplate = "ALTER TABLE {0} {#nocheck#} ADD CONSTRAINT {#name#} FOREIGN KEY({#colname#}) {#reference#} {#cascade#}";
        private const string PrimaryKeyTemplate = "ALTER TABLE {0} ADD CONSTRAINT {#name#} PRIMARY KEY {#clustered#} ({#colname#}) ";
        private const string IndexTemplate = "CREATE {#clustered#} INDEX {#name#}  ON {0} ({#colname#}) {#included#}";
        private const string UniqueTemplate = "ALTER TABLE {0} ADD CONSTRAINT {#name#} UNIQUE {#clustered#}  ({#colname#})";

        public TestDbTableConstraint()
        {
            IndexColumns = new List<string>();
            ReferencedColumns = new List<string>();
            IncludedColumns = new List<string>();
        }

        public string Name { get; set; }

        public TestDbConstraintType ConstraintType { get; set; }

        public List<string> IndexColumns { get; private set; }

        public string ReferencedTable { get; set; }

        public List<string> ReferencedColumns { get; private set; }

        public string CascadeOptions { get; set; }

        public bool WithNoCheck { get; set; }

        public bool Clustered { get; set; }

        public List<string> IncludedColumns { get; private set; }

        public string CreateScript
        {
            get
            {
                switch (ConstraintType)
                {
                    case TestDbConstraintType.PkIndexConstraint:
                        return FormatIndexCreateScript(PrimaryKeyTemplate);

                    case TestDbConstraintType.IndexDesc:
                        return FormatIndexCreateScript(IndexTemplate);

                    case TestDbConstraintType.UniqueIndexConstraint:
                        return FormatIndexCreateScript(UniqueTemplate);

                    case TestDbConstraintType.ForeignKey:
                        return ForeignTemplate
                                 .Replace("{#nocheck#}", WithNoCheck ? "WITH NOCHECK" : string.Empty)
                                 .Replace("{#name#}", Name)
                                 .Replace("{#colname#}", string.Join(",", IndexColumns))
                                 .Replace("{#reference#}", GetForeignKeyRerences())
                                 .Replace("{#cascade#}", CascadeOptions);

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private string FormatIndexCreateScript(string template)
        {
            var temp = string.Join(",", IncludedColumns);
            var included = string.IsNullOrEmpty(temp) ? string.Empty : " INCLUDE (" + temp + ")";
            var indexKeys = string.Join(", ", IndexColumns);
            return template.Replace("{#name#}", Name)
                .Replace("{#clustered#}", Clustered ? "CLUSTERED" : "NONCLUSTERED")
                .Replace("{#colname#}", indexKeys)
                .Replace("{#included#}", included);
        }

        private string GetForeignKeyRerences()
        {
            if (string.IsNullOrEmpty(ReferencedTable)) return string.Empty;
            return "REFERENCES " + ReferencedTable + " (" + string.Join(",", ReferencedColumns) + ")";
        }
    }

    public enum TestDbConstraintType
    {
        Unknown,
        PkIndexConstraint,
        IndexDesc,
        UniqueIndexConstraint,
        ForeignKey
    }

    internal static class Helper
    {
        public static TR MayBe<T, TR>(this T obj, Func<T, TR> selector) where T : class
        {
            return obj == null ? default(TR) : selector(obj);
        }
    }
}