using System;
using System.Data;
using NUnit.Framework;
using System.Linq;
using StkCommon.TestUtils.MockDataBase;

namespace StkCommon.TestUtils.Test.MockDataBase
{
	[TestFixture]
	public class MockDbTest : AutoMockerTestsBase<MockDb>
	{

		[Test]
		public void ShouldNewReaderFromProperty()
		{
			// Given
			int @int;
			string @string;
			DateTime dateTime;
			Guid guid;

			// When
			Target
				.NewReader<TestMapType>(t => t.Prop1, t => t.Prop2, t => t.Prop3, t => t.Prop4);
			SetupNewReaderRow(out @int, out @string, out dateTime, out guid);

			// Then
			AssertNewReader(@int, @string, dateTime, guid);
		}

		[Test]
		public void ShouldNewReaderFromFields()
		{
			// Given
			int @int;
			string @string;
			DateTime dateTime;
			Guid guid;

			// When
			Target
				.NewReader("Prop1", "Prop2", "Prop3", "Prop4");
			SetupNewReaderRow(out @int, out @string, out dateTime, out guid);

			// Then
			AssertNewReader(@int, @string, dateTime, guid);
		}

		[Test]
		public void ShouldNewReaderFromFieldsWithTypes()
		{
			// Given

			// When
			Target
				.NewReader("Prop1", "Prop2", "Prop3", "Prop4")
				.WithFieldTypes(typeof(int), typeof(string), typeof(DateTime), typeof(Guid));

			// Then
			AssertNewReader();
		}

		[Test]
		public void ShouldManyResults()
		{
			// Given

			// When
			Target
				.NewReader<TestMapType>(t => t.Prop1)
					.NewRow(10)
				.NextResult<TestMapType>(t => t.Prop2, t => t.Prop4)
					.NewRow("super", Guid.NewGuid());

			// Then
			var reader = Target.AsDbConnection().CreateCommand().ExecuteReader();
			Assert.AreEqual(1, reader.FieldCount);
			Assert.AreEqual(typeof(int), reader.GetFieldType(0));
			Assert.IsTrue(reader.Read());
			Assert.AreEqual(10, reader.GetInt32(0));

			Assert.IsTrue(reader.NextResult());
			Assert.AreEqual(2, reader.FieldCount);
			Assert.AreEqual(typeof(string), reader.GetFieldType(0));
			Assert.IsTrue(reader.Read());
			Assert.AreEqual("super", reader.GetString(0));

			Assert.IsFalse(reader.NextResult());
		}

		[Test]
		public void ShouldManyReaders()
		{
			// Given

			// When
			Target
				.NewReader<TestMapType>(t => t.Prop1)
					.NewRow(10)
				.NewReader<TestMapType>(t => t.Prop2, t => t.Prop4)
					.NewRow("super", Guid.NewGuid());

			// Then
			var connection = Target.AsDbConnection();
			var reader = connection.CreateCommand().ExecuteReader();
			Assert.AreEqual(1, reader.FieldCount);
			Assert.AreEqual(typeof(int), reader.GetFieldType(0));
			Assert.IsTrue(reader.Read());
			Assert.AreEqual(10, reader.GetInt32(0));

			reader = connection.CreateCommand().ExecuteReader();
			Assert.AreEqual(2, reader.FieldCount);
			Assert.AreEqual(typeof(string), reader.GetFieldType(0));
			Assert.IsTrue(reader.Read());
			Assert.AreEqual("super", reader.GetString(0));
		}

		[Test]
		public void ShouldExecuteScalar1()
		{
			// Given

			// When
			Target
				.NewReader<TestMapType>(t => t.Prop2, t => t.Prop1)
					.NewRow("test", 10)
					.NewRow("super", 20);

			// Then
			var value = Target.AsDbConnection().CreateCommand().ExecuteScalar();
			Assert.AreEqual("test", value);
		}

		[Test]
		public void ShouldExecuteScalar2()
		{
			// Given
			var guid = Guid.NewGuid();

			// When
			Target.NewScalar(guid);

			// Then
			var value = Target.AsDbConnection().CreateCommand().ExecuteScalar();
			Assert.AreEqual(guid, value);
		}

		[Test]
		public void ShouldExecuteNonQuery()
		{
			// Given

			// When
			Target.NewNonQuery(7);

			// Then
			var value = Target.AsDbConnection().CreateCommand().ExecuteNonQuery();
			Assert.AreEqual(7, value);
		}

		[Test]
		public void ShouldExecuteCombine()
		{
			// Given

			// When
			Target
				.NewReader("TestProp").NewRow(3)
				.NewScalar("super")
				.NewNonQuery(11);

			// Then
			var connection = Target.AsDbConnection();
			var reader = connection.CreateCommand().ExecuteReader();
			Assert.AreEqual(1, reader.FieldCount);
			Assert.IsTrue(reader.Read());
			Assert.AreEqual(3, reader.GetValue(0));

			var value = connection.CreateCommand().ExecuteScalar();
			Assert.AreEqual("super", value);

			value = connection.CreateCommand().ExecuteNonQuery();
			Assert.AreEqual(11, value);
		}

