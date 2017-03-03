using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace StkCommon.TestUtils.DbTestFramework
{
    /// <summary>
    /// Задание "из кода" элементов БД, которые будут созданы и будут использоваться для тестирования, а также параметров теста
    /// </summary>
    public class TestDbSetup : IDisposable
    {
        private const string CreateTableTemplate = "create table {0} ({1})";
        private const string IdentityInsertOnTemplate = "SET IDENTITY_INSERT {0} ON";
        private const string IdentityInsertOffTemplate = "SET IDENTITY_INSERT {0} OFF";
        private const string InsertTemplate = "insert into {0} ({1}) values({2})";
        private const string DropConstraintTemplate = "ALTER TABLE {0}   DROP CONSTRAINT {1}";
        private const string DropIndexTemplate = "DROP INDEX {1} ON {0}";

        private readonly string _connectionString;
        private readonly object[] _parameters;
        private readonly bool _isApplySupported;
        private readonly List<TestDbTable> _tables = new List<TestDbTable>();
        private readonly Dictionary<string, string> _sps = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _udts = new Dictionary<string, string>();

        public TestDbSetup(string connectionString, object[] parameters, bool isApplySupported)
        {
            _connectionString = connectionString;
            _parameters = parameters;
            _isApplySupported = isApplySupported;
        }

        public TestDbSetup Table(string tableName, params string[] fields)
        {
            _tables.Add(new TestDbTable(tableName, fields));
            return this;
        }

        public TestDbSetup Row(params string[] values)
        {
            _tables.Last().Add(values.ToParametersRow(_parameters));
            return this;
        }

        public TestDbSetup Sp(string spName, string text)
        {
            _sps[spName] = text;
            return this;
        }

        public TestDbSetup Udt(string udtName, string text)
        {
            _udts[udtName] = text;
            return this;
        }

        /// <summary>
        /// Задать первичный ключ таблицы. Применяется к последней заданной таблице
        /// </summary>
        /// <param name="name">Название CONSTRAINT'а</param>
        /// <param name="clustered">Кластеризованный ли ключ</param>
        /// <param name="columns">Колонки первичного ключа</param>
        /// <returns></returns>
        public TestDbSetup Pk(string name, bool clustered, params string[] columns)
        {
            var constraint = new TestDbTableConstraint
            {
                Clustered = clustered,
                Name = name,
                ConstraintType = TestDbConstraintType.PkIndexConstraint
            };
            constraint.IndexColumns.AddRange(columns);
            _tables.Last().Constraints.Add(constraint);
            return this;
        }

        /// <summary>
        /// Задать уникальный индекс таблицы. Применяется к последней заданной таблице
        /// </summary>
        /// <param name="name">Название CONSTRAINT'а</param>
        /// <param name="clustered">Кластеризованный ли индекс</param>
        /// <param name="columns">Колонки индекса</param>
        /// <returns></returns>
        public TestDbSetup UnqIndex(string name, bool clustered, params string[] columns)
        {
            var constraint = new TestDbTableConstraint
            {
                Clustered = clustered,
                Name = name,
                ConstraintType = TestDbConstraintType.UniqueIndexConstraint
            };
            constraint.IndexColumns.AddRange(columns);
            _tables.Last().Constraints.Add(constraint);
            return this;
        }

        /// <summary>
        /// Задать ограничение по внешнему ключу (FOREIGN KEY). Применяется к последней заданной таблице
        /// </summary>
        /// <param name="name">Название CONSTRAINT'а</param>
        /// <param name="cascadeOptions">Опции каскадного обновления, удаления (например, ON DELETE CASCADE, ON DELETE CASCADE)</param>
        /// <param name="columns">Список колонок таблицы, для которой определяется внешний ключ</param>
        /// <param name="referencedTable">Название таблицы, на которую ссылается определяемый FOREIGN KEY</param>
        /// <param name="referencedColumns">Список колонок первичного ключа</param>
        /// <returns></returns>
        public TestDbSetup Fk(string name, string cascadeOptions, IEnumerable<string> columns, string referencedTable, IEnumerable<string> referencedColumns)
        {
            var constraint = new TestDbTableConstraint
            {
                Name = name,
                ConstraintType = TestDbConstraintType.ForeignKey,
                ReferencedTable = referencedTable,
                CascadeOptions = cascadeOptions
            };
            constraint.IndexColumns.AddRange(columns);
            constraint.ReferencedColumns.AddRange(referencedColumns);
            _tables.Last().Constraints.Add(constraint);
            return this;
        }

        /// <summary>
        /// Создание в БД предварительно заданных объектов (таблиц, пользовательских типов данных, хранимых процедур). Предварительно из БД удаляются объекты с такими же именами
        /// </summary>
        public void Apply()
        {
            if (!_isApplySupported) throw new NotSupportedException();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    CleanDb(cmd);

                    foreach (var table in _tables)
                    {
                        var pkField = table.Constraints
                            .FirstOrDefault(x => x.ConstraintType == TestDbConstraintType.PkIndexConstraint);

                        var fieldsStr = CreateFieldString(table, pkField);

                        cmd.CommandText = string.Format(CreateTableTemplate, table.TableName, fieldsStr);
                        cmd.ExecuteNonQuery();

                        var isAutoInc = table.Fields.Any(f => f.IsAutoInc);
                        if (isAutoInc)
                        {
                            cmd.CommandText = string.Format(IdentityInsertOnTemplate, table.TableName);
                            cmd.ExecuteNonQuery();
                        }

                        fieldsStr = string.Join(", ", table.Fields.Select(f => f.Name));
                        foreach (var valuesStr in table.Values.Select(values => string.Join(", ", values)))
                        {
                            cmd.CommandText = string.Format(InsertTemplate, table.TableName, fieldsStr, valuesStr);
                            cmd.ExecuteNonQuery();
                        }

                        if (isAutoInc)
                        {
                            cmd.CommandText = string.Format(IdentityInsertOffTemplate, table.TableName);
                            cmd.ExecuteNonQuery();
                        }
                        var constraints = table
                            .GetRelevantConstraints()
                            .OrderBy(x => x.ConstraintType)
                            .Where(x => x.ConstraintType != TestDbConstraintType.Unknown);
                        foreach (var constr in constraints)
                        {
                            if (constr.ConstraintType == TestDbConstraintType.ForeignKey
                                && table.Fields.Where(f => f.IsForeignKey)
                                    .Select(f => f.Name).All(fn => !constr.IndexColumns.Contains(fn)))
                                continue;
                            cmd.CommandText = string.Format(constr.CreateScript, table.TableName);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    foreach (var kv in _udts)
                    {
                        cmd.CommandText = kv.Value;
                        cmd.ExecuteNonQuery();
                    }
                    foreach (var kv in _sps)
                    {
                        cmd.CommandText = kv.Value;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void Verify()
        {
            if (_isApplySupported) throw new NotSupportedException();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    foreach (var table in _tables)
                    {
                        var fieldsStr = string.Join(", ", table.Fields.Select(f => f.Name));

                        cmd.CommandText = string.Format("select {0} from {1}", fieldsStr, table.TableName);
                        using (var reader = cmd.ExecuteReader())
                        {
                            var result = new TestDbResult(table.Fields.Select(f => f.Name).ToArray(), _parameters);
                            foreach (var value in table.Values)
                            {
                                result.AddRow(value);
                            }
                            result.Verify(reader);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Добавить для таблицы с указанным именем список ограничений (в том числе, включая, первичный ключ)
        /// </summary>
        public void SetTableConstraints(string tableName, IEnumerable<TestDbTableConstraint> dbConstraints)
        {
            _tables.First(t => t.TableName == tableName).Constraints.AddRange(dbConstraints);
        }

        public void Dispose()
        {
            if (0 == _tables.Count && 0 == _sps.Count && 0 == _udts.Count) return;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    CleanDb(cmd);
                    _sps.Clear();
                    _udts.Clear();
                    _tables.Clear();
                }
            }
        }

        private void CleanDb(IDbCommand cmd)
        {
            foreach (var kv in _sps.Reverse())
            {
                DropProc(cmd, kv.Key);
            }

            foreach (var constraint in _tables
                .SelectMany(x => x.Constraints.Select(c => new { c, x.TableName }))
                .OrderByDescending(x => x.c.ConstraintType))
            {
                DropConstraint(cmd, constraint.c.ConstraintType, constraint.c.Name, constraint.TableName);
            }

            foreach (var table in _tables)
            {
                DropTable(cmd, table.TableName);
            }

            foreach (var kv in _udts.Reverse())
            {
                DropUdt(cmd, kv.Key);
            }
        }

        private static string CreateFieldString(TestDbTable table, TestDbTableConstraint pkField)
        {
            const string notHolder = "#not#";
            const string identityField = "{0} {1} not null identity(1,1)";
            const string otherField = "{0} {1} " + notHolder + " null";

            Func<string, string, string> replaceNotHolder =
                (s, fieldName) => s.Replace(notHolder,
                                                      pkField != null && pkField.IndexColumns.Contains(fieldName)
                                                      ? " not "
                                                      : string.Empty);

            var fieldNames = table.Fields.Select(f => f.IsAutoInc
                                                     ? string.Format(identityField, f.Name, f.Type)
                                                     : replaceNotHolder(string.Format(otherField, f.Name, f.Type), f.Name));
            return string.Join(", ", fieldNames);
        }

        private static void DropConstraint(IDbCommand cmd, TestDbConstraintType constraintConstraintType, string constraintName, string tableName)
        {
            var text = "if object_id('{1}') is not null ";
            switch (constraintConstraintType)
            {
                case TestDbConstraintType.PkIndexConstraint:
                case TestDbConstraintType.UniqueIndexConstraint:
                case TestDbConstraintType.ForeignKey:
                    text += DropConstraintTemplate;
                    break;

                case TestDbConstraintType.IndexDesc:
                    text += DropIndexTemplate;
                    break;

                default:
                    return;
            }
            cmd.CommandText = string.Format(text, tableName, constraintName);
            cmd.ExecuteNonQuery();
        }

        private static void DropUdt(IDbCommand cmd, string udtName)
        {
            cmd.CommandText = string.Format("if TYPE_ID('{0}') is not null drop type {0}", udtName);
            cmd.ExecuteNonQuery();
        }

        private static void DropProc(IDbCommand cmd, string procName)
        {
            cmd.CommandText = string.Format("if object_id('{0}', 'P') is not null drop procedure {0}", procName);
            cmd.ExecuteNonQuery();
            cmd.CommandText = string.Format("if object_id('{0}', 'FN') is not null or object_id('{0}', 'IF') is not null or object_id('{0}', 'TF') is not null drop function {0}", procName);
            cmd.ExecuteNonQuery();
        }

        private static void DropTable(IDbCommand cmd, string tableName)
        {
            cmd.CommandText = string.Format("if object_id('{0}', 'U') is not null drop table {0}", tableName);
            cmd.ExecuteNonQuery();
        }
    }
}