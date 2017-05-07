using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.Data.Common;
using StkCommon.Data.Extensions;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace StkCommon.Data.Test.Extensions
{
	[TestFixture]
	public class DescriptionExtensionsTest
	{
		[Test]
		public void ShouldGetDescription()
		{
			//Given
			var firstField = typeof(AuditEvents.VideoControl)
				.GetFields()
				.First();

			//When
			var res1 = firstField.GetCustomAttributes<DescriptionAttribute>(false);
			var res2 = firstField.GetDescription();
			var res3 = firstField.GetCustomAttribute<DescriptionAttribute>().Description;

			//Then
			res1.Should().ContainSingle(p => p.Description == "Создание ОТМ");
			res2.Should().Be("Создание ОТМ");
			res3.Should().Be("Создание ОТМ");

		}
	}

	public static class AuditEvents
	{
		public static class VideoControl
		{
			/// <summary>
			/// Создание ОТМ
			/// </summary>
			[Description("Создание ОТМ")]
			public static int CreateTask = 1901;

			/// <summary>
			/// Обновление ОТМ
			/// </summary>
			public static int UpdateTask = 1902;

		}
	}
}