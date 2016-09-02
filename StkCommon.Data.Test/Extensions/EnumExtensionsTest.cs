using FluentAssertions;
using NUnit.Framework;
using StkCommon.Data.Extensions;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace StkCommon.Data.Test.Extensions
{
	[TestFixture]
	public class EnumExtensionsTest
	{
		[TestCase(TestEnum.Value1, false, "")]
		[TestCase(TestEnum.Value1, true, "Value1")]
		[TestCase(1, true, "Value1")]
		[TestCase(1, false, "")]
		[TestCase(TestEnum.Value2, true, "2")]
		[TestCase(TestEnum.Value2, false, "2")]
		[TestCase(TestEnum.Value3, false, "3")]
		[TestCase(TestEnum.Value4, false, "custom")]
		[TestCase((TestEnum)4, false, "custom")]
		[TestCase(4, false, "custom")]
		[TestCase(5, false, "")]
		[TestCase(5, true, "5")]
		public void ShouldGetDescription(TestEnum testEnum, bool showValueIfNotDescription,
			string expectedDescription)
		{
			//Given
			//When
			//Then
			testEnum.GetDescription(showValueIfNotDescription)
				.ShouldBeEquivalentTo(expectedDescription);
		}

		/// <summary>
		/// “естовое перечисление без атрибута описани€
		/// </summary>
		public enum TestEnum
		{
			Value1 = 1,
			[Description("2")]
			Value2 = 2,
			[Description("3")]
			Value3 = 3,
			[Description("custom")]
			Value4 = 4
		}
	}
}
