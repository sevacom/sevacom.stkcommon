using NUnit.Framework;
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable ClassNeverInstantiated.Local

namespace StkCommon.Data.Test
{
	[TestFixture]
	public class PropertyNameTest
	{
		[Test]
		public void CanGetPropertyName_UsingLambda()
		{
			Assert.AreEqual("Name", PropertyName.For<Person>(x => x.Name));
			Assert.AreEqual("Age", PropertyName.For<Person>(x => x.Age));
		}

		[Test]
		public void CanGetPropertyName_Composite_UsingLambda()
		{
			Assert.AreEqual("Home.City", PropertyName.For<Person>(x => x.Home.City));
			Assert.AreEqual("Home.FlatNumber", PropertyName.For<Person>(x => x.Home.FlatNumber));
		}

		private class Person
		{
			public string Name { get; set; }
			public int Age { get; set; }
			public Home Home { get; set; }
		}

		private class Home
		{
			public string City { get; set; }
			public string FlatNumber { get; set; }
		}

	}
}