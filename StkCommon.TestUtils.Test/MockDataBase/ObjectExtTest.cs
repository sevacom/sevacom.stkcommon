using System;
using NUnit.Framework;

namespace StkCommon.TestUtils.Test.MockDataBase
{
	[TestFixture]
	public class ObjectExtTest
	{
		[Test]
		public void ShouldToDataReaderByFields()
		{
			// Given
			var value = new TestMapType { Prop1 = 3, Prop2 = "test" };

			// When
			var reader = value.ToDataReaderByFields(t => t.Prop1, t => t.Prop2);

			// Then
			Assert.AreEqual(2, reader.FieldCount);
			Assert.IsTrue(reader.Read());
			Assert.AreEqual(value.Prop1, reader.GetValue(0));
			Assert.AreEqual(value.Prop2, reader.GetValue(1));
			Assert.AreEqual("Prop1", reader.GetName(0));
			Assert.AreEqual("Prop2", reader.GetName(1));
		}

		[Test]
		public void ShouldToEmptyDataReader()
		{
			// Given

			// When
			var reader = new TestMapType().ToEmptyDataReader();

			// Then
			Assert.AreEqual(4, reader.FieldCount);
			Assert.IsFalse(reader.Read());
			Assert.AreEqual("Prop4", reader.GetName(3));
		}

		[Test]
		public void ShouldToDataReader()
		{
			// Given
			var value = new TestMapType { Prop1 = 3, Prop2 = "test", Prop4 = Guid.NewGuid(), Prop3 = DateTime.Now};

			// When
			var reader = value.ToDataReader();

			// Then
			Assert.AreEqual(4, reader.FieldCount);
			Assert.IsTrue(reader.Read());
			Assert.AreEqual(value.Prop1, reader.GetValue(0));
			Assert.AreEqual(value.Prop2, reader.GetValue(1));
			Assert.AreEqual(value.Prop3, reader.GetValue(2));
			Assert.AreEqual(value.Prop4, reader.GetValue(3));
			Assert.AreEqual("Prop1", reader.GetName(0));
			Assert.AreEqual("Prop2", reader.GetName(1));
			Assert.AreEqual("Prop3", reader.GetName(2));
			Assert.AreEqual("Prop4", reader.GetName(3));
		}

		[Test]
		public void ShouldToDataReaderArray()
		{
			// Given

			// When
			var reader = new[] { new TestMapType { Prop1 = 3 }, new TestMapType { Prop1 = 5 } }.ToDataReader();

			// Then
			Assert.IsTrue(reader.Read());
			Assert.AreEqual(3, reader.GetValue(0));
			Assert.IsTrue(reader.Read());
			Assert.AreEqual(5, reader.GetValue(0));
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