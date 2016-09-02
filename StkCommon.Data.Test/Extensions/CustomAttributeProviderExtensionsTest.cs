using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.Data.Extensions;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace StkCommon.Data.Test.Extensions
{
	[TestFixture]
	public class CustomAttributeProviderExtensionsTest
	{
		[Test]
		public void ShouldGetCustomAttributes()
		{
			//Given
			var firstField = typeof(AuditEvents.VideoControl)
				.GetFields()
				.First();
			var secondField = typeof(AuditEvents.VideoControl)
				.GetFields()
				.ElementAt(1);

			//When
			var firstFieldAttributes = firstField.GetCustomAttributes<DescriptionAttribute>(false);
			var firstFieldAttribute = firstField.GetCustomAttribute<DescriptionAttribute>();
			var secondFieldAttribute = secondField.GetCustomAttribute<DescriptionAttribute>();

			//Then
			firstFieldAttributes.Should().ContainSingle(p => p.Description == "Создание ОТМ");
			firstFieldAttribute.Should().NotBeNull();
			firstFieldAttribute.Description.Should().Be("Создание ОТМ");
			secondFieldAttribute.Should().BeNull();
		}
	}
}