		[Test]
		public void ShouldAllTypesSupport()
		{
			// Given
			var byteArray = new byte[] { 1, 2, 3, 4 };
			var charArray = new[] { 'a', 's', 'd' };
			var expectedValues = new object[]
				{
					true, (byte)128, 'q', new DateTime(2016, 12, 2), (decimal)1, 1.1, (float)2.2, Guid.NewGuid(), (short)3, 4, (long)5, "test", byteArray, charArray
				};


			// When
			Target
				.NewReader("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13")
				.NewRow(expectedValues);


			// Then
			var reader = Target.AsDbConnection().CreateCommand().ExecuteReader();
			reader.Read();
			Assert.AreEqual(expectedValues[0], reader.GetBoolean(0));
			Assert.AreEqual(expectedValues[1], reader.GetByte(1));
			Assert.AreEqual(expectedValues[2], reader.GetChar(2));
			Assert.AreEqual(expectedValues[3], reader.GetDateTime(3));
			Assert.AreEqual(expectedValues[4], reader.GetDecimal(4));
			Assert.AreEqual(expectedValues[5], reader.GetDouble(5));
			Assert.AreEqual(expectedValues[6], reader.GetFloat(6));
			Assert.AreEqual(expectedValues[7], reader.GetGuid(7));
			Assert.AreEqual(expectedValues[8], reader.GetInt16(8));
			Assert.AreEqual(expectedValues[9], reader.GetInt32(9));
			Assert.AreEqual(expectedValues[10], reader.GetInt64(10));
			Assert.AreEqual(expectedValues[11], reader.GetString(11));

			var bBuffer = new byte[100];
			Assert.AreEqual(2, reader.GetBytes(12, 2, bBuffer, 5, bBuffer.Length));
			Assert.IsTrue(byteArray.Skip(2).SequenceEqual(bBuffer.Skip(5).Take(2)));

			var cBuffer = new char[100];
			Assert.AreEqual(charArray.Length, reader.GetChars(13, 0, cBuffer, 0, cBuffer.Length));
			Assert.IsTrue(charArray.SequenceEqual(cBuffer.Take(charArray.Length)));

			var values = new object[100];
			Assert.AreEqual(expectedValues.Length, reader.GetValues(values));
			Assert.IsTrue(expectedValues.SequenceEqual(values.Take(expectedValues.Length)));
		}

		[Test]
		public void ShouldFieldOperationSupported()
		{
			// Given
			var fields = new[] { "Prop1", "Prop2", "Prop3" };
			var types = new[] { typeof(int), typeof(string), typeof(Guid) };

			// When
			Target.NewReader(fields).WithFieldTypes(types);

			// Then
			var reader = Target.AsDbConnection().CreateCommand().ExecuteReader();

			Assert.AreEqual("Int32", reader.GetDataTypeName(0));
			Assert.AreEqual(typeof(string), reader.GetFieldType(1));
			Assert.AreEqual("Prop2", reader.GetName(1));
			Assert.AreEqual(2, reader.GetOrdinal("Prop3"));

			var table = reader.GetSchemaTable();
			Assert.IsTrue(table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).SequenceEqual(fields));
			Assert.IsTrue(table.Columns.Cast<DataColumn>().Select(c => c.DataType).SequenceEqual(types));
		}

		private void SetupNewReaderRow(out int @int, out string @string, out DateTime dateTime, out Guid guid)
		{
			@int = 1;
			@string = "2";
			dateTime = new DateTime(2016, 12, 02);
			guid = Guid.NewGuid();

			// When
			Target.NewRow(@int, @string, @dateTime, @guid);
		}

		private void AssertNewReader(int? @int = null, string @string = null, DateTime? dateTime = null, Guid? guid = null)
		{
			var reader = Target.AsDbConnection().CreateCommand().ExecuteReader();
			Assert.AreEqual(4, reader.FieldCount);

			Assert.AreEqual(typeof(int), reader.GetFieldType(0));
			Assert.AreEqual(typeof(string), reader.GetFieldType(1));
			Assert.AreEqual(typeof(DateTime), reader.GetFieldType(2));
			Assert.AreEqual(typeof(Guid), reader.GetFieldType(3));

			Assert.AreEqual("Prop1", reader.GetName(0));
			Assert.AreEqual("Prop2", reader.GetName(1));
			Assert.AreEqual("Prop3", reader.GetName(2));
			Assert.AreEqual("Prop4", reader.GetName(3));

			var expectedRead = null != @int && null != @string && null != dateTime && null != guid;
			Assert.AreEqual(expectedRead, reader.Read());
			if (!expectedRead) return;

			Assert.AreEqual(@int, reader.GetValue(0));
			Assert.AreEqual(@string, reader.GetValue(1));
			Assert.AreEqual(@dateTime, reader.GetValue(2));
			Assert.AreEqual(@guid, reader.GetValue(3));
		}

		private class TestMapType
		{
			public int Prop1 { get; set; }

			public string Prop2 { get; set; }

			public DateTime Prop3 { get; set; }

			public Guid Prop4 { get; set; }
		}
	}
}