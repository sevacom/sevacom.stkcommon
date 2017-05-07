using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace StkCommon.TestUtils.MockDataBase
{
	/// <summary>
	/// DB configure start point
	/// </summary>
	public partial class MockDb
	{
		private int _cmdIndex = -1;
		private readonly List<MockCommandData> _commands = new List<MockCommandData>();

		public List<MockCommandData> Commands { get { return _commands; } }

		public IDbConnection AsDbConnection()
		{
			return this;
		}

		/// <summary>
		/// New IDataReader query
		/// </summary>
		/// <param name="fields"></param>
		/// <returns></returns>
		public MockDb NewReader<T>(params Expression<Func<T, object>>[] fields)
		{
			SetCurrentNewReader();
			return NextResult(fields);
		}

		/// <summary>
		/// New IDataReader query
		/// </summary>
		/// <param name="fields"></param>
		/// <returns></returns>
		public MockDb NewReader(params string[] fields)
		{
			SetCurrentNewReader();
			return NextResult(fields);
		}

		/// <summary>
		/// Setup filed types for current IDataReader
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		public MockDb WithFieldTypes(params Type[] types)
		{
			var currentResult = CurrentSetupCommandData.ReaderResult.CurrentResult;
			if (types.Length != currentResult.Names.Count)
			{
				throw new ArgumentException("Invalid types count", "types");
			}
			var currTypes = currentResult.Types;

			currTypes.Clear();
			currTypes.AddRange(types);

			return this;
		}

		/// <summary>
		/// Next result into current IDataReader context
		/// </summary>
		/// <param name="fields"></param>
		/// <returns></returns>
		public MockDb NextResult(params string[] fields)
		{
			var data = new MockReaderResultData();
			data.SetNames(fields);
			CurrentSetupCommandData.ReaderResult.CurrentResult = data;
			return this;
		}

		/// <summary>
		/// Next result into current IDataReader context
		/// </summary>
		/// <param name="fields"></param>
		/// <returns></returns>
		public MockDb NextResult<T>(params Expression<Func<T, object>>[] fields)
		{
			var nameList = new List<string>(fields.Length);
			var typeList = new List<Type>(fields.Length);
			foreach (var member in fields.Select(GetPropertyInfo))
			{
				nameList.Add(member.Name);
				typeList.Add(member.PropertyType);
			}

			return NextResult(nameList.ToArray())
				.WithFieldTypes(typeList.ToArray());
		}

		public MockDb NewRow(params object[] values)
		{
			CurrentSetupCommandData.ReaderResult.CurrentResult.Values.Add(values);
			return this;
		}

		public MockDb NewNonQuery(int value = 1)
		{
			CurrentSetupCommandData = new MockCommandData { NonQueryResult = value };
			return this;
		}

		public MockDb NewScalar(object value)
		{
			if (value == null) throw new ArgumentNullException("value");
			return NewReader("").NewRow(value);
		}

		private MockCommandData NextCommand()
		{
			_cmdIndex++;
			if (_cmdIndex == _commands.Count)
			{
				throw new InvalidOperationException("Command not defined");
			}
			return _commands[_cmdIndex];
		}

		private MockCommandData CurrentSetupCommandData
		{
			get { return _commands.LastOrDefault(); }
			set { _commands.Add(value); }
		}

		private static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object>> field)
		{
			var convExpr = field.Body as UnaryExpression;
			var body = null == convExpr ? field.Body : convExpr.Operand;
			var member = (PropertyInfo)((MemberExpression)body).Member;
			return member;
		}

		private void SetCurrentNewReader()
		{
			CurrentSetupCommandData = new MockCommandData { ReaderResult = new MockReaderData() };
		}
	}
}