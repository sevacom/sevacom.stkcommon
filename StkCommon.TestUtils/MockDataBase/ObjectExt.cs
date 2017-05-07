using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using StkCommon.TestUtils.MockDataBase;

namespace System
{
	public static class ObjectExt
	{
		public static IDataReader ToDataReaderByFields<T>(this T self, params Expression<Func<T, object>>[] fields)
		{
			var db = new MockDb()
				.NewReader(fields)
				.NewRow(fields.Select(f => f.Compile()(self)).ToArray());
			return db.AsDbConnection().CreateCommand().ExecuteReader();
		}

		public static IDataReader ToEmptyDataReader(this object self)
		{
			return ArrayToDataReader(new[] { self }, true);
		}

		public static IDataReader ToDataReader(this object self)
		{
			return ArrayToDataReader(new[] { self }, false);
		}

		public static IDataReader ToDataReader<T>(this IEnumerable<T> objects)
		{
			return ArrayToDataReader(objects.ToArray(), false);
		}

		private static IDataReader ArrayToDataReader<T>(T[] objects, bool isEmpty)
		{
			var props = objects.First().GetType().GetProperties()
				.Where(p => p.PropertyType.IsPrimitive || p.PropertyType == typeof(string) || p.PropertyType == typeof(Guid)
					|| p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(byte[]) || p.PropertyType == typeof(char[]))
				.ToArray();

			var db = new MockDb()
				.NewReader(props.Select(p => p.Name).ToArray())
				.WithFieldTypes(props.Select(p => p.PropertyType).ToArray());

			if (!isEmpty)
			{
				foreach (var o in objects)
				{
					var localO = o;
					db.NewRow(props.Select(p => p.GetValue(localO, null)).ToArray());
				}
			}

			return db.AsDbConnection().CreateCommand().ExecuteReader();
		}
	}
}