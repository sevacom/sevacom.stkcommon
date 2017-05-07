using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using StkCommon.Data.Extensions;
using StkCommon.TestUtils;

namespace StkCommon.Data.Test.Extensions
{
	[TestFixture]
	public class DateTimeExTest: AutoMockerTestsBase
	{
		private DateTime _expectedNow;

		/// <summary>
		/// Помечен атрибутом SetUp вызывается перед каждым тестом.
		/// </summary>
		public override void SetUp()
		{
			base.SetUp();
			_expectedNow = new DateTime(2000, 1, 1);
		}

		[Test]
		public void ShouldNowPropertyMockedWithMockNow()
		{
			//Given
			DateTimeEx.MockNow(_expectedNow);

			//When //Then
			DateTimeEx.Now.Should().Be(_expectedNow);
		}

		[Test]
		public void ShouldNowPropertyMockedWithIDateTimeProvider()
		{
			//Given
			GetMock<IDateTimeProvider>().Setup(p => p.Now).Returns(_expectedNow);
			DateTimeEx.Mock(Get<IDateTimeProvider>());

			//When //Then
			DateTimeEx.Now.Should().Be(_expectedNow);
			GetMock<IDateTimeProvider>().Verify(p => p.Now, Times.Once);
		}

		[Test]
		public void ShouldResetMockNow()
		{
			//Given
			ShouldNowPropertyMockedWithMockNow();

			//When 
			DateTimeEx.Reset();

			//Then
			DateTimeEx.Now.Should().NotBe(_expectedNow);
		}

		[Test]
		public void ShouldResetMockIDateTimeProvider()
		{
			//Given
			ShouldNowPropertyMockedWithIDateTimeProvider();

			//When 
			DateTimeEx.Reset();

			//Then
			DateTimeEx.Now.Should().NotBe(_expectedNow);
		}
	}
}
