using System.Collections.Generic;
using System.Linq;

namespace StkCommon.TestUtils.DbTestFramework
{
    public class TestDbTable
    {
        public TestDbTable(string tableName, params string[] fields)
        {
            TableName = tableName;

            Fields = new List<TestDbField>(fields.Length);
            Values = new List<string[]>();
            Constraints = new List<TestDbTableConstraint>();
            foreach (var field in fields)
            {
                Fields.Add(new TestDbField(field));
            }
        }

        public string TableName { get; private set; }

        public List<TestDbField> Fields { get; private set; }

        public List<string[]> Values { get; private set; }

        public List<TestDbTableConstraint> Constraints { get; private set; }

        public void Add(string[] values)
        {
            Values.Add(values);
        }

        public IEnumerable<TestDbTableConstraint> GetRelevantConstraints()
        {
            var constraints =
                from constraint in Constraints
                let colNames = constraint.IncludedColumns.Concat(constraint.IndexColumns).ToList()
                where colNames.All(cn => Fields.Any(f => f.Name == cn))
                select constraint;
            return constraints;
        }
    }
